using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Data.SQLite;
using BrightIdeasSoftware;
using ScintillaNET;
using System.Xml;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;

namespace SynNotes {

  public partial class Form1 : Form {
    
    // local vars
    IniFile ini;
    const string conffile = "settings.ini";
    const string dbfile = "notes.db";
    const string dbver = "1";
    string userdir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SynNotes\\";
    KeyHook hook;                  // global hotkeys hook
    SQLiteConnection sql;          // sqlite connection handler
    Note note;                     // model for right part of the screen
    List<TagItem> tags = new List<TagItem>();    // minimal metadata cache for tags from db
    List<NoteItem> notes = new List<NoteItem>(); // minimal metadata cache for notes from db
    TagItem tagDeleted;            // pointer to DELETED tag
    TagItem tagAll;                // pointer to ALL tag
    Dictionary<string, List<scStyle>> lexers = new Dictionary<string, List<scStyle>>(StringComparer.InvariantCultureIgnoreCase);
    Dictionary<string, List<string>> keywords = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
    public static Timer saveTimer; // autosave
    public static System.Timers.Timer syncTimer;    // auto-sync
    int treeTopLine, treeSelLine;  // used to restore tree position after restore of window
    string ConnString = "";        //params to connect to db 
    TaskScheduler ui; 

    #region init/close
    public Form1() {
      InitializeComponent();
      statusBar.Padding = new Padding(0, 0, 0, 0); //http://stackoverflow.com/questions/2646606/how-do-i-reclaim-the-space-from-the-grip
    }

    //this event fires before form shown, doing inits which affect form appearance
    private void Form1_Move(object sender, EventArgs e) {
      //only for first start
      if (note == null) {
        note = new Note(this);
        // read settings from ini
        if (File.Exists(userdir + conffile)) ini = new IniFile(userdir + conffile);
        else ini = new IniFile(conffile);
        this.WindowState = (FormWindowState)FormWindowState.Parse(this.WindowState.GetType(), ini.GetValue("Form", "WindowState", "Normal"));
        this.Form1_Resize(this, null); //trigger tray icon
        if (this.WindowState == FormWindowState.Normal && !ini.defaults) {
          this.Top = Int32.Parse(ini.GetValue("Form", "Top", "100"));
          this.Left = Int32.Parse(ini.GetValue("Form", "Left", "100"));
          this.Width = Int32.Parse(ini.GetValue("Form", "Width", "500"));
          this.Height = Int32.Parse(ini.GetValue("Form", "Height", "400"));
        }
        // check db
        if (File.Exists(dbfile)) sqlConnect(dbfile);
        else if (File.Exists(userdir + dbfile)) sqlConnect(userdir + dbfile);
        else {
          sqlConnect(dbfile, false);
          if (sql == null) sqlConnect(userdir + dbfile);
          sqlCreate();
        }
        // inits        
        initTree();
        initScintilla();
      }
    }

    //non affecting appearance inits
    private void Form1_Shown(object sender, EventArgs e) {
      ui = TaskScheduler.FromCurrentSynchronizationContext();
      //delay inits after form drawn
      Task.Factory.StartNew(() => {
        // read sync acc
        using (SQLiteCommand cmd = new SQLiteCommand("SELECT value FROM config WHERE name='email'", sql)) {
          var res = cmd.ExecuteScalar();
          if (res != null) Sync.Email = (string)res;
          cmd.CommandText = "SELECT value FROM config WHERE name='password'";
          res = cmd.ExecuteScalar();
          if (res != null) Sync.Password = (string)res;
          cmd.CommandText = "SELECT value FROM config WHERE name='freq'";
          res = cmd.ExecuteScalar();
          if (res != null) Sync.Freq = int.Parse((string)res);
          cmd.CommandText = "SELECT value FROM config WHERE name='lastSync'";
          res = cmd.ExecuteScalar();
          if (res != null) Sync.LastSync = int.Parse((string)res);
        }
        //init auto-sync timer
        if (syncTimer == null) {
          syncTimer = new System.Timers.Timer();
          syncTimer.Elapsed += timer_sync;          
        }
        if (Sync.Freq != 0) {
          syncTimer.Interval = Sync.Freq * 60 * 1000;
          syncTimer.Start();
          SnSync(0); // start full sync on start
        }
      }).ContinueWith(t => {
        // send mouse wheel event to control under cursor
        Application.AddMessageFilter(new MouseWheelMessageFilter());

        // hotkeys
        hook = new KeyHook();
        hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(HotkeyPressed);
        hook.SetHotkey(1, ini.GetValue("Keys", "HotkeyShow", ""));
        string s = ini.GetValue("Keys", "HotkeySearch", "Win+`");
        tbSearch.AccessibleDescription = "Search Notes (" + s + ")"; //used for placeholder in search bar
        hook.SetHotkey(2, s);
      }, ui);
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
      //save ini settings file
      try{
        ini.SaveSettings(conffile);
      }
      catch{
        if (!Directory.Exists(userdir)) Directory.CreateDirectory(userdir);
        ini.SaveSettings(userdir + conffile);
      }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
      // store form settings for saving
      if (this.WindowState == FormWindowState.Normal) {
        ini.SetValue("Form", "Top", this.Top.ToString());
        ini.SetValue("Form", "Left", this.Left.ToString());
        ini.SetValue("Form", "Width", this.Width.ToString());
        ini.SetValue("Form", "Height", this.Height.ToString());
      }
      ini.SetValue("Form", "WindowState", this.WindowState.ToString());
      //autosave      
      if (sql != null) {
        if (scEdit.Modified) note.Save();
        if (note.Item != null) {
          using (SQLiteTransaction tr = sql.BeginTransaction()) {
            using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
              cmd.CommandText = "UPDATE tags SET expanded=0";
              cmd.ExecuteNonQuery();
              foreach (var item in tree.ExpandedObjects) {
                var tag = item as TagItem;
                if (tag != null) {
                  cmd.CommandText = "UPDATE tags SET expanded=1 WHERE id=" + tag.Id;
                  cmd.ExecuteNonQuery();
                }
              }
              cmd.CommandText = "INSERT OR REPLACE INTO config(name,value) VALUES('lastNote'," + note.Item.Id + ")";
              cmd.ExecuteNonQuery();
            }
            tr.Commit();
          }
        }
        //close db connection
        sql.Dispose();
      }
    }

    /// <summary>
    /// create db schema
    /// </summary>
    private void sqlCreate() {
      try {
        using (SQLiteTransaction tr = sql.BeginTransaction()) {
          using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
            //per-db configuration
            cmd.CommandText = "CREATE TABLE config("+
              "name TEXT PRIMARY KEY NOT NULL,"+
              "value TEXT"+
            ") WITHOUT ROWID;"+
            "INSERT INTO config VALUES('ver', " + dbver + ");"; //current db schema ver
            cmd.ExecuteNonQuery();
            //tags
            cmd.CommandText = "CREATE TABLE tags(" +
              "id INTEGER PRIMARY KEY NOT NULL," +
              "name TEXT," +
              "`index` INTEGER," + //order in the list
              "version INTEGER," + //track tag content changes
              "expanded BOOLEAN NOT NULL DEFAULT 0," + //should it be expanded on start
              "lexer TEXT," +      //lexer to use
              "share TEXT)";       //array of emails
            cmd.ExecuteNonQuery();
            //notes
            cmd.CommandText = "CREATE TABLE notes(" +
              "id INTEGER PRIMARY KEY NOT NULL," + //my id
              "key TEXT," +        //simplenote id
              "deleted BOOLEAN NOT NULL DEFAULT 0," + //in trash or not
              "modifydate REAL," + //unixtime of last edit
              "createdate REAL," + //unixtime of creation
              "syncnum INTEGER," + //track note changes
              "version INTEGER," + //track note content changes
              "systemtags TEXT," + //array of not-parsed tags
              "pinned BOOLEAN NOT NULL DEFAULT 0," +  //displayed before others
              "unread BOOLEAN NOT NULL DEFAULT 0," +  //modified shared note
              "title TEXT," +      //copy of first line of text
              "content TEXT," +    //note content, including the first line
              "lexer TEXT," +      //lexer to use
              "topline INTEGER)";  //top visible line
            cmd.ExecuteNonQuery();
            //full-text search
            cmd.CommandText = "CREATE VIRTUAL TABLE fts USING fts4(" +
              "content=\"notes\"," +
              "title," +             //lower(notes.title) for search only by title
              "content," +           //lower(notes.content) with first line for search by content
              "matchinfo=fts3," +    //reduce size footprint
              "tokenize=unicode61)"; //remove diacritics"
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TRIGGER notes_bu BEFORE UPDATE ON notes BEGIN"+
              "  DELETE FROM fts WHERE docid=old.id;"+
              "END;"+
              "CREATE TRIGGER notes_bd BEFORE DELETE ON notes BEGIN"+
              "  DELETE FROM fts WHERE docid=old.id;"+
              "END;"+
              "CREATE TRIGGER notes_au AFTER UPDATE ON notes BEGIN"+
              "  INSERT INTO fts(docid, title, content) VALUES(new.id, new.title, new.content);"+
              "END;"+
              "CREATE TRIGGER notes_ai AFTER INSERT ON notes BEGIN"+
              "  INSERT INTO fts(docid, title, content) VALUES(new.id, new.title, new.content);" +
              "END;";
            cmd.ExecuteNonQuery();
            //many-to-many notes-to-tags
            cmd.CommandText = "CREATE TABLE nt("+
              "note INTEGER REFERENCES notes(id) ON DELETE CASCADE,"+
              "tag INTEGER REFERENCES tags(id) ON DELETE CASCADE,"+
              "PRIMARY KEY (note, tag)"+
            ") WITHOUT ROWID;"+
            "CREATE INDEX ix_nt_tag ON nt(tag);";
            cmd.ExecuteNonQuery();
          }
          tr.Commit();
        }
      }
      catch (Exception e) {
        MessageBox.Show("Unable to provision db: " + sql.DataSource + "\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        this.Close();
      }
    }

