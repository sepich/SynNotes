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

    IniFile ini;
    string conffile = "settings.ini";
    string confuserdir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SynNotes\\";

    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e) {      
      // read settings from ini
      if (File.Exists(confuserdir + conffile)) ini = new IniFile(confuserdir + conffile);
      else ini = new IniFile(conffile);
      this.WindowState = (FormWindowState)FormWindowState.Parse(this.WindowState.GetType(), ini.GetValue("Form", "WindowState", "Normal"));
      this.Form1_Resize(this, null); //trigger tray icon
      if (this.WindowState == FormWindowState.Normal) {
        this.Top = Int32.Parse(ini.GetValue("Form", "Top", "100"));
        this.Left = Int32.Parse(ini.GetValue("Form", "Left", "100"));
        this.Width = Int32.Parse(ini.GetValue("Form", "Width", "500"));
        this.Height = Int32.Parse(ini.GetValue("Form", "Height", "400"));
      }
      
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

    [DllImport("user32.dll")] private static extern int ShowWindow(IntPtr hWnd, uint Msg);
    private const uint SW_RESTORE = 0x09;

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

  }
}
