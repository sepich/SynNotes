using System;
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
using Microsoft.VisualBasic;

namespace SynNotes {
  
  public partial class Form1 : Form {
    
    // vars
    IniFile ini;
    const string conffile = "settings.ini";
    const string dbfile = "notes.db";
    const string dbver = "1";
    const string DELETED = "Deleted";
    const string ALL = "All";
    string userdir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SynNotes\\";
    KeyHook hook = new KeyHook();
    Note note;
    SQLiteConnection sql;
    List<TagItem> roots;

    // WinAPI
    [DllImport("user32.dll")] private static extern int ShowWindow(IntPtr hWnd, uint Msg);
    private const uint SW_RESTORE = 0x09;

    public Form1() {InitializeComponent();}

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
      hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(HotkeyPressed);
      hook.SetHotkey(1, ini.GetValue("Keys", "HotkeyShow", ""));
      string s = ini.GetValue("Keys", "HotkeySearch", "Win+`");
      hook.SetHotkey(2, s);
      cbSearch.AccessibleDescription = "Search Notes (" + s + ")"; //used for placeholder
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
      note = new Note(this);
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

    /// <summary>
    /// try to connect to provided db, create if not exist
    /// </summary>
    private void sqlConnect(string db, bool warn = true) {
      try {
        SQLiteConnectionStringBuilder connString = new SQLiteConnectionStringBuilder();
        connString.DataSource = db;
        connString.FailIfMissing = false;
        connString.ForeignKeys = true;
        sql = new SQLiteConnection(connString.ToString());
        sql.Open();
        //check if db valid to throw ex, as Open do nothing
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

    /// <summary>
    /// called for registered hotkeys
    /// </summary>
    private void HotkeyPressed(object sender, KeyPressedEventArgs e) {
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
      if (sql != null) {
        if (scEdit.Modified) note.Save();
        using (SQLiteTransaction tr = sql.BeginTransaction()) {
          using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
            cmd.CommandText = "UPDATE tags SET expanded=0";
            cmd.ExecuteNonQuery();
            foreach (var item in tree.ExpandedObjects) {
              if (!(item is TagItem)) continue;
              var tag = (TagItem)item;
              cmd.CommandText = "UPDATE tags SET expanded=1 WHERE id=" + tag.Id;
              cmd.ExecuteNonQuery();
            }
            cmd.CommandText = "INSERT OR REPLACE INTO config(name,value) VALUES('lastNote'," + note.Item.Id + ")";
            cmd.ExecuteNonQuery();
          }
          tr.Commit();
        }
        //close db connection
        sql.Dispose();
      }
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
      if (e.KeyCode == Keys.Escape && e.Modifiers == Keys.None) {
        if (tagBox.Focused) cbSearch.Focus();
        else if (cbSearch.Focused && cbSearch.Text.Length > 0) cbSearch.Text = "";
        else this.WindowState = FormWindowState.Minimized;
      }
      if (e.KeyCode == Keys.Delete && tree.Focused) delClick(null, null);
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
      var s = "SELECT 0 as id, '"+ALL+"' AS name, count(id) AS num, -1 AS `index`, 0 AS expanded FROM notes WHERE deleted=0";
      roots = getTags(s, true);
      //Deleted as system
      s = "SELECT 0 as id, '" + DELETED + "' AS name, count(id) AS num, " + (int.MaxValue).ToString() + " AS `index`, 0 AS expanded FROM notes WHERE deleted=1";
      roots.AddRange(getTags(s, true));
      //read all tags info
      s = "SELECT t.id, t.name, COUNT(c.note) AS num, `index`, expanded FROM tags t LEFT JOIN nt c ON t.id=c.tag GROUP BY t.name ORDER BY t.`index`";
      roots.AddRange(getTags(s));

      //getters
      tree.Roots = roots;
      tree.CanExpandGetter = delegate(object x) {
        return (x is TagItem && ((TagItem)x).Count > 0);
      };
      tree.ChildrenGetter = delegate(object x) { return getNotes(x); };
      cDate.AspectGetter = delegate(object x) {
        if (x is TagItem) return ((TagItem)x).Count;
        else return ((NoteItem)x).DateShort;
      };
      cSort.AspectGetter = delegate(object x) {
        if (x is TagItem) return ((TagItem)x).Index;
        else return ((NoteItem)x).Name;
      };
      //restore view
      tree.Sort(cSort, SortOrder.Ascending);
      foreach (var tag in roots) {
        if (tag.Expanded) tree.Expand(tag);
      }

      //search last opened note
      using (SQLiteCommand cmd = new SQLiteCommand("SELECT value FROM config WHERE name='lastNote'", sql)) {
        var res = cmd.ExecuteScalar();
        var id = Convert.ToInt32(res);
        if (res == null) btnAdd_ButtonClick(null, null); //first run - create new note and select it
        else {
          //check expanded tags for note
          NoteItem sel = null;
          foreach (var tag in roots) {
            var tmp = tag.Notes.Find(x => x.Id == id);
            if (tmp != null) {
              sel = tmp;
              break;
            }
          }
          //check in All
          if (sel == null) {
            tree.Expand(roots[0]);
            sel = roots[0].Notes.Find(x => x.Id == id);
          }
          //check in Deleted
          if (sel == null) {
            tree.Expand(roots[1]);
            sel = roots[1].Notes.Find(x => x.Id == id);
          }
          tree.SelectedObject = sel;
        }
      }

      //renderer
      //this.tree.TreeColumnRenderer.LinePen = new Pen(Color.Transparent);
    }

    /// <summary>
    /// return list of tag tree nodes for sql query
    /// </summary>
    /// <param name="query">query [id, name, num, index]</param>
    /// <param name="sys">set not a user tag (e.g All, Deleted)</param>
    private List<TagItem> getTags(string query, bool sys=false) {
      List<TagItem> res = new List<TagItem>();
      TagItem node;
      using (SQLiteCommand cmd = new SQLiteCommand(query, sql)) {
        using (SQLiteDataReader rdr = cmd.ExecuteReader()) {
          while (rdr.Read()) {
            node = new TagItem();
            node.Id = Convert.ToInt32(rdr["id"]);
            node.Name = rdr["name"].ToString();
            node.Count = Convert.ToInt32(rdr["num"]);
            node.Index = Convert.ToInt32(rdr["index"]);
            node.Expanded = Convert.ToBoolean(rdr["expanded"]);
            node.System = sys;
            res.Add(node);
          }
        }
      }
      return res;
    }

    /// <summary>
    /// get notes when tag is expanded
    /// </summary>
    private List<NoteItem> getNotes(object x) {
      TagItem tag = (TagItem)x;
      if (tag.Notes.Count > 0) tag.Notes.Clear();
      NoteItem node;
      string s;
      if (tag.System) {
        if (tag.Name == ALL) s = "SELECT id, title, modifydate FROM notes WHERE deleted=0";  // All
        else s = "SELECT id, title, modifydate FROM notes WHERE deleted=1";                  // Deleted
      }
      else s = "SELECT n.id, n.title, n.modifydate"+                                         // per-tag
        " FROM notes n LEFT JOIN nt c ON c.note=n.id" +
        " WHERE n.deleted=0 AND c.tag=" + tag.Id.ToString() + 
        " ORDER BY pinned DESC, title ASC";
      using (SQLiteCommand cmd = new SQLiteCommand(s, sql)) {
        using (SQLiteDataReader rdr = cmd.ExecuteReader()) {
          while (rdr.Read()) {
            node = new NoteItem();
            node.Id = rdr.GetInt32(0);
            node.Name = rdr.GetString(1);
            node.ModifyDate = rdr.GetFloat(2);
            tag.Notes.Add(node);
          }
        }
      }
      return tag.Notes;
    }

    //add new note
    private void btnAdd_ButtonClick(object sender, EventArgs e) {
      // get parent tag if something selected
      TagItem tag;
      if (tree.SelectedObject!=null) {
        if (tree.SelectedObject is TagItem) tag = (TagItem)tree.SelectedObject;
        else tag = (TagItem)tree.GetParent(tree.SelectedObject);
        if (tag.System && tag.Name == DELETED) tag = roots[0]; //don't create new deleted notes
      }
      else {
        tag = roots[0]; //All
      }
      //add new note to db
      var ut = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString().Replace(',','.');
      using (SQLiteTransaction tr = sql.BeginTransaction()) {
        using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
          cmd.CommandText = "INSERT INTO notes(modifydate, createdate, title, content) "+
            "VALUES("+ut+", "+ut+", 'New Note', 'New Note')";
          cmd.ExecuteNonQuery();
          if (!tag.System) {
            cmd.CommandText = "INSERT INTO nt(note,tag) VALUES(" + sql.LastInsertRowId.ToString() + "," + tag.Id + ")";
            cmd.ExecuteNonQuery();
          }
        }
        tr.Commit();
      }
      // refresh tree root, select new leaf
      tag.Count += 1;
      tree.RefreshObject(tag);
      tree.Expand(tag);
      tree.Reveal(tag.Notes.Find(x => x.Id == sql.LastInsertRowId), true);
      scEdit.Focus();
    }

