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
    string[] SyncFreq = new string[] { "Manual", "1m", "5m", "10m", "15m", "30m", "1h" };
 
    public FormSync() {
      InitializeComponent();
      // load values from Sync
      tbEmail.Text = Sync.Email;
      tbPass.Text = Sync.Password;
      cbFreq.DataSource = SyncFreq;
      switch (Sync.Freq) {
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
      Sync.Email = tbEmail.Text.Trim();
      Sync.Password = tbPass.Text;
      switch (cbFreq.SelectedItem.ToString()) {
        case("1m"):  Sync.Freq = 1; break;
        case("5m"):  Sync.Freq = 5; break;
        case("10m"): Sync.Freq = 10; break;
        case("15m"): Sync.Freq = 15; break;
        case("30m"): Sync.Freq = 30; break;
        case("1h"):  Sync.Freq = 60; break;
        default:     Sync.Freq = 0; break;
      } 
      this.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.Close();
    }
  }
}
