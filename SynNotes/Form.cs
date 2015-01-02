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
      setHotkey(2, ini.GetValue("Keys", "HotkeySearch", "Win+`"));      
      //check db
      if (File.Exists(dbfile)) sqlConnect(dbfile);
      else if (File.Exists(userdir + dbfile)) sqlConnect(userdir + dbfile);
      else {
        sqlConnect(dbfile, false);
        if(sql==null) sqlConnect(userdir + dbfile);
        sqlCreate();
      }
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
              "deleted BOOLEAN," + //in trash or not
              "modifydate REAL," + //unixtime of last edit
              "createdate REAL," + //unixtime of creation
              "syncnum INTEGER," + //track note changes
              "version INTEGER," + //track note content changes
              "systemtags TEXT," + //array of not-parsed tags
              "pinned BOOLEAN," +  //displayed before others
              "unread BOOLEAN," +  //modified shared note
              "tags TEXT," +       //array of strings
              "title TEXT," +      //copy of first line of text
              "content TEXT," +    //note content, including the first line
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

  }
}
