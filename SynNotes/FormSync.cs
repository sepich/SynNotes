using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;

namespace SynNotes {
  public partial class FormSync : Form {
    Form1 _f;
    string[] SyncFreq = new string[] { "Manual", "1m", "5m", "10m", "15m", "30m", "1h" };
 
    public FormSync(Form1 frm) {
      InitializeComponent();
      // load values from main form
      _f = frm;
      tbEmail.Text = _f.sync.Email;
      tbPass.Text = _f.sync.Password;
      cbFreq.DataSource = SyncFreq;
      switch (_f.sync.Freq) {
        case (1):  cbFreq.SelectedItem = "1m"; break;
        case (5):  cbFreq.SelectedItem = "5m"; break;
        case (10): cbFreq.SelectedItem = "10m"; break;
        case (15): cbFreq.SelectedItem = "15m"; break;
        case (30): cbFreq.SelectedItem = "30m"; break;
        case (60): cbFreq.SelectedItem = "1h"; break;
      }
    }

    //link te reg new acc
    private void linkCreate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      linkCreate.LinkVisited = true;
      System.Diagnostics.Process.Start("http://simplenote.com/");
    }

    private void tbEmail_KeyDown(object sender, KeyEventArgs e) {
      // close on ESC
      if (e.KeyCode == Keys.Escape && e.Modifiers == Keys.None) this.Close();
    }

    // save values
    private void btnSave_Click(object sender, EventArgs e) {
      _f.sync.Email = tbEmail.Text.Trim();
      _f.sync.Password = tbPass.Text;
      switch (cbFreq.SelectedItem.ToString()) {
        case("1m"):  _f.sync.Freq = 1; break;
        case("5m"):  _f.sync.Freq = 5; break;
        case("10m"): _f.sync.Freq = 10; break;
        case("15m"): _f.sync.Freq = 15; break;
        case("30m"): _f.sync.Freq = 30; break;
        case("1h"):  _f.sync.Freq = 60; break;
        default:     _f.sync.Freq = 0; break;
      } 
      this.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.Close();
    }
  }
}
