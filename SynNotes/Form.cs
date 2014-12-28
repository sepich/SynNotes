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

namespace SynNotes {
  
  public partial class Form1 : Form {
    
    // vars
    IniFile ini;
    string conffile = "settings.ini";
    string confuserdir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SynNotes\\";
    KeyHook hook = new KeyHook();

    // WinAPI
    [DllImport("user32.dll")] private static extern int ShowWindow(IntPtr hWnd, uint Msg);
    private const uint SW_RESTORE = 0x09;

    public Form1()
    {
        InitializeComponent();
    }

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
      if (File.Exists(confuserdir + conffile)) ini = new IniFile(confuserdir + conffile);
      else ini = new IniFile(conffile);
      this.WindowState = (FormWindowState)FormWindowState.Parse(this.WindowState.GetType(), ini.GetValue("Form", "WindowState", "Normal"));
      this.Form1_Resize(this, null); //trigger tray icon
      if (this.WindowState == FormWindowState.Normal && !ini.defaults) {
        this.Top = Int32.Parse(ini.GetValue("Form", "Top", "100"));
        this.Left = Int32.Parse(ini.GetValue("Form", "Left", "100"));
        this.Width = Int32.Parse(ini.GetValue("Form", "Width", "500"));
        this.Height = Int32.Parse(ini.GetValue("Form", "Height", "400"));
      }
      // hotkeys
      hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
      setHotkey(1, ini.GetValue("Keys", "HotkeyShow", ""));
      setHotkey(2, ini.GetValue("Keys", "HotkeySearch", "Win+`"));
    }

    void hook_KeyPressed(object sender, KeyPressedEventArgs e) {
      // Show hotkey  
      if (this.WindowState == FormWindowState.Minimized) ShowWindow(this.Handle, SW_RESTORE);
      else {
        this.Activate();
        this.BringToFront();
      }
      //Search hotkey
      if (e.id == 2) this.comboBox1.Focus();
      //statusText.Text = e.Modifier.ToString().Replace(", ", "+") + "+" + e.Key.ToString();
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
      //save ini settings file
      try{
        ini.SaveSettings(conffile);
      }
      catch{
        if (!Directory.Exists(confuserdir)) Directory.CreateDirectory(confuserdir);
        ini.SaveSettings(confuserdir + conffile);
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

  }
}
