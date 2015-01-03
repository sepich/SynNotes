using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Data.SQLite;
using BrightIdeasSoftware;

namespace SynNotes {
  
  public partial class Form1 : Form {
    
    // vars
    IniFile ini;
    string conffile = "settings.ini";
    string dbfile = "notes.db";
    string dbver = "1";
    string userdir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SynNotes\\";
    KeyHook hook = new KeyHook();
    SQLiteConnection sql;
    List<TagItem> roots;

    // WinAPI
    [DllImport("user32.dll")] private static extern int ShowWindow(IntPtr hWnd, uint Msg);
    private const uint SW_RESTORE = 0x09;

    public Form1() {InitializeComponent();}

    // parse string to hotkey
    private void setHotkey(int id, string keys) {
      if (keys.Length > 0) {
        try {
          var a = keys.Split('+');
          //get keycode
          uint k;
          if (a.Last()[0] == '`') k = (uint)Keys.Oemtilde.GetHashCode();
          else k = (uint)a.Last()[0];
          //get modifiers
          var m = new ModifierKey();
          for (var i = 0; i < a.Length - 1; i++) {
            m = m | (ModifierKey)Enum.Parse(typeof(ModifierKey), a[i].Trim());
          }
          //set key hook
          if (!hook.RegisterHotKey(id, m, k)) statusText.Text = "Error registering HotKey: " + keys;
        }
        catch {
          statusText.Text = "Cannot parse '" + keys + "' as keys sequence";
        }
      }
    }

    private void Form1_Load(object sender, EventArgs e) {
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
      //System.Threading.Thread.Sleep(5000);
      // hotkeys
      hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
      setHotkey(1, ini.GetValue("Keys", "HotkeyShow", ""));
      string s = ini.GetValue("Keys", "HotkeySearch", "Win+`");
      setHotkey(2, s);
      cbSearch.AccessibleDescription = "Search Notes (" + s + ")";
      //check db
      if (File.Exists(dbfile)) sqlConnect(dbfile);
      else if (File.Exists(userdir + dbfile)) sqlConnect(userdir + dbfile);
      else {
        sqlConnect(dbfile, false);
        if(sql==null) sqlConnect(userdir + dbfile);
        sqlCreate();
      }
      //init tree
      initTree();
      //init tagBox
      tagBox.Tag = new List<string>();
    }

    //create db schema
    void sqlCreate() {
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
        MessageBox.Show("Unable to provision db: " + sql.DataSource + "\n" + e.Message);
        this.Close();
      }
    }