    /// <summary>
    /// try to connect to provided db, create if not exist
    /// </summary>
    private void sqlConnect(string db, bool warn = true) {
      try {
        var connString = new SQLiteConnectionStringBuilder();
        connString.DataSource = db;
        connString.FailIfMissing = false;
        connString.ForeignKeys = true;
        ConnString = connString.ToString();

        sql = new SQLiteConnection(ConnString);
        sql.Open();
        //check if db valid and throw ex, as Open do nothing
        using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM config WHERE name='ver'", sql)) {
          if(Convert.ToString(cmd.ExecuteScalar()) != dbver){
            //update db schema
          }
        }
      }
      catch(Exception e) {
        if (warn) {
          MessageBox.Show("Unable to use db: " + db + "\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          this.Close();
        }
      }
    }
    #endregion init/close

    #region hotkeys
    /// <summary>
    /// called for registered hotkeys
    /// </summary>
    private void HotkeyPressed(object sender, KeyPressedEventArgs e) {
      // Show hotkey  
      if (this.WindowState == FormWindowState.Minimized) {
        NativeMethods.ShowWindow(this.Handle, NativeMethods.SW_RESTORE);
        //restore selected
        if (tree.SelectedObject == null) {
          tree.TopItemIndex = treeTopLine;
          tree.SelectedIndex = treeSelLine;
        }
      }
      else {
        this.Activate();
        this.BringToFront();
      }
      //Search hotkey
      if (e.id == 2) this.tbSearch.Focus();
      //statusText.Text = e.Modifier.ToString().Replace(", ", "+") + "+" + e.Key.ToString();
    }
    // local keys
    private void Form1_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Escape && e.Modifiers == Keys.None) {
        if (tbSearch.ForeColor == SystemColors.WindowText && tbSearch.Text.Length > 0) {
          tbSearch.Text = "";
          if (!tbSearch.Focused) tbSearch.Focus();
        }
        else {
          treeTopLine = tree.TopItemIndex;
          treeSelLine = tree.SelectedIndex;
          this.WindowState = FormWindowState.Minimized;
        }
      }
      else if (e.KeyCode == Keys.Delete && tree.Focused) deleteSelected();
      else if (e.KeyCode == Keys.F7 && e.Modifiers == Keys.None) createNote();
    }
    #endregion hotkeys

    #region notify icon
    private void notifyIcon1_Click(object sender, EventArgs e) {
      MouseEventArgs me = (MouseEventArgs)e;
      if (me.Button == MouseButtons.Left) NativeMethods.ShowWindow(this.Handle, NativeMethods.SW_RESTORE);
    }

    private void Form1_Resize(object sender, EventArgs e) {
      // show tray icon when minimized
      if (this.WindowState == FormWindowState.Minimized) {
        notifyIcon1.Visible = true;
        this.ShowInTaskbar = false;
      }
      else if (notifyIcon1.Visible) {
        notifyIcon1.Visible = false;
        this.ShowInTaskbar = true;
      }
    }

    private void exitToolStripMenuItem1_Click(object sender, EventArgs e) {
      this.Close();
    }

    private void showToolStripMenuItem1_Click(object sender, EventArgs e) {
      NativeMethods.ShowWindow(this.Handle, NativeMethods.SW_RESTORE);
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
      this.Close();
    }
    #endregion notify icon

    #region tree 
    /// <summary>
    /// switch tree to tags view
    /// </summary>
    private void treeAsTags() {
      //view
      tree.RowHeight = -1;
      tree.EmptyListMsg = "";
      tree.IsSimpleDragSource = true;
      tree.IsSimpleDropSink = true;      
      SimpleDropSink sink = (SimpleDropSink)tree.DropSink;
      sink.CanDropOnSubItem = false;
      sink.CanDropOnBackground = false;
      sink.FeedbackColor = SystemColors.Highlight;
      sink.Billboard.BackColor = SystemColors.Control;

      //getters
      tree.Roots = null;
      tree.CanExpandGetter = delegate(object x) {  // should be a little faster than use x.Count (=notes.FindAll)
        var tag = x as TagItem;
        if (tag != null) {
          if (!tag.System) return notes.Exists(n => !n.Deleted && n.Tags.Contains(tag));
          else {
            if (tag == tagAll) return notes.Exists(n => !n.Deleted);
            else return notes.Exists(n => n.Deleted);
          }
        }
        return false;
      };
      tree.ChildrenGetter = delegate(object x) {
        return ((TagItem)x).Notes;
      };
      tree.ParentGetter = delegate(object x) {
        var n = x as NoteItem;
        if (n != null) {
          if (n.Tags.Count > 0) return n.Tags[0];
          else return tagAll;
        }
        return null;
      };
      cSort.AspectGetter = delegate(object x) {  //hidden column used for sorting
        var tag = x as TagItem;
        if (tag != null) return tag.Index;
        else {
          var n = x as NoteItem;
          if (n.Pinned) return "";
          return n.Name;
        }
      };
      tree.Roots = tags;
      tree.Sort(cSort, SortOrder.Ascending);     //sort by this hidden column
      cName.Renderer = fancyRenderer; //OLV drop renderer when Roots assigned
      //select current
      if(note != null && note.Item != null) tree.Reveal(notes.Find(x => x.Id == note.Item.Id), true);
    }

    /// <summary>
    /// switch tree to list view, update it with search results
    /// </summary>
    /// <param name="query">seacrh query</param>
    private void treeAsList(string query) {
      //view
      if (tree.RowHeight < 0) {
        tree.Roots = null;
        tree.RowHeight = 64;
        tree.CanExpandGetter = null;
        tree.ChildrenGetter = null;
        tree.EmptyListMsg = "";
        tree.IsSimpleDragSource = false;
        tree.IsSimpleDropSink = false;
        cSort.AspectGetter = delegate(object x) {  //hidden column used for sorting
          var note = x as NoteItem;
          if (note != null) return note.Relevance;
          return 0;
        }; 
      }
      //search
      if (!query.Contains("*")) { //search by prefixes by default
        query = query + "*";
        query = query.Replace(" ", "* ");
      }

      //detach from UI thread
      Task.Factory.StartNew<List<NoteItem>>((s) => {
        return readNotes(sql, tags, (string)s);
      }, query).ContinueWith(search => {
        tree.Roots = search.Result;
        tree.Sort(cSort, SortOrder.Descending);
        cName.Renderer = fancyRenderer; //OLV drop renderer when Roots assigned
        //select first result
        if (search.Result.Count > 0) {
          tree.SelectedIndex = 0;
          note.ShowSelected();
          tree.Reveal(tree.SelectedObject, true);
        }
        else tree.EmptyListMsg = "0 results found";
      }, ui);

    }

    //init tree with root items from db
    private void initTree() {
      //read tags from db
      TagItem node;
      using (SQLiteCommand cmd = new SQLiteCommand("SELECT id, name, `index`, expanded, lexer, version FROM tags ORDER BY `index`", sql)) {
        using (SQLiteDataReader rdr = cmd.ExecuteReader()) {
          while (rdr.Read()) {
            node = new TagItem(notes);
            node.Id = rdr.GetInt32(0);
            node.Name = rdr.GetString(1);
            node.Index = rdr.GetInt32(2);
            node.Expanded = rdr.GetBoolean(3);
            if (!rdr.IsDBNull(4)) node.Lexer = rdr.GetString(4);
            if (!rdr.IsDBNull(5)) node.Version = rdr.GetInt32(5);
            tags.Add(node);
          }
        }
      }
      //All
      tagAll = new TagItem(notes);
      tagAll.Name = Glob.All;
      tagAll.System = true;
      tagAll.Index = int.MaxValue - 1;
      tags.Add(tagAll);
      //Deleted
      tagDeleted = new TagItem(notes);
      tagDeleted.Name = Glob.Deleted;
      tagDeleted.System = true;
      tagDeleted.Index = int.MaxValue;
      tags.Add(tagDeleted);

      //get notes from db (filling tags)
      notes.AddRange(readNotes(sql, tags));
      treeAsTags();// startup view is tags view

      //restore view
      foreach (var tag in tags) {
        if (tag.Expanded) tree.Expand(tag);
      }

      //search last opened note
      using (SQLiteCommand cmd = new SQLiteCommand("SELECT value FROM config WHERE name='lastNote'", sql)) {
        var res = cmd.ExecuteScalar();
        if (res == null) createNote(); //first run - create new note and select it
        else tree.Reveal(notes.Find(x => x.Id == Convert.ToInt32(res)), true);
        if (tree.SelectedObject == null) tree.Reveal(notes[0], true);
        treeTopLine = tree.TopItemIndex;
        treeSelLine = tree.SelectedIndex;
      }
    }

    /// <summary>
    /// get notes from db with tags
    /// </summary>
    static List<NoteItem> readNotes(SQLiteConnection sql, List<TagItem> tags, string query=""){
      List<NoteItem> result = new List<NoteItem>();
      string s;
      if (query.Length > 0)
        s = @"SELECT n.id, n.title, n.modifydate, n.deleted, c.tag, n.lexer, n.pinned, snippet(fts, '<b>', '</b>', '...'), matchinfo(fts)" +
        " FROM fts s LEFT JOIN notes n ON s.docid=n.id LEFT JOIN nt c ON c.note=n.id LEFT JOIN tags t ON t.id=c.tag" +
        " WHERE NOT n.deleted AND fts MATCH ?"+
        " ORDER BY n.id, t.`index`";
      else
        s = "SELECT n.id, n.title, n.modifydate, n.deleted, c.tag, n.lexer, n.pinned, n.key, n.syncnum" +                                        
        " FROM notes n LEFT JOIN nt c ON c.note=n.id LEFT JOIN tags t ON t.id=c.tag" +
        " ORDER BY n.id, t.`index`";
      NoteItem node = new NoteItem();
      int id = -1;
      int tmp;
      using (SQLiteCommand cmd = new SQLiteCommand(s, sql)) {
        if (query.Length > 0) cmd.Parameters.AddWithValue(null, query);
        using (SQLiteDataReader rdr = cmd.ExecuteReader()) {          
          while (rdr.Read()) {
            tmp = rdr.GetInt32(0);
            if (tmp != id) {
              id = tmp;
              node = new NoteItem();
              node.Id = id;
              node.Name = rdr.GetString(1);
              node.ModifyDate = rdr.GetDouble(2);
              node.Deleted = rdr.GetBoolean(3);
              node.Tags = new List<TagItem>();              
              if (!rdr.IsDBNull(4)) node.Tags.Add(tags.Find(x => x.Id == rdr.GetInt32(4)));
              if (!rdr.IsDBNull(5)) node.Lexer = rdr.GetString(5);
              node.Pinned = rdr.GetBoolean(6);
              if (query.Length > 0) { // search results
                node.Snippet = rdr.GetString(7);
                node.Relevance = getRelevance(rdr,8);
              }
              else {                  // sync attrs
                if (!rdr.IsDBNull(7)) node.Key = rdr.GetString(7);
                if (!rdr.IsDBNull(8)) node.SyncNum = rdr.GetInt32(8);
              }
              result.Add(node);
            }
            else node.Tags.Add(tags.Find(x => x.Id == rdr.GetInt32(4)));
          }
        }
      }
      return result;
    }

    // parse sqlite matchinfo
    // https://www.sqlite.org/fts3.html#matchinfo
    private static int getRelevance(SQLiteDataReader rdr, int col) {
      byte[] buffer = new byte[rdr.GetBytes(col, 0, null, 0, 0)];
      try {
        rdr.GetBytes(col, 0, buffer, 0, buffer.Length);
        int[] info = GetIntArrayFromByteArray(buffer);
        int res=0;
        var p = info[0];
        var c = info[1];
        for (int i = 0; i < p; i++) {
          for (int j = 0; j < c; j++) {
            var hits = info[3 * (j + i * c) + 2];//In the current row, the number of times the phrase appears in the column.
            if (hits > 0 && j == 0) hits += 100; //matched in title
            res += hits;
          }          
        }
        return res;
      }
      catch(Exception e) {
        MessageBox.Show("Search error: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      return 0;
    }

    private static int[] GetIntArrayFromByteArray(byte[] byteArray) {
      int[] intArray = new int[byteArray.Length / 4];
      for (int i = 0; i < byteArray.Length; i += 4)
        intArray[i / 4] = BitConverter.ToInt32(byteArray, i);
      return intArray;
    }
    #endregion tree

    #region tree events
    // change note in right pane
    private void tree_SelectionChanged(object sender, EventArgs e) {
      if (scEdit.Modified) note.Save();
      note.ShowSelected();
    }

    // expand tag by key / mouse click
    private void tree_ItemActivate(object sender, EventArgs e) {
      if (tree.SelectedObject is TagItem) tree.ToggleExpansion(tree.SelectedObject);
    }

    //activate note on tree click if it is search result
    private void tree_MouseClick(object sender, MouseEventArgs e) {
      if (tree.RowHeight > 0 && e.Clicks == 1) scEdit.Focus();
    }

    //tag expand/collapse by clicking on icon
    private void tree_CellClick(object sender, CellClickEventArgs e) {
      if (e.Location.X < 16) {
        if (e.Model is TagItem) tree.ToggleExpansion(e.Model);
      }
    }

    // edit only valid for tags
    private void tree_CellEditStarting(object sender, CellEditEventArgs e) {
      if (e.RowObject is NoteItem) e.Cancel = true;
    }

    // basic checks
    private void tree_CellEditValidating(object sender, CellEditEventArgs e) {
      if (e.Cancel) return;
      var delims = new char[] { ' ', ',', ';' };
      var val = ((TextBox)e.Control).Text.Trim(delims);
      if (val == e.Value.ToString()) return;
      if (val.IndexOfAny(delims) > 0) {
        MessageBox.Show("Tag name contains invalid characters: ' ,;'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        e.Cancel = true;
        return;
      }
      if (tags.Exists(x => !x.System && x != e.RowObject && x.Name.ToLower() == val.ToLower())) {
        MessageBox.Show("Tag with name '" + val + "' already exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        e.Cancel = true;
        return;
      }
    }

    //rename tag
    private void tree_CellEditFinishing(object sender, CellEditEventArgs e) {
      if (e.Cancel) return;
      var tag = ((TagItem)e.RowObject);
      tag.Version = 0; // reset version for re-sync
      var delims = new char[] { ' ', ',', ';' };
      var oldval = tag.Name;
      tag.Name = e.NewValue.ToString().Trim(delims);
      using (SQLiteTransaction tr = sql.BeginTransaction()) {
        using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
          cmd.CommandText = "UPDATE tags SET name=?, version=0 WHERE id=?";
          cmd.Parameters.AddWithValue(null, tag.Name);
          cmd.Parameters.AddWithValue(null, tag.Id);
          cmd.ExecuteNonQuery();
          // update modifydate of all affected notes for sync
          var now = (DateTime.UtcNow.Subtract(Glob.Epoch)).TotalSeconds;
          tag.Notes.ForEach(x => x.ModifyDate = now);
          cmd.CommandText = "UPDATE notes SET modifydate=? WHERE id IN (SELECT note FROM nt WHERE tag=?)";
          cmd.Parameters.AddWithValue(null, now);
          cmd.Parameters.AddWithValue(null, tag.Id);
          cmd.ExecuteNonQuery();
        }
        tr.Commit();
      }
      note.RenameLabel(oldval); //rename tagBox label if exist
    }
    #endregion tree events

    #region tree context menu
    // fill tree context menu with items valid for row
    private void tree_CellRightClick(object sender, CellRightClickEventArgs e) {
      treeMenu.Items.Clear();
      var tag = e.Model as TagItem;
      //for tags
      if (tag != null) {        
        if (tag == tagDeleted && tree.SelectedObjects.Count == 1) treeMenu.Items.Add("Purge Deleted", null, purgeTagClick);
        else treeMenu.Items.Add("New Note (F7)", null, btnAdd_ButtonClick);
        if (!tag.System) {
          if (tree.SelectedObjects.Count == 1) treeMenu.Items.Add("Rename (F2)", null, renTagClick);
          treeMenu.Items.Add("Delete (Del)", null, delClick);
          if (tree.SelectedObjects.Count == 1) {
            ToolStripMenuItem lex = (ToolStripMenuItem)treeMenu.Items.Add("Default Lexer");
            foreach (var l in Glob.Lexers) {
              var i = lex.DropDownItems.Add(l, null, lexTagClick);
              if (tag.Lexer == l) ((ToolStripMenuItem)i).Checked = true;
            }
          }
        }
      }
      //for notes
      else {
        var n = e.Model as NoteItem;
        if (n == null) return; // right click on blank space
        if (n.Deleted) {
          treeMenu.Items.Add("Restore", null, restoreClick);
          treeMenu.Items.Add("Purge (Del)", null, delClick);
        }
        else {
          if(tree.RowHeight == -1) treeMenu.Items.Add("New Note (F7)", null, btnAdd_ButtonClick);
          if(n.Pinned) treeMenu.Items.Add("Unpin Note", null, pinClick);
          else treeMenu.Items.Add("Pin Note", null, pinClick);
          treeMenu.Items.Add("Delete (Del)", null, delClick);
        }
      }
    }

    //pin note
    private void pinClick(object sender, EventArgs e) {
      pinNote();
    }

    //set lexer for tag
    private void lexTagClick(object sender, EventArgs e) {
      var tag = tree.SelectedObject as TagItem;
      if(tag != null){
        var s = (ToolStripMenuItem)sender;
        tag.Lexer = s.Text;
        using (SQLiteTransaction tr = sql.BeginTransaction()) {
          using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
            cmd.CommandText = "UPDATE tags SET lexer=? WHERE id="+tag.Id;
            cmd.Parameters.AddWithValue(null, s);
            cmd.ExecuteNonQuery();
          }
          tr.Commit();
        }
        //update opened note lexer if inherited
        if (note.Item.Tags.Contains(tag) && String.IsNullOrEmpty(note.Item.Lexer)) {
          note.SetLanguage(s.Text);
          btnLexer.Text = "^" + s.Text;
        }
      }
    }

    //restore note
    private void restoreClick(object sender, EventArgs e) {
      using (SQLiteTransaction tr = sql.BeginTransaction()) {
        using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
          foreach (var i in tree.SelectedObjects) {
            var n = i as NoteItem;
            if (n != null && n.Deleted) {
              n.Deleted = false;
              n.ModifyDate = (DateTime.UtcNow.Subtract(Glob.Epoch)).TotalSeconds;
              cmd.CommandText = "UPDATE notes SET deleted=0, modifydate=? WHERE id=" + n.Id;
              cmd.Parameters.AddWithValue(null, n.ModifyDate);
              cmd.ExecuteNonQuery();              
              tree.RefreshObject(tagAll);
              n.Tags.ForEach(x => tree.RefreshObject(x));
            }
          }
        }
        tr.Commit();
      }
      tree.RefreshObject(tagDeleted);
    }

    //purge deleted notes
    private void purgeTagClick(object sender, EventArgs e) {
      if (tagDeleted.Count > 0) {
        tree.Expand(tagDeleted);
        tree.SelectedObjects = tagDeleted.Notes;
        deleteSelected();
      }
    }

    //rename tag
    private void renTagClick(object sender, EventArgs e) {
      tree.EditModel(tree.SelectedObject);
    }

    //delete selected items
    private void delClick(object sender, EventArgs e) {
      deleteSelected();
    }

    //bottom buttons 
    private void btnAdd_ButtonClick(object sender, EventArgs e) {
      createNote();
    }
    private void btnPin_Click(object sender, EventArgs e) {
      pinNote();
    }

    /// <summary>
    /// creates new note under selected tag and make it active
    /// </summary>
    private void createNote() {
      // get parent tag if something selected
      TagItem tag;
      if (tree.SelectedObject != null) {
        if (tree.SelectedObject is TagItem) tag = (TagItem)tree.SelectedObject;
        else tag = (TagItem)tree.GetParent(tree.SelectedObject);
        if (tag == tagDeleted) tag = tagAll; //don't create new deleted notes
      }
      else tag = tagAll;
      //add new note to db
      var node = new NoteItem();
      node.ModifyDate = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
      node.Name = "New Note";
      if(!tag.System) node.Tags.Add(tag);
      using (SQLiteTransaction tr = sql.BeginTransaction()) {
        using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
          cmd.CommandText = "INSERT INTO notes(modifydate, createdate, title, content) VALUES(@mdate, @mdate, @title, @title)";
          cmd.Parameters.AddWithValue("@mdate", node.ModifyDate);
          cmd.Parameters.AddWithValue("@title", node.Name);
          cmd.ExecuteNonQuery();
          node.Id = sql.LastInsertRowId;
          if (!tag.System) {
            cmd.CommandText = "INSERT INTO nt(note,tag) VALUES(" + node.Id + "," + tag.Id + ")";
            cmd.ExecuteNonQuery();
          }
        }
        tr.Commit();
      }
      // refresh tree root, select new leaf
      notes.Add(node);
      tree.RefreshObject(tag);
      tree.Expand(tag);
      tree.Reveal(node, true);
      scEdit.Focus();
    }

    /// <summary>
    /// deletes all selected objects
    /// </summary>
    private void deleteSelected() {
      var n = tree.SelectedObjects.Count;
      if (n > 1 && MessageBox.Show("Delete " + n.ToString() + " objects?", "Delete multiple objects?",
        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No) return;

      using (SQLiteTransaction tr = sql.BeginTransaction()) {
        using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
          foreach (var item in tree.SelectedObjects) {
            var tag = item as TagItem;
            // delete tag
            if (tag != null) {
              if (tag.System) continue; //can't del system folder
              if (n > 1 || MessageBox.Show("Delete the tag: " + tag.Name + "?\n(This will not delete it's Notes, just unassign this Tag from them)", "Delete Tag?",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes) 
                  deleteTag(tag);
            }
            //delete note
            else {
              var i = (NoteItem)item;
              if (tree.RowHeight > 0) i = notes.Find(x => x.Id == i.Id);
              //purge it
              if (i.Deleted) {
                if (n == 1 && MessageBox.Show("Purge the note: " + i.Name + "?\n(This will purge the Note, no undelete is possible)", "Purge Note?",
                  MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No) return;
                purgeNote(i);
                continue;
              }
              //move to deleted
              else {
                if (n == 1 && MessageBox.Show("Delete the note: " + i.Name + "?\n(This will move the Note to Deleted items folder)", "Delete Note?",
                  MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No) return;
                i.Deleted = true;
                i.ModifyDate = (DateTime.UtcNow.Subtract(Glob.Epoch)).TotalSeconds;
                cmd.CommandText = "UPDATE notes SET deleted=1, modifydate=? WHERE id=" + i.Id;
                cmd.Parameters.AddWithValue(null, i.ModifyDate);
                cmd.ExecuteNonQuery();                
                if (tree.RowHeight == -1) tree.RefreshObject(tagAll);
              }
              //update tree
              if (tree.RowHeight == -1) {
                tree.RefreshObject(tagDeleted);
                i.Tags.ForEach(x => tree.RefreshObject(x));
              }
              else treeAsList(tbSearch.Text);
            }
          }
        }
        tr.Commit();
      }
      if (n > 1 && tree.SelectedObjects.Count > 0) tree.SelectedObject = tree.SelectedObjects[0]; //reset the selection
    }

    // purge tag
    private void deleteTag(TagItem tag, bool doSync = true) {
      using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
        cmd.CommandText = "DELETE FROM tags WHERE id=" + tag.Id; // no need to cleanup `nt` because of cascade
        cmd.ExecuteNonQuery();
      }
      tags.Remove(tag);
      notes.ForEach(x => x.Tags.Remove(tag));
      if (tree.RowHeight == -1) {
        tree.RemoveObject(tag);
        cName.Renderer = fancyRenderer; //OLV drop renderer when Roots refreshed
      }
      note.RemoveLabel(null, tag); //del from tagBox if exist

      // task to sync deletion
      if (doSync && Sync.Freq > 0 && tag.Version>0) {
        try {
          Task.Factory.StartNew(() => { Sync.delTag(tag); });
        }
        catch {
          statusText.Text = "Unable to sync delete";
        }
      }
    }

    // purge note
    private void purgeNote(NoteItem i, bool doSync = true) {
      using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
        cmd.CommandText = "DELETE FROM notes WHERE id=" + i.Id; // no need to cleanup `nt` because of cascade
        cmd.ExecuteNonQuery();
      }
      notes.Remove(i);      
      // refresh tags
      if (tree.RowHeight == -1) {
        if (i.Deleted) tree.RefreshObject(tagDeleted);
        else i.Tags.ForEach(x => tree.RefreshObject(x));
        if ((note.Item == null || note.Item.Id == i.Id) && notes.Count > 0) tree.Reveal(notes[0], true);
      }
      else treeAsList(tbSearch.Text);

      // task to sync deletion
      if (doSync && Sync.Freq > 0 && !string.IsNullOrEmpty(i.Key)) {
        try {
          Task.Factory.StartNew(() => { Sync.delNote(i); });
        }
        catch {
          statusText.Text = "Unable to sync delete";
        }
      }
    }

    /// <summary>
    /// (un)pin selected note
    /// </summary>
    private void pinNote() {
      var note = tree.SelectedObject as NoteItem;
      if (note != null) {
        note.Pinned = note.Pinned ? false : true;
        note.ModifyDate = (DateTime.UtcNow.Subtract(Glob.Epoch)).TotalSeconds;
        if (tree.RowHeight > 0) { // we're in search mode
          note = notes.Find(x => x.Id == note.Id);
          note.Pinned = note.Pinned ? false : true;
        }        
        //save to db
        using (SQLiteTransaction tr = sql.BeginTransaction()) {
          using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
            cmd.CommandText = "UPDATE notes SET pinned=?, modifydate=? WHERE id=" + note.Id;
            cmd.Parameters.AddWithValue(null, note.Pinned);
            cmd.Parameters.AddWithValue(null, note.ModifyDate);
            cmd.ExecuteNonQuery();
          }
          tr.Commit();
        }
        if (tree.RowHeight > 0) tree.RefreshObject(tree.SelectedObject);
        else tree.RefreshObject(tree.GetParent(note)); //resort
      }
    }
    #endregion tree context menu

    #region tree drag'n'drop
    //started to drag
    private void tree_ItemDrag(object sender, ItemDragEventArgs e) {
      //what is dragged?
      var containNotes = false;
      var containTags = false;
      foreach (var i in tree.SelectedObjects) {
        if (i is NoteItem) containNotes = true;
        else containTags = true;
      }
      //configure helper
      var sink = (SimpleDropSink)tree.DropSink;
      if (!containNotes) { //tag can drop only between tags
        sink.CanDropBetween = true;
        sink.AcceptableLocations = DropTargetLocation.BetweenItems;
      }
      else if(!containTags) {  //note can drop only to tag
        sink.CanDropBetween = false;
        sink.AcceptableLocations = DropTargetLocation.Item;
      }
    }

    //check if can drop here
    private void tree_ModelCanDrop(object sender, ModelDropEventArgs e) {
      e.Handled = true;  
      e.Effect = DragDropEffects.None;
      if(e.TargetModel == null || e.SourceModels.Contains(e.TargetModel)) return; //drop to self
      var tag = e.TargetModel as TagItem;
      if (tag != null) {
        //what is dropped?
        var containNotes = false;
        var containTags = false;
        foreach (var i in e.SourceModels) {
          if (i is NoteItem) containNotes = true;
          else containTags = true;
        }
        if (!containNotes && e.DropTargetLocation != DropTargetLocation.Item && tag != tagDeleted ) e.Effect = DragDropEffects.Move; //can rearrange tags
        if (!containTags) {
          if ((e.DragEventArgs.KeyState & 8) == 8) e.Effect = DragDropEffects.Copy; //Ctrl pressed - add tag
          else e.Effect = DragDropEffects.Move; //can drop notes to tags
        }
      }
    }

    //process the dropped
    private void tree_ModelDropped(object sender, ModelDropEventArgs e) {
      switch (e.DropTargetLocation) {        
        case DropTargetLocation.AboveItem:
          moveTag(e.SourceModels, (TagItem)e.TargetModel);
          break;
        case DropTargetLocation.BelowItem:
          moveTag(e.SourceModels, (TagItem)e.TargetModel, 1);
          break;
        case DropTargetLocation.Item:
          moveNote(e.SourceModels, (TagItem)e.TargetModel, e.StandardDropActionFromKeys);
          e.RefreshObjects();          
          break;
      }
      //OLV drops renderer/sort when Roots assigned
      cName.Renderer = fancyRenderer; 
      tree.Sort(cSort, SortOrder.Ascending);
    }

    //note change/add tag
    private void moveNote(IList from, TagItem to, DragDropEffects how) {
      using (SQLiteTransaction tr = sql.BeginTransaction()) {
        using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
          var sqlTag = new SQLiteParameter();
          foreach (var i in from) {
            var n = i as NoteItem;
            if (n.Deleted && to == tagDeleted) continue;
            if (!n.Deleted && to == tagDeleted) { //delete
              n.Deleted = true;
              n.ModifyDate = (DateTime.UtcNow.Subtract(Glob.Epoch)).TotalSeconds;
              cmd.CommandText = "UPDATE notes SET deleted=1, modifydate=? WHERE id=" + n.Id;
              cmd.Parameters.AddWithValue(null, n.ModifyDate);
              cmd.ExecuteNonQuery();
            }
            else {              
              if (n.Deleted && to != tagDeleted) {  //undelete
                n.Deleted = false;
                cmd.CommandText = "UPDATE notes SET deleted=0 WHERE id=" + n.Id;
                cmd.ExecuteNonQuery();
              }
              n.ModifyDate = (DateTime.UtcNow.Subtract(Glob.Epoch)).TotalSeconds;
              cmd.CommandText = "UPDATE notes SET modifydate=? WHERE id=" + n.Id;
              cmd.Parameters.AddWithValue(null, n.ModifyDate);
              cmd.ExecuteNonQuery();
              if (to == tagAll) continue; 
              if (how == DragDropEffects.Move) n.Tags.Clear();
              n.Tags.Add(to);
              //save to db
              cmd.CommandText = "DELETE FROM nt WHERE note=" + n.Id;
              cmd.ExecuteNonQuery();
              cmd.CommandText = "INSERT INTO nt(note,tag) VALUES(?,?)";
              cmd.Parameters.Clear();
              cmd.Parameters.AddWithValue(null, n.Id);
              cmd.Parameters.Add(sqlTag);
              foreach (var t in n.Tags) {
                sqlTag.Value = t.Id;
                cmd.ExecuteNonQuery();
              }
            }
          }
        }
        tr.Commit();
      }
      tree.SelectedObjects = from;
      //tree.Sort();
      note.drawTags(); //redraw tagbox
    }

    //rearrange tags
    private void moveTag(IList from, TagItem to, int offset = 0) {
      foreach (var t in from) tags.Remove((TagItem)t);
      tags.InsertRange(tags.IndexOf(to) + offset, from.Cast<TagItem>());
      var i = 1;
      tags.ForEach(x => {
        if (!x.System) x.Index = i++;
      });
      tree.Roots = tags;
      tree.SelectedObjects = from;
      //save to db
      using (SQLiteTransaction tr = sql.BeginTransaction()) {
        using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
          SQLiteParameter index = new SQLiteParameter();
          SQLiteParameter id = new SQLiteParameter();
          cmd.CommandText = "UPDATE tags SET `index`=? WHERE id=?";
          cmd.Parameters.Add(index);
          cmd.Parameters.Add(id);
          foreach (var t in tags) if (!t.System) {
              index.Value = t.Index;
              id.Value = t.Id;
              cmd.ExecuteNonQuery();
            }
        }
        tr.Commit();
      }      
    }
    #endregion tree drag'n'drop

    #region tag box
    private void tagBox_Enter(object sender, EventArgs e) {
      note.FillAutocomplete();
    }

    private void tagBox_TextChanged(object sender, EventArgs e) {
      char[] delims = {' ',',',';'};
      if (tagBox.Text.Length>0 && Array.IndexOf(delims, tagBox.Text[tagBox.Text.Length - 1]) > 0) note.ParseTags();
    }

    private void tagBox_KeyDown(object sender, KeyEventArgs e) {
      switch (e.KeyCode) {
        case Keys.Back:
          if (tagBox.SelectionStart == 0) note.UnassignLastTag();
          break;
        case Keys.Enter:
        case Keys.Tab:
        case Keys.Space:
          note.ParseTags();
          break;
      }
    }
    #endregion tag box

    #region search bar
    //placeholder text hide
    private void cbSearch_Enter(object sender, EventArgs e) {
      if (tbSearch.ForeColor == SystemColors.GrayText) {
        tbSearch.Text = "";
        tbSearch.ForeColor = SystemColors.WindowText;
      }
    }
    //placeholder text show
    private void cbSearch_Leave(object sender, EventArgs e) {
      if (tbSearch.ForeColor == SystemColors.WindowText && tbSearch.Text.Length == 0) {
        tbSearch.ForeColor = SystemColors.GrayText;
        tbSearch.Text = tbSearch.AccessibleDescription;
      }
    }

    //incremental search
    private void cbSearch_TextChanged(object sender, EventArgs e) {
      if (tbSearch.ForeColor == SystemColors.WindowText) {
        if (tbSearch.Text.Length > 0) treeAsList(tbSearch.Text); // call for each change for incremental search
        else treeAsTags(); // switch to tags view
      }
    }

    // switch results on Up/Dn
    private void cbSearch_KeyDown(object sender, KeyEventArgs e) {
      var i = tree.SelectedIndex;
      switch (e.KeyCode) {
        case Keys.Down:
          if (i < tree.Items.Count - 1) tree.SelectedIndex = i + 1;
          tree.FocusedItem = tree.SelectedItem;
          break;
        case Keys.Up:
          if (i > 0) tree.SelectedIndex = i - 1;
          tree.FocusedItem = tree.SelectedItem;
          break;
        case Keys.Right:
          if (tree.RowHeight < 0 && tbSearch.Text.Length == 0) tree.Expand(tree.SelectedObject);
          break;
        case Keys.Left:
          if (tree.RowHeight < 0 && tbSearch.Text.Length == 0) {
            var node = tree.GetParent(tree.SelectedObject);
            if (node == null) tree.Collapse(tree.SelectedObject);
            else tree.SelectedObject = node;
            tree.FocusedItem = tree.SelectedItem;
          }
          break;
      }
    }
    #endregion search bar

    #region lexer menu
    //fill menu
    private void btnLexer_Click(object sender, MouseEventArgs e) {
      lexerMenu.Items.Clear();
      ToolStripItem i;
      foreach (var l in Glob.Lexers) {
        i = lexerMenu.Items.Add(l, null, lexClick);
        if (l == note.Item.Lexer) ((ToolStripMenuItem)i).Checked = true;
      }
      lexerMenu.Items.Add("-");
      i = lexerMenu.Items.Add(Glob.Inherit, null, lexClick);
      if (String.IsNullOrEmpty(note.Item.Lexer)) ((ToolStripMenuItem)i).Checked = true;
      lexerMenu.Show();
    }

    //positioning
    private void lexerMenu_Opening(object sender, CancelEventArgs e) {
      lexerMenu.Show(btnLexer.PointToScreen(new Point(btnLexer.Width, 0)), ToolStripDropDownDirection.AboveLeft);
    }

    // change lexer
    private void lexClick(object sender, EventArgs e) {
      var i = (ToolStripItem)sender;
      var lex = i.Text;
      using (SQLiteTransaction tr = sql.BeginTransaction()) {
        using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
          cmd.CommandText = "UPDATE notes SET lexer=?, modifydate=? WHERE id=" + note.Item.Id;
          if (i.Text == Glob.Inherit) {
            foreach (var tag in note.Item.Tags) if (!String.IsNullOrEmpty(tag.Lexer)) {
                lex = tag.Lexer;
                break;
            }
            btnLexer.Text = "^" + lex;
            cmd.Parameters.AddWithValue(null, null);
            note.Item.Lexer = null;
          }
          else {
            btnLexer.Text = lex;
            cmd.Parameters.AddWithValue(null, lex);
            note.Item.Lexer = lex;
          }
          note.Item.ModifyDate = (DateTime.UtcNow.Subtract(Glob.Epoch)).TotalSeconds;
          cmd.Parameters.AddWithValue(null, note.Item.ModifyDate);
          cmd.ExecuteNonQuery();
        }
        tr.Commit();
      }
      note.SetLanguage(lex);
    }
    #endregion lexer menu

    #region scintilla
    private void initScintilla() {
      //read theme file
      var file = ini.GetValue("Scintilla", "Theme", "Visual Studio.xml");
      readKeywords();
      readTheme(file);
      //folding
      scEdit.SetProperty("fold", "1");
      scEdit.SetProperty("fold.compact", "1");
      scEdit.SetProperty("fold.html", "1");
      scEdit.SetProperty("fold.quotes.python", "1");
      scEdit.Margins[2].Type = MarginType.Symbol;
      scEdit.Margins[2].Mask = Marker.MaskFolders;
      scEdit.Margins[2].Sensitive = true;
      scEdit.Margins[2].Width = 20;
      scEdit.SetFoldMarginColor(true, SystemColors.Window);
      //for (int i = 25; i <= 31; i++) {
      //  scEdit.Markers[i].SetForeColor(SystemColors.ControlLightLight);
      //  scEdit.Markers[i].SetBackColor(SystemColors.ControlDark);
      //}
      scEdit.Markers[Marker.Folder].Symbol = MarkerSymbol.Arrow;
      scEdit.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.ArrowDown;
      scEdit.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.Arrow;
      scEdit.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
      scEdit.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.ArrowDown;
      scEdit.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
      scEdit.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

      //smart highlight
      scEdit.IndicatorCurrent = 8;
      scEdit.Indicators[8].Style = IndicatorStyle.RoundBox;
      scEdit.Indicators[8].Alpha = 128;
      scEdit.Indicators[8].ForeColor = lexers["globals"].Find(x => x.id==29).bgcolor;
      scEdit.Indicators[8].Under = true;
      scEdit.SearchFlags = SearchFlags.None;
      //find mark
      scEdit.Indicators[1].Style = IndicatorStyle.RoundBox;
      scEdit.Indicators[1].Alpha = 128;
      scEdit.Indicators[1].ForeColor = lexers["globals"].Find(x => x.id == 31).bgcolor;
      //selection
      var s = lexers["globals"].Find(x => x.id == 0 && x.name == "Selected text colour");
      if(s!=null){
        scEdit.SetSelectionForeColor(true, s.fgcolor);
        scEdit.SetSelectionBackColor(true, s.bgcolor);
      }
      //highlight line
      s = lexers["globals"].Find(x => x.id == 0 && x.name == "Current line background colour");
      if (s != null) scEdit.CaretLineBackColor = s.bgcolor;
      else scEdit.CaretLineVisible = false;
    }

    //fills in keywords dic
    private void readKeywords() {
      var fn="themes\\langs.model.xml";
      if(!File.Exists(fn)) return;

      string _readTo = "";
      var s = new XmlReaderSettings();
      s.IgnoreComments = true;
      s.IgnoreWhitespace = true;
      try {
        var reader = XmlReader.Create(fn, s);
        reader.ReadStartElement();
        while (!reader.EOF) {
          //read lang 
          if (reader.Name.Equals("Language", StringComparison.OrdinalIgnoreCase) && !reader.IsEmptyElement && reader.HasAttributes) {
            _readTo = "";
            while (reader.MoveToNextAttribute()) {
              if (reader.Name.Equals("name", StringComparison.OrdinalIgnoreCase)) {
                if (Glob.Lexers.Contains(reader.Value, StringComparer.OrdinalIgnoreCase)) _readTo = reader.Value.ToLower();
                break;
              }
            }
            if (!String.IsNullOrEmpty(_readTo)) while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("Language", StringComparison.OrdinalIgnoreCase))) {
              if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("Keywords", StringComparison.OrdinalIgnoreCase) ) {
                if (!keywords.ContainsKey(_readTo)) keywords.Add(_readTo, new List<string>());
                keywords[_readTo].Add(reader.ReadElementContentAsString());
              }
              else reader.Read();
            }
          }
          reader.Read();
        }
        reader.Close();
      }
      catch (Exception e) {
        MessageBox.Show("Error reading keywords file: '" + fn + "'\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
    }

    //fills in lexers dic with styles
    private void readTheme(string file) {
      string _readTo = "";
      var s = new XmlReaderSettings();
      s.IgnoreComments = true;
      s.IgnoreWhitespace = true;
      try {
        var reader = XmlReader.Create("themes\\" + file, s);
        reader.ReadStartElement();
        while (!reader.EOF) {
          //read lang style
          if (reader.Name.Equals("LexerStyles", StringComparison.OrdinalIgnoreCase) && !reader.IsEmptyElement) {
            while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("LexerStyles", StringComparison.OrdinalIgnoreCase))) {
              reader.Read();
              if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("LexerType", StringComparison.OrdinalIgnoreCase) && reader.HasAttributes) {
                _readTo = "";
                while (reader.MoveToNextAttribute()) {
                  if (reader.Name.Equals("name", StringComparison.OrdinalIgnoreCase)){
                    if(Glob.Lexers.Contains(reader.Value, StringComparer.OrdinalIgnoreCase)) _readTo = reader.Value.ToLower();
                    break;
                  }
                }
                if (!String.IsNullOrEmpty(_readTo)) {
                  while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("LexerType", StringComparison.OrdinalIgnoreCase))) {
                    reader.Read();
                    if (reader.Name.Equals("WordsStyle", StringComparison.OrdinalIgnoreCase) && reader.HasAttributes) ReadStyle(_readTo, reader);
                  }
                }
                //else reader.Skip();                
              }
            }
            reader.Read();
          }
          //read global styles
          if (reader.Name.Equals("GlobalStyles", StringComparison.OrdinalIgnoreCase) && !reader.IsEmptyElement) {
            while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("GlobalStyles", StringComparison.OrdinalIgnoreCase))) {
              reader.Read();
              if (reader.Name.Equals("WidgetStyle", StringComparison.OrdinalIgnoreCase) && reader.HasAttributes) {
                ReadStyle("globals", reader);
              }
            }
          }
          reader.Skip();
        }
        reader.Close();
      }
      catch (Exception e) {
        MessageBox.Show("Error reading theme file: 'themes\\" + file + "'\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
    }

    //parse xml tag attributes to lexer style
    private void ReadStyle(string LexerName, XmlReader reader) {
      var sc = new scStyle();
      if (reader.HasAttributes) {
        while (reader.MoveToNextAttribute()) {
          switch (reader.Name.ToLower()) {
            case "name":
              sc.name = reader.Value;
              break;
            case "styleid":
              if (!String.IsNullOrEmpty(reader.Value)) sc.id = Int32.Parse(reader.Value);
              break;
            case "fgcolor":
              sc.fgcolor = (Color)new ColorConverter().ConvertFromString("#"+reader.Value);
              break;
            case "bgcolor":
              sc.bgcolor = (Color)new ColorConverter().ConvertFromString("#"+reader.Value);
              break;
            case "fontname":
              sc.fontname = reader.Value;
              break;
            case "fontsize":
              if (!String.IsNullOrEmpty(reader.Value)) sc.fontsize = Int32.Parse(reader.Value);
              break;
            case "fontstyle":
              if (!String.IsNullOrEmpty(reader.Value)){
                var i = Int32.Parse(reader.Value);
                sc.bold = (i & 1) == 1;
                sc.italic = (i & 2) == 2;
                sc.underline = (i & 4) == 4;
              }
              break;
          }
        }
        reader.MoveToElement();
        if (!lexers.ContainsKey(LexerName)) lexers.Add(LexerName, new List<scStyle>());
        lexers[LexerName].Add(sc);
      }
    }

    //smart select
    private void scEdit_UpdateUI(object sender, UpdateUIEventArgs e) {
      //selection changed
      if ((e.Change & UpdateChange.Selection) > 0) { 
        //cleanup all highlihts if any
        if (scEdit.Indicators[8].End(0) != scEdit.TextLength) {
          scEdit.IndicatorClearRange(0, scEdit.TextLength);
        }

        string ss = scEdit.SelectedText;
        if (ss.Length > 3 && ss.IndexOfAny(new char[] { ' ', '(', ')' }) == -1) {
          scEdit.TargetStart = 0;
          scEdit.TargetEnd = scEdit.TextLength;
          while (scEdit.SearchInTarget(ss) != -1) {
            if (scEdit.TargetStart != scEdit.SelectionStart) scEdit.IndicatorFillRange(scEdit.TargetStart, scEdit.TargetEnd - scEdit.TargetStart);
            scEdit.TargetStart = scEdit.TargetEnd;
            scEdit.TargetEnd = scEdit.TextLength;
          }
        }
      }
    }

    //trigger autosave on change
    private void scEdit_SavePointLeft(object sender, EventArgs e) {
      if (saveTimer == null) {
        saveTimer = new Timer();
        saveTimer.Interval = 5000;
        saveTimer.Tick += saveTimer_Elapsed;
      }
      if (statusText.Text != Glob.Changed) statusText.Text = Glob.Changed;
      saveTimer.Stop();
      saveTimer.Start();    
    }

    //autosave
    void saveTimer_Elapsed(object sender, EventArgs e) {
      statusText.Text = Glob.Saved;
      note.Save();
    }
    #endregion scintilla

    #region sync
    private void btnSync_MouseDown(object sender, MouseEventArgs e) {
      // show sync account setup on right click
      if (e.Button == System.Windows.Forms.MouseButtons.Right) showFormSyncAcc();
      // trigger sync
      else {
        // check if account configured
        if (String.IsNullOrEmpty(Sync.Email) || String.IsNullOrEmpty(Sync.Password)) {
          showFormSyncAcc();
          return;
        }
        // do sync
        SnSync(0);
      }      
    }

    //configure simplenote acc
    private void showFormSyncAcc() {
      Form frmSync = new FormSync();
      if (frmSync.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        using (SQLiteTransaction tr = sql.BeginTransaction()) {
          using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
            cmd.CommandText = "INSERT OR REPLACE INTO config(name,value) VALUES('email', ?)";
            cmd.Parameters.AddWithValue(null, Sync.Email);
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            cmd.CommandText = "INSERT OR REPLACE INTO config(name,value) VALUES('password', ?)";
            cmd.Parameters.AddWithValue(null, Sync.Password);
            cmd.ExecuteNonQuery();
            cmd.CommandText = "INSERT OR REPLACE INTO config(name,value) VALUES('freq'," + Sync.Freq + ")";
            cmd.ExecuteNonQuery();
          }
          tr.Commit();
        }
        syncTimer.Stop();

        // validate user
        if (!String.IsNullOrEmpty(Sync.Email)) {
          statusText.Text = "Validating login/pass...";
          Task.Factory.StartNew<bool>(() => {
            return Sync.checkLogin();
          }).ContinueWith(t => {
            if (t.Result) {
              statusText.Text = Sync.Email;
              if (Sync.Freq > 0) {
                syncTimer.Interval = Sync.Freq * 60 * 1000;
                syncTimer.Start();
              }
            }
            else statusText.Text = "Wrong login/pass";
          });
        }
      
      }
      frmSync.Dispose();
    }

    // aut-sync timer event
    private void timer_sync(object sender, EventArgs e) {
      SnSync(Sync.LastSync);
    }

    /// <summary>
    /// do Simplenote sync 
    /// </summary>
    private void SnSync(double since) {
      statusText.Text = "Syncing...";
      // detach from ui thread
      Task.Factory.StartNew<string>(() => {
                
        try {

          // sync tags
          var tagindex = Sync.getTags();
          Parallel.ForEach(tags.FindAll(x => !x.System), (i) => {
            //foreach(var i in tags.FindAll(x => !x.System)){
            var meta = tagindex.Find(x => x.name == i.Name); //case insensitive?
            //delete local
            if (meta == null && i.Version > 0) deleteTag(i, false);
            //create or update remote
            else if (meta == null || (meta.index != i.Index && i.Version >= meta.version)) {
              var raw = Sync.pushTag(i);
              UpdateTag(i, raw, ui);
            }
            // update local
            else if (i.Version < meta.version)
              UpdateTag(i, meta, ui);
          });
          
          // upload modified notes from last sync
          if (since > 0) { // only for incremental, as in full-sync they sync in index compare later
            Parallel.ForEach( notes.FindAll(x => x.ModifyDate > since && !String.IsNullOrEmpty(x.Key)), (i) => {
              var raw = Sync.pushNote(i, sql);
              UpdateNote(i, raw, ui);
            });
          }  
          // upload newly created, no key id yet
          Parallel.ForEach( notes.FindAll(x => String.IsNullOrEmpty(x.Key)), (i) => {
            var raw = Sync.pushNote(i, sql);
            UpdateNote(i, raw, ui);
          });

          // search for new/updated notes and update db in async mode
          var num = 0;
          var index = Sync.getIndex(since);
          Sync.LastSync = (DateTime.UtcNow.Subtract(Glob.Epoch)).TotalSeconds;
          Parallel.ForEach(index, (i) => {
            var note = notes.Find(x => x.Key == i.key);
            var t = double.Parse(i.modifydate, System.Globalization.CultureInfo.InvariantCulture); // dot as decimal separator
            if (note != null && Math.Round(note.ModifyDate,1) > Math.Round(t,1)) { // local newer
              num++;
              var raw = Sync.pushNote(note, sql);
              UpdateNote(note, raw, ui);
            }
            else if (note == null || note.SyncNum < i.syncnum) { // remote newer
              num++;
              var raw = Sync.getNote(i.key);
              UpdateNote(note, raw, ui);
            }
          });

          // purge notes with key id not in remote index 
          if (since == 0) { //only in full-sync
            Parallel.ForEach(notes, (i) => {
              if (string.IsNullOrEmpty(i.Key)) return;
              if (!index.Exists(x => x.key == i.Key)) {
                Task t1 = new Task(() => { purgeNote(i, false); });
                t1.Start(ui);
              }
            });
          }

          var dt = Glob.Epoch.AddSeconds(Sync.LastSync).ToLocalTime();
          var stat = (num > 0) ? " (" + num + " items)" : "";
          return String.Format("Last Sync: {0}{1}", dt.ToString("HH:mm"), stat);

        }
        catch (AggregateException e) {
          return e.Message; // e.InnerExceptions[0].Message;
        }

      }).ContinueWith( (t) => { // sync with ui
        statusText.Text = t.Result;        
      }, ui);
    }

    // update tag order/ver
    private void UpdateTag(TagItem tag, TagMeta raw, TaskScheduler ui) {
      tag.Index = raw.index;
      tag.Version = raw.version;
      //save to db
      var conn = new SQLiteConnection(ConnString);
      conn.Open();
      using (SQLiteTransaction tr = conn.BeginTransaction()) {
        using (SQLiteCommand cmd = new SQLiteCommand(conn)) {
          cmd.CommandText = "UPDATE tags SET `index`=?, version=? WHERE id=?"; // don't update name
          cmd.Parameters.AddWithValue(null, tag.Index);
          cmd.Parameters.AddWithValue(null, tag.Version);
          cmd.Parameters.AddWithValue(null, tag.Id);
          cmd.ExecuteNonQuery();
        }
        tr.Commit();
      }
      conn.Close();

      // sync with ui
      Task t1 = new Task(() => {        
        tree.RefreshObject(tag);
        tree.Sort();
      });
      t1.Start(ui);
    }

    /// <summary>
    /// Update note in DB with sync result
    /// </summary>
    /// <param name="n">Note to update, could be null = create new</param>
    /// <param name="raw">Data from sync to update with</param>
    /// <param name="ui">Scheduler for UI thread to display changes</param>
    private void UpdateNote(NoteItem n, NoteData raw, TaskScheduler ui) {
      bool update = true;
      if (n == null) {
        n = new NoteItem();
        update = false;
      }

      n.Deleted = raw.deleted == 1;
      n.Key = raw.key;
      n.ModifyDate = double.Parse(raw.modifydate, System.Globalization.CultureInfo.InvariantCulture);
      if (raw.content!=null)  n.Name = note.GetTitle(raw.content);
      n.SyncNum = raw.syncnum;

      // read system tags
      string systemtags=""; //unparsed tags to save
      foreach (var tag in raw.systemtags) {
        if (tag == "pinned") n.Pinned = true;
        else if (tag == "unread") n.Unread = true;
        else if (tag.StartsWith("sn-lexer=")) {
          var lexer = tag.Substring(9);
          if (Glob.Lexers.Contains(lexer)) n.Lexer = lexer;
        }
        else systemtags += tag + " ";
      }

      // create new connection as it is another thread
      var conn = new SQLiteConnection(ConnString);
      conn.Open();
      using (SQLiteTransaction tr = conn.BeginTransaction()) {
        using (SQLiteCommand cmd = new SQLiteCommand(conn)) {

          if (raw.content == null) { // no content after push note and no merges
            cmd.CommandText = (update) ?
              "UPDATE notes SET key=?, deleted=?, modifydate=?, createdate=?, syncnum=?, version=?, systemtags=?, pinned=?, unread=?, lexer=? WHERE id=?" :
              "INSERT INTO notes(key,  deleted,   modifydate,   createdate,   syncnum,   version,   systemtags,   pinned,   unread,   lexer)  VALUES(?,?,?,?,?,?,?,?,?,?)";
          }
          else {                    // update content when push & merge, or just a new remote note
            cmd.CommandText = (update) ?
              "UPDATE notes SET key=?, deleted=?, modifydate=?, createdate=?, syncnum=?, version=?, systemtags=?, pinned=?, unread=?, title=?, content=?, lexer=? WHERE id=?" :
              "INSERT INTO notes(key,  deleted,   modifydate,   createdate,   syncnum,   version,   systemtags,   pinned,   unread,   title,   content,   lexer)  VALUES(?,?,?,?,?,?,?,?,?,?,?,?)";
          }
          cmd.Parameters.AddWithValue(null, n.Key);
          cmd.Parameters.AddWithValue(null, n.Deleted);
          cmd.Parameters.AddWithValue(null, n.ModifyDate);
          cmd.Parameters.AddWithValue(null, double.Parse(raw.createdate, System.Globalization.CultureInfo.InvariantCulture));
          cmd.Parameters.AddWithValue(null, n.SyncNum);
          cmd.Parameters.AddWithValue(null, raw.version);
          cmd.Parameters.AddWithValue(null, systemtags.Trim());
          cmd.Parameters.AddWithValue(null, n.Pinned);
          cmd.Parameters.AddWithValue(null, n.Unread);
          if (raw.content != null) {
            cmd.Parameters.AddWithValue(null, n.Name);
            cmd.Parameters.AddWithValue(null, raw.content);
          }
          cmd.Parameters.AddWithValue(null, n.Lexer);
          if(update) cmd.Parameters.AddWithValue(null, n.Id);
          cmd.ExecuteNonQuery();
          if (!update) n.Id = conn.LastInsertRowId;
        }
        tr.Commit();
      }
      conn.Close();
      
      // sync with ui
      Task t1 = new Task(() => {
        if (!update) notes.Add(n);
        if (raw.tags.Length > 0 || n.Tags.Count > 0) {
          var tmp = tree.SelectedObject;
          var tags = string.Join(" ", raw.tags);
          note.ParseTags(tags, n); // read tags (and create if not exist)
          if (tmp != null && tree.SelectedObject != tmp) tree.SelectedObject = tmp; //restore selected position
        }
        tree.RefreshObject(tagAll);
        tree.RefreshObject(tagDeleted);
      });
      t1.Start(ui);
    }


    #endregion sync



























  }

  #region tree owner draw
  public class FancyRenderer : BaseRenderer {
    public override void Render(Graphics g, Rectangle r) {
      //get objects
      TagItem tag = null;
      NoteItem note = null;
      bool isTag = false;
      if (this.RowObject is TagItem) {
        tag = (TagItem)this.RowObject;
        isTag = true;
      }
      else note = (NoteItem)this.RowObject;

      this.DrawBackground(g, r);
      if (isTag) drawTag(g, r, tag);
      else if (this.ListView.RowHeight > 0) drawFound(g, r, note);
      else drawNote(g, r, note);
    }

    //draw found result
    private void drawFound(Graphics g, Rectangle r, NoteItem note) {
      //note title
      r.X += 3; //padding
      r.Y += 3;
      var ctag = this.GetForegroundColor();
      var cnote = Color.FromArgb(100, 100, 100);
      if (ctag == SystemColors.HighlightText) {
        cnote = ControlPaint.Dark(ctag, 90);
      }
      else ctag = SystemColors.HotTrack;

      var stringSize = g.MeasureString(note.DateShort, this.Font);
      var offset = (int)stringSize.Height + 1;
      var r2 = new Rectangle(r.X + 3, offset + 5, r.Width, r.Height - offset); //rect search result
      using (StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap)) {
        fmt.LineAlignment = StringAlignment.Near;
        fmt.Trimming = StringTrimming.EllipsisCharacter;

        //modify date        
        using (SolidBrush b = new SolidBrush(Color.FromArgb(128, this.GetForegroundColor()))) {
          fmt.Alignment = StringAlignment.Far;
          g.DrawString(note.DateShort, this.Font, b, r, fmt);
        }

        //note title
        fmt.Alignment = StringAlignment.Near;
        r.Width -= (int)stringSize.Width - 1;
        using (var f = new Font(this.Font, FontStyle.Bold))
        using (var b = new SolidBrush(ctag)) {
          g.DrawString(this.GetText(), f, b, r, fmt);
        }
        
        //pinned
        if (note.Pinned) {
          r.Height = offset;
          stringSize = g.MeasureString(this.GetText(), this.Font);
          offset = (int)stringSize.Width;
          r.X += offset + 3;
          r.Y -= 3;
          if (this.IsItemSelected) this.DrawImage(g, r, 7); //inverted
          else this.DrawImage(g, r, 6);
        }
      }

      //search result to rtf
      var s = note.Snippet.Replace("\n", " ").Replace("\r", " ");
      while (s.Contains("  ")) s = s.Replace("  ", " ");
      s = s.Replace(@"\", @"\\")
           .Replace("{", @"\{")
           .Replace("}", @"\}")
           .Replace(@"<b>", @"\b ")
           .Replace(@"</b>", @"\b0 ");
      g.DrawRtfText(@"{\rtf\b0 " + s + " }", r2, cnote, this.Font);
    }

    //draw tag with collapse/expand icon and badge
    private void drawTag(Graphics g, Rectangle r, TagItem tag) {
      //collapse icon of tag
      ImageList il = this.ListView.SmallImageList;
      TreeListView tree = (TreeListView)this.ListView;
      TreeListView.Branch br = tree.TreeModel.GetBranch(this.RowObject);
      if (br.IsExpanded) {//opened
        if (this.IsItemSelected) this.DrawImage(g, r, 8);
        else this.DrawImage(g, r, 2);
      }
      else {//closed
        if (this.IsItemSelected) this.DrawImage(g, r, 9);
        else this.DrawImage(g, r, 3);
      }
      r.X += 16;
      r.Width -= 16;

      //system tag icon        
      if (tag.System) {
        if (tag.Name == Glob.All) this.DrawImage(g, r, 4);
        else this.DrawImage(g, r, 5);
        r.X += 16;
        r.Width -= 16;
      }

      //tag title
      using (StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap)) {
        fmt.LineAlignment = StringAlignment.Center;
        fmt.Trimming = StringTrimming.EllipsisCharacter;
        fmt.Alignment = StringAlignment.Near;
        SizeF stringSize = new SizeF();
        using (var f = new Font(this.Font, FontStyle.Bold)) {
          g.DrawString(this.GetText(), f, this.TextBrush, r, fmt);
          stringSize = g.MeasureString(this.GetText(), f);
        }
        var pad = (int)stringSize.Width + 3;
        r.X += pad;
        r.Width -= pad;

        //count badge
        using (var f = new Font(this.Font.FontFamily, this.Font.Size - 1, FontStyle.Regular)) {
          stringSize = g.MeasureString(tag.Count.ToString(), f);
          var badgerect = new Rectangle(r.X + 3, r.Y + 3, (int)stringSize.Width + 3, r.Height - 6);
          using (GraphicsPath path = this.GetRoundedRect(badgerect, 10)) {
            using (SolidBrush b = new SolidBrush(Color.FromArgb(80, Color.Black))) {
              g.FillPath(b, path); //bg
            }
            using (SolidBrush b = new SolidBrush(Color.White)) {
              fmt.Alignment = StringAlignment.Center;
              g.DrawString(tag.Count.ToString(), f, b, badgerect, fmt); //count
            }
          }
        }
      }
    }

    //draw note title with modify date
    private void drawNote(Graphics g, Rectangle r, NoteItem note) {
      //note indent
      r.X += 32;
      r.Width -= 32;
      //note title
      using (StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap)) {
        fmt.LineAlignment = StringAlignment.Center;
        fmt.Trimming = StringTrimming.EllipsisCharacter;

        //modify date
        var stringSize = g.MeasureString(note.DateShort, this.Font);
        var offset = (int)stringSize.Width + 1;
        r.Width -= offset;
        var daterect = new Rectangle(r.X + r.Width, r.Y, offset, r.Height);
        using (SolidBrush b = new SolidBrush(Color.FromArgb(128, this.GetForegroundColor()))) {
          fmt.Alignment = StringAlignment.Far;
          g.DrawString(note.DateShort, this.Font, b, daterect, fmt);
        }

        //note title
        fmt.Alignment = StringAlignment.Near;
        g.DrawString(this.GetText(), this.Font, this.TextBrush, r, fmt);
      }
      //pinned
      if (note.Pinned) {
        var stringSize = g.MeasureString(this.GetText(), this.Font);
        var offset = (int)stringSize.Width;
        r.X += offset;
        r.Width -= offset;
        if (this.IsItemSelected) this.DrawImage(g, r, 7); //inverted
        else this.DrawImage(g, r, 6);
      }
    }

    /// <summary>
    /// Return a GraphicPath that is round corner rectangle.
    /// </summary>
    protected GraphicsPath GetRoundedRect(Rectangle rect, float diameter) {
      GraphicsPath path = new GraphicsPath();

      RectangleF arc = new RectangleF(rect.X, rect.Y, diameter, diameter);
      path.AddArc(arc, 180, 90);
      arc.X = rect.Right - diameter;
      path.AddArc(arc, 270, 90);
      arc.Y = rect.Bottom - diameter;
      path.AddArc(arc, 0, 90);
      arc.X = rect.Left;
      path.AddArc(arc, 90, 90);
      path.CloseFigure();

      return path;
    }
  }
  #endregion tree owner draw 

  internal class scStyle {
    public string name { get; set; }
    public int id { get; set; }
    public Color fgcolor { get; set; }
    public Color bgcolor { get; set; }
    public string fontname { get; set; }
    public float fontsize { get; set; }
    public bool bold { get; set; }
    public bool italic { get; set; }
    public bool underline { get; set; }
  }

  /// <summary>
  /// send mouse wheel event to control under cursor
  /// </summary>
  internal class MouseWheelMessageFilter : IMessageFilter {
    public bool PreFilterMessage(ref Message m) {
      if (m.Msg == NativeMethods.WM_MOUSEWHEEL) {
        // LParam contains the location of the mouse pointer
        Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
        IntPtr hWnd = NativeMethods.WindowFromPoint(pos);
        if (hWnd != IntPtr.Zero && hWnd != m.HWnd && Control.FromHandle(hWnd) != null) {
          // redirect the message to the correct control
          NativeMethods.SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
          return true;
        }
      }
      return false;
    }
  }

  // WinAPI
  internal static class NativeMethods {
    internal const uint SW_RESTORE = 0x09;
    internal const int WM_MOUSEWHEEL = 0x20a;

    [DllImport("user32.dll")]
    internal static extern int ShowWindow(IntPtr hWnd, uint Msg);
    [DllImport("user32.dll")]
    internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    //[DllImport("user32.dll")]
    //internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    [DllImport("user32.dll")]
    internal static extern IntPtr WindowFromPoint(Point pt);
    [DllImport("user32.dll")]
    internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
  }

  // globals
  public static class Glob {
    public const string All = "All";
    public const string Deleted = "Deleted";
    public static string[] Lexers = { "Asm", "Asp", "Bash", "Batch", "Cpp", "Css", "Diff", "Hypertext", "Lua", "Pascal", "Perl", "Powershell", "Props", "Python", "Ruby", "Sql", "Tcl", "VB", "VBScript", "Xml", "Yaml", "Null" };
    public const string Inherit = "Inherit";
    public const string Changed = "changed";
    public const string Saved = "saved";
    public const string Unsaved = "error of saving";
    public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); //unixtime start
  }
}
