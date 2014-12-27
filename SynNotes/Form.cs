using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

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
  }
}