    private void tree_SelectionChanged(object sender, EventArgs e) {
      if (scEdit.Modified) note.Save();
      note.ShowSelected();
    }  

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

    // fill tree context menu with items valid for row
    private void tree_CellRightClick(object sender, CellRightClickEventArgs e) {
      treeMenu.Items.Clear();
      //for tags
      if (e.Model is TagItem) {
        var tag = (TagItem)e.Model;
        if (tag.System && tag.Name == DELETED && tree.SelectedObjects.Count == 1) {
          var purge = treeMenu.Items.Add("Purge Deleted");
          purge.Click += purgeTagClick;
        }
        else {
          var newnote = treeMenu.Items.Add("New Note");
          newnote.Click += btnAdd_ButtonClick;
        }
        if (!tag.System) {
          if (tree.SelectedObjects.Count == 1) {
            var ren = treeMenu.Items.Add("Rename");
            ren.Click += renTagClick;
          }

          var del = treeMenu.Items.Add("Delete");
          del.Click += delClick;
        }
      }
      //for notes
      else {
        var newnote = treeMenu.Items.Add("New Note");
        newnote.Click += btnAdd_ButtonClick;

        var del = treeMenu.Items.Add("Delete");
        del.Click += delClick;
      }
    }

    //purge deleted notes
    private void purgeTagClick(object sender, EventArgs e) {
      var tag = roots.Find(x => x.System && x.Name == DELETED);
      if (tag != null) {
        tree.Expand(tag);
        tree.SelectedObjects = tag.Notes;
        delClick(null, null);
      }
    }