    //try to connect to provided db, create if not exist
    void sqlConnect(string db, bool warn=true) {
      try {
        SQLiteConnectionStringBuilder connString = new SQLiteConnectionStringBuilder();
        connString.DataSource = db;
        connString.FailIfMissing = false;
        connString.ForeignKeys = true;
        sql = new SQLiteConnection(connString.ToString());
        sql.Open();
        //check if db valid to throw ex
        using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM config WHERE name='ver'", sql)) {
          if(Convert.ToString(cmd.ExecuteScalar()) != dbver){
            //update db schema
          }
        }
      }
      catch(Exception e) {
        if (warn) {
          MessageBox.Show("Unable to use db: " + db + "\n" + e.Message);
          this.Close();
        }
      }
    }

    void hook_KeyPressed(object sender, KeyPressedEventArgs e) {
      // Show hotkey  
      if (this.WindowState == FormWindowState.Minimized) ShowWindow(this.Handle, SW_RESTORE);
      else {
        this.Activate();
        this.BringToFront();
      }
      //Search hotkey
      if (e.id == 2) this.cbSearch.Focus();
      //statusText.Text = e.Modifier.ToString().Replace(", ", "+") + "+" + e.Key.ToString();
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
      if (scEdit.Modified && scEdit.Tag != null) saveNote();
      //close db connection
      if (sql != null) sql.Dispose();
    }

    private void notifyIcon1_Click(object sender, EventArgs e) {
      MouseEventArgs me = (MouseEventArgs)e;
      if (me.Button == MouseButtons.Left) ShowWindow(this.Handle, SW_RESTORE);
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

    private void openToolStripMenuItem1_Click(object sender, EventArgs e) {
      ShowWindow(this.Handle, SW_RESTORE);
    }

    private void Form1_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Escape && e.Modifiers == Keys.None) this.WindowState = FormWindowState.Minimized;
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
      this.Close();
    }

    //placeholder text hide
    private void cbSearch_Enter(object sender, EventArgs e) {      
      if (cbSearch.Tag.ToString() == "hint") {
        cbSearch.Tag = null;
        cbSearch.ForeColor = SystemColors.WindowText;
        cbSearch.Text = "";
      }
    }

    //placeholder text show
    private void cbSearch_Leave(object sender, EventArgs e) {      
      if (cbSearch.Tag == null && cbSearch.Text.Length==0) {
        cbSearch.Tag = "hint";
        cbSearch.ForeColor = SystemColors.GrayText;
        cbSearch.Text = cbSearch.AccessibleDescription;
      }
    }

    //init tree with root items from db
    private void initTree() {
      //All as system
      var s = "SELECT 0 as id, 'All' AS name, count(id) AS num FROM notes WHERE deleted=0";
      roots = getTags(s, true);
      //read all tags info
      s = "SELECT t.id, t.name, COUNT(c.note) AS num FROM tags t LEFT JOIN nt c ON t.id=c.tag GROUP BY t.name ORDER BY t.`index`";
      roots.AddRange(getTags(s));
      //Deleted as system
      s = "SELECT 0 as id, 'Deleted' AS name, count(id) AS num FROM notes WHERE deleted=1";
      roots.AddRange(getTags(s, true));

      //getters
      tree.Roots = roots;
      cDate.AspectGetter = delegate(object x) {
        if (x is TagItem) return ((TagItem)x).count;
        else return ((NoteItem)x).modifyDateS;
      };
      tree.CanExpandGetter = delegate(object x) {
        return (x is TagItem && ((TagItem)x).count > 0);
      };
      tree.ChildrenGetter = delegate(object x) { return getNotes(x); };
      
      //renderer
      //this.tree.TreeColumnRenderer.LinePen = new Pen(Color.Transparent);
    }

    //get sql query [id, name, num], return list of tag tree nodes
    private List<TagItem> getTags(string query, bool sys=false) {
      List<TagItem> res = new List<TagItem>();
      TagItem node;
      using (SQLiteCommand cmd = new SQLiteCommand(query, sql)) {
        using (SQLiteDataReader rdr = cmd.ExecuteReader()) {
          while (rdr.Read()) {
            node = new TagItem();
            node.id = Convert.ToInt32(rdr["id"]);
            node.name = rdr["name"].ToString();
            node.count = Convert.ToInt32(rdr["num"]);
            node.isSystem = sys;
            res.Add(node);
          }
        }
      }
      return res;
    }

    //get notes when tag is expanded
    private List<NoteItem> getNotes(object x) {
      TagItem tag = (TagItem)x;
      if (tag.notes.Count > 0) tag.notes.Clear();
      NoteItem node;
      string s;
      if (tag.isSystem) {
        if (tag.name == "All") s = "SELECT id, title, modifydate FROM notes WHERE deleted=0";  // All
        else s = "SELECT id, title, modifydate FROM notes WHERE deleted=1";                    // Deleted
      }
      else s = "SELECT n.id, n.title, n.modifydate"+                                           // per-tag
        " FROM notes n LEFT JOIN nt c ON c.note=n.id" +
        " WHERE n.deleted=0 AND c.tag=" + tag.id.ToString() + 
        " ORDER BY pinned DESC, title ASC";
      using (SQLiteCommand cmd = new SQLiteCommand(s, sql)) {
        using (SQLiteDataReader rdr = cmd.ExecuteReader()) {
          while (rdr.Read()) {
            node = new NoteItem();
            node.id = rdr.GetInt32(0);
            node.name = rdr.GetString(1);
            node.modifyDate = rdr.GetFloat(2);
            tag.notes.Add(node);
          }
        }
      }
      return tag.notes;
    }

    //add new note
    private void btnAdd_ButtonClick(object sender, EventArgs e) {
      // get parent tag if something selected
      TagItem tag=null;
      if (tree.SelectedObject!=null) {
        if (tree.SelectedObject is TagItem) tag = (TagItem)tree.SelectedObject;
        else tag = (TagItem)tree.GetParent(tree.SelectedObject);
        if (tag.isSystem && tag.name == "Deleted") tag = roots[0]; //don't create new deleted notes
      }
      else {
        tag = roots[0];
      }
      //add new note to db
      var ut = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString().Replace(',','.');
      using (SQLiteTransaction tr = sql.BeginTransaction()) {
        using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
          cmd.CommandText = "INSERT INTO notes(modifydate, createdate, title, content) "+
            "VALUES("+ut+", "+ut+", 'New Note', 'New Note')";
          cmd.ExecuteNonQuery();
        }
        tr.Commit();
      }
      tag.count += 1;
      tree.RefreshObject(tag);
      tree.Expand(tag);
      tree.Reveal(tag.notes.Find(x => x.id == sql.LastInsertRowId), true);
    }

    private void tree_SelectionChanged(object sender, EventArgs e) {
      if (scEdit.Modified && scEdit.Tag != null) saveNote();
      if (tree.SelectedItem != null && tree.SelectedObject is NoteItem) viewNote();
    }

    //load note for selected item
    private void viewNote() {
      NoteItem note = (NoteItem)tree.SelectedObject;
      using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
        cmd.CommandText = "SELECT content, lexer, topline FROM notes WHERE id=" + note.id;
        using (SQLiteDataReader rdr = cmd.ExecuteReader()) {
          while (rdr.Read()) {
            scEdit.Text = rdr.GetString(0);
            scEdit.ConfigurationManager.Language = rdr.IsDBNull(1) ? "Bash" : rdr.GetString(1);
            //TODO set top line
          }
        }
      }
      //checked when saving
      scEdit.Modified = false;
      scEdit.Tag = note;
      // window title and tags
      this.Text = getTitle();
      List<string> tags = (List<string>)tagBox.Tag;
      tags.Clear();
      using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
        cmd.CommandText = "SELECT t.name FROM tags t LEFT JOIN nt c ON t.id=c.tag WHERE c.note=" + note.id;
        using (SQLiteDataReader rdr = cmd.ExecuteReader()) {
          while (rdr.Read()) {
            tags.Add(rdr.GetString(0));
          }
        }
      }
      parseTags(true);
    }

    //save opened note
    private void saveNote() {
      NoteItem note = (NoteItem)scEdit.Tag;
      var ut = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
      var title = getTitle();
      //save text
      using (SQLiteTransaction tr = sql.BeginTransaction()) {
        using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
          cmd.CommandText = "UPDATE notes SET modifydate=?, title=?, content=? WHERE id=?";
          cmd.Parameters.AddWithValue(null, ut);
          cmd.Parameters.AddWithValue(null, title);
          cmd.Parameters.AddWithValue(null, scEdit.Text);
          cmd.Parameters.AddWithValue(null, note.id);
          cmd.ExecuteNonQuery();
        }
        tr.Commit();
      }
      //save tags
      using (SQLiteTransaction tr = sql.BeginTransaction()) {
        using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
          cmd.CommandText = "DELETE FROM nt WHERE note=?";
          cmd.Parameters.AddWithValue(null, note.id);
          cmd.ExecuteNonQuery();

          cmd.CommandText = "INSERT INTO nt(note,tag) VALUES(?,?)";
          cmd.Parameters.AddWithValue(null, note.id);
          SQLiteParameter t = new SQLiteParameter();
          cmd.Parameters.Add(t);
          TagItem tmp;
          foreach (var tag in (List<string>)tagBox.Tag) {
            tmp = roots.Find(x => x.name == tag);
            if (tmp == null || tmp.id == 0) continue;
            t.Value = tmp.id;
            cmd.ExecuteNonQuery();
          }
        }
        tr.Commit();
      }      

      //refresh tree
      scEdit.Modified = false;
      note.name = title;
      tree.RefreshObject(note);
    }

    //get title from active scintilla text
    private string getTitle() {
      string title;
      if (scEdit.Text.Length == 0) return "(blank)";
      var len1 = scEdit.Text.Length > 100 ? 100 : scEdit.Text.Length;
      title = scEdit.Text.Substring(0, len1).Trim();
      var len2 = title.IndexOfAny(new char[] { '\n', '\r' });
      if(len2 > 0) return title.Substring(0, len2);
      else if(len1<100) return title;
      else return title + "...";
    }

    #region tag box

    private void tagBox_Enter(object sender, EventArgs e) {
      fillTagBoxAC();
    }

    //read tag list to fill autocomplete
    private void fillTagBoxAC() {
      if (roots.Count > 0) {
        List<string> tags = (List<string>)tagBox.Tag;
        tagBox.AutoCompleteCustomSource.Clear();
        foreach (var tag in roots) {
          //TODO if (tag.isSystem) continue;
          if (tags.Contains(tag.name, StringComparer.OrdinalIgnoreCase)) continue;
          tagBox.AutoCompleteCustomSource.Add(tag.name);
        }
      }
    }

    private void tagBox_TextChanged(object sender, EventArgs e) {
      char[] delims = {' ',',',';'};
      if (tagBox.Text.Length>0 && Array.IndexOf(delims, tagBox.Text[tagBox.Text.Length - 1]) > 0) parseTags();
    }

    private void tagBox_KeyDown(object sender, KeyEventArgs e) {
      switch (e.KeyCode) {
        case Keys.Escape:
          cbSearch.Focus();
          break;
        case Keys.Back:
          if (tagBox.SelectionStart == 0) {
            List<string> tags = (List<string>)tagBox.Tag;
            if (tags.Count > 0) {
              tags.RemoveAt(tags.Count - 1);
              scEdit.Modified = true; // so changes in tags would be saved
              parseTags(true);
            }
          }
          break;
        case Keys.Enter:
        case Keys.Tab:
        case Keys.Space:
          parseTags();
          break;
      }
    }

    //parse textbox for tags
    private void parseTags(bool repaint=false) {
      List<string> tags = (List<string>)tagBox.Tag;
      string[] list;
      // repaint is called when it's need to recreate tags box view from list
      if (repaint) {
        list = tags.ToArray();
        foreach (Control l in splitContainer1.Panel2.Controls) 
          if (l.Tag == "tag") {
            splitContainer1.Panel2.Controls.Remove(l);
            l.Dispose();
        }
        tagBox.Left = 37;
      }
      //else parse text input
      else {
        list = tagBox.Text.Split(new char[] { ' ', ',', ';' });
        scEdit.Modified = true; // so changes in tags would be saved
      }

      foreach (var tag in list) {
        if (tag.Length == 0) continue;
        if (!repaint) {
          if (tags.Contains(tag, StringComparer.OrdinalIgnoreCase)) continue;
          tags.Add(tag);
        }

        // create label element
        Label l = new Label();
        l.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        l.Location = tagBox.Location;
        l.Font = tagBox.Font;
        l.BackColor = Color.Gainsboro;
        l.Tag="tag"; // mark for deletion
        l.AutoSize = true;
        splitContainer1.Panel2.Controls.Add(l);        
        l.Text = tag;
        tagBox.Left += l.Width + 5;
        l.BringToFront();
      }
      tagBox.Text = "";
      fillTagBoxAC(); //refill autocomplete except for parsed tags
    }

    #endregion tag box

  }
}