    //rename tag
    private void renTagClick(object sender, EventArgs e) {
      tree.EditModel(tree.SelectedObject);
    }

    //delete selected items
    private void delClick(object sender, EventArgs e) {
      var n = tree.SelectedObjects.Count;
      if(n>1 && MessageBox.Show("Delete " + n.ToString() + " objects?", "Delete multiple objects?", 
        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No) return;

      using (SQLiteTransaction tr = sql.BeginTransaction()) {
        using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
          foreach (var item in tree.SelectedObjects) {
            // delete tag
            if (item is TagItem) {
              var tag = (TagItem)item;
              if (tag.System) continue; //can't del system folder
              if (n>1 || MessageBox.Show("Delete the tag: " + tag.Name + "?\n(This will not delete it's Notes, just unassign this Tag from them)",
                "Delete Tag?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                == System.Windows.Forms.DialogResult.Yes) {
                cmd.CommandText = "DELETE FROM tags WHERE id=" + tag.Id;
                cmd.ExecuteNonQuery();
                note.UnassignTag(tag.Name, null);
                tree.RemoveObject(tag);
                roots.Remove(tag);
              }
            }
            //delete note
            else {
              var i = (NoteItem)item;
              var tag = (TagItem)tree.GetParent(i);
              //purge it
              if (tag.System && tag.Name == DELETED) {
                if (n == 1 && MessageBox.Show("Purge the note: " + i.Name + "?\n(This will purge the Note, no undelete is possible)", "Purge Note?",
                  MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No) return;
                cmd.CommandText = "DELETE FROM notes WHERE id=" + i.Id;
                cmd.ExecuteNonQuery();
              }
              //move to deleted
              else {
                if (n == 1 && MessageBox.Show("Delete the note: " + i.Name + "?\n(This will move the Note to Deleted items folder)", "Delete Note?",
                  MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No) return;
                cmd.CommandText = "UPDATE notes SET deleted=1 WHERE id=" + i.Id;
                cmd.ExecuteNonQuery();
                var del = roots.Find(x => x.System && x.Name == DELETED);
                del.Count += 1;
                tree.RefreshObject(del);
              }
              tag.Count -= 1;
              tree.RefreshObject(tag);
            }
          }
        }
        tr.Commit();
      }
      if (n > 1 && tree.SelectedObjects.Count > 0) tree.SelectedObject = tree.SelectedObjects[0]; //reset the selection
    }

    // expand tag by key / mouse click
    private void tree_ItemActivate(object sender, EventArgs e) {
      if (tree.SelectedObject is TagItem) tree.ToggleExpansion(tree.SelectedObject);
    }
    private void tree_MouseClick(object sender, MouseEventArgs e) {
      if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Clicks == 1 && tree.SelectedObject is TagItem) tree.ToggleExpansion(tree.SelectedObject);
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
      if (roots.Exists(x => !x.System && x != e.RowObject && x.Name.ToLower() == val.ToLower())) {
        MessageBox.Show("Tag with name '" + val + "' already exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        e.Cancel = true;
        return;
      }
    }

    //rename tag
    private void tree_CellEditFinishing(object sender, CellEditEventArgs e) {
      if (e.Cancel) return;
      var tag = ((TagItem)e.RowObject);
      var delims = new char[] { ' ', ',', ';' };
      var val = e.NewValue.ToString().Trim(delims);
      using (SQLiteTransaction tr = sql.BeginTransaction()) {
        using (SQLiteCommand cmd = new SQLiteCommand(sql)) {
          cmd.CommandText = "UPDATE tags SET name=? WHERE id=?";
          cmd.Parameters.AddWithValue(null, val);
          cmd.Parameters.AddWithValue(null, tag.Id);
          cmd.ExecuteNonQuery();
        }
        tr.Commit();
      }
      note.RenameTag(tag.Name, val);
      tag.Name = val;
      tree.RefreshObject(tag);
    }



  }
}
