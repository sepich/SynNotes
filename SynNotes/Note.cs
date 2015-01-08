using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace SynNotes {
  public partial class Form1 : Form {

    /// <summary>
    /// used for rendering right part of the screen, managing currently opened Note
    /// </summary> 
    class Note {
      private Form1 f;                       // main form 
      public NoteItem Item { get; set; }     // note id
      private List<Label> Labels;            // tag labels displayed

      public Note(Form1 form) {
        Labels = new List<Label>();
        f = form;
      }

      /// <summary>
      /// show note for selected item
      /// </summary>
      public void ShowSelected() {
        if (f.tree.SelectedItem != null && f.tree.SelectedObject is NoteItem && (NoteItem)f.tree.SelectedObject != Item) Item = (NoteItem)f.tree.SelectedObject;
        else return;

        using (SQLiteCommand cmd = new SQLiteCommand(f.sql)) {
          cmd.CommandText = "SELECT content, lexer, topline FROM notes WHERE id=" + Item.Id;
          using (SQLiteDataReader rdr = cmd.ExecuteReader()) {
            while (rdr.Read()) {
              f.scEdit.Text = rdr.GetString(0);
              f.scEdit.ConfigurationManager.Language = rdr.IsDBNull(1) ? "Bash" : rdr.GetString(1);
              //TODO set top line
            }
          }
        }
        f.scEdit.Modified = false;
        f.Text = GetTitle();
        drawTags();
      }

      /// <summary>
      /// save current note
      /// </summary>
      public void Save() {
        if (Item == null) return;
        Item.ModifyDate = (float)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        Item.Name = GetTitle();
        //save text
        using (SQLiteTransaction tr = f.sql.BeginTransaction()) {
          using (SQLiteCommand cmd = new SQLiteCommand(f.sql)) {
            cmd.CommandText = "UPDATE notes SET modifydate=?, title=?, content=? WHERE id=?";
            cmd.Parameters.AddWithValue(null, Item.ModifyDate);
            cmd.Parameters.AddWithValue(null, Item.Name);
            cmd.Parameters.AddWithValue(null, f.scEdit.Text);
            cmd.Parameters.AddWithValue(null, Item.Id);
            cmd.ExecuteNonQuery();
            //TODO update fts
          }
          tr.Commit();
        }
        f.scEdit.Modified = false;
      }

      /// <summary>
      /// get title from active scintilla text
      /// </summary>
      public string GetTitle() {
        if (f.scEdit.Text.Length == 0) return "(blank)";
        var len1 = f.scEdit.Text.Length > 100 ? 100 : f.scEdit.Text.Length;
        var title = f.scEdit.Text.Substring(0, len1).Trim();
        var len2 = title.IndexOfAny(new char[] { '\n', '\r' });
        if (len2 > 0) return title.Substring(0, len2); // first non-blank line
        else if (len1 < 100) return title;             // whole text is just one line
        else return title + "...";                     // first 100 chars of first line 
      }

      #region tag box
      /// <summary>
      /// rename displayed tag, if exist
      /// </summary>
      public void RenameLabel(string oldvalue) {
        var l = Labels.Find(x => x.Text == oldvalue);
        if (l != null) drawTags();
      }

      /// <summary>
      /// (re)draw tags from string list
      /// </summary>
      private void drawTags() {
        //cleanup
        if (Labels.Count > 0) {
          foreach (Label l in Labels) {
            f.splitContainer1.Panel2.Controls.Remove(l);
            l.Dispose();
          }
          Labels.Clear();
        }
        f.tagBox.Left = 37;

        // create labels
        Item.Tags.ForEach(x => drawTag(x.Name));
      }

      /// <summary>
      /// draw additional tag from string
      /// </summary>
      private void drawTag(string tag) {
        Label l = new Label();
        l.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        l.Location = f.tagBox.Location;
        l.Font = f.tagBox.Font;
        l.BackColor = Color.Gainsboro;
        l.AutoSize = true;
        l.Click += tagClick;
        l.MouseEnter += tagMouseEnter;
        l.MouseLeave += tagMouseLeave;
        l.Cursor = Cursors.Hand;
        f.splitContainer1.Panel2.Controls.Add(l);
        l.Text = tag;
        f.tagBox.Left += l.Width + 5;
        l.BringToFront();
        Labels.Add(l); // used for deletion
      }

      // change tag bg on mouse hover
      private void tagMouseLeave(object sender, EventArgs e) {
        var l = (Label)sender;
        l.BackColor = Color.Gainsboro;
      }
      void tagMouseEnter(object sender, EventArgs e) {
        var l = (Label)sender;
        l.BackColor = Color.Salmon;
      }

      /// <summary>
      /// unassign tag on click
      /// </summary>
      private void tagClick(object sender, EventArgs e) {
        var l = (Label)sender;
        var t = Item.Tags.Find(x => x.Name == l.Text);
        UnassignTag(t);
        RemoveLabel(l);
      }

      /// <summary>
      /// parse textbox for tags to list
      /// </summary>
      public void ParseTags() {
        TagItem tagItem;
        var s = f.tagBox.Text;
        f.tagBox.Text = "";
        using (SQLiteTransaction tr = f.sql.BeginTransaction()) {
          using (SQLiteCommand cmd = new SQLiteCommand(f.sql)) {
            foreach (var tag in s.Split(new char[] { ' ', ',', ';' })) {
              if (tag == "") continue;
              if (Item.Tags.Exists(x => x.Name.ToLower() == tag.ToLower())) continue;     //skip already assigned
              tagItem = f.tags.Find(x => !x.System && x.Name.ToLower() == tag.ToLower()); //search if exist
              if (tagItem==null) {
                //create tag in db if it is new
                cmd.Parameters.Clear();
                cmd.CommandText = "INSERT INTO tags(name,`index`) VALUES(?,?)";                
                cmd.Parameters.AddWithValue(null, tag);
                cmd.Parameters.AddWithValue(null, f.tags.Count - 1);
                cmd.ExecuteNonQuery();
                //refresh tree
                tagItem = new TagItem(f.notes);
                tagItem.Name = tag;
                tagItem.Id = f.sql.LastInsertRowId;
                tagItem.Index = f.tags.Count - 1;
                f.tree.AddObject(tagItem);
                f.tree.SelectedObject = Item;
                f.tags.Add(tagItem);
              }
              cmd.CommandText = "INSERT INTO nt(note,tag) VALUES(" + Item.Id + "," + tagItem.Id + ")";
              cmd.ExecuteNonQuery();
              Item.Tags.Add(tagItem);
              f.tree.RefreshObject(tagItem);              
              drawTag(tagItem.Name);
            }
          } 
          tr.Commit();
        }
        FillAutocomplete(); //refill autocomplete except for parsed tags
      }

      /// <summary>
      /// fill autocomplete with unused tags
      /// </summary>
      public void FillAutocomplete() {
        f.tagBox.AutoCompleteCustomSource.Clear();
        if (f.tags.Count > 0) {
          foreach(var x in f.tags){
            if (!x.System && !Item.Tags.Contains(x)) f.tagBox.AutoCompleteCustomSource.Add(x.Name);
          }
        }
      }

      /// <summary>
      /// unassign Note from last assigned tag
      /// </summary>
      public void UnassignLastTag() {
        if (Item.Tags.Count > 0) {
          var l = Labels[Labels.Count - 1];
          f.tagBox.Left = l.Left;
          UnassignTag(Item.Tags[Item.Tags.Count - 1]);
          RemoveLabel(l, null, false); // don't redraw all labels, just move textbox left edge back
        }
      }

      /// <summary>
      /// remove label if exist, optionally find label by tag
      /// </summary>
      public void RemoveLabel(Label label, TagItem tag=null, bool redraw = true) {
        if (label == null) label = Labels.Find(x => x.Text == tag.Name);
        if (label == null) return;
        f.splitContainer1.Panel2.Controls.Remove(label);
        label.Dispose();
        Labels.Remove(label);
        if(redraw) drawTags(); //not used in del last label
      }

      /// <summary>
      /// Unassign provided tag if it assigned
      /// </summary>
      /// <param name="label">use null for autosearch</param>
      public void UnassignTag(TagItem tag) {
        if (tag == null) return;
        Item.Tags.Remove(tag);
        //save to db
        using (SQLiteTransaction tr = f.sql.BeginTransaction()) {
          using (SQLiteCommand cmd = new SQLiteCommand(f.sql)) {
            cmd.CommandText = "DELETE FROM nt WHERE note="+Item.Id+" AND tag="+tag.Id;
            cmd.ExecuteNonQuery();
          }
          tr.Commit();
        }
        //update tree
        f.tree.RefreshObject(tag);
        FillAutocomplete();
      }
      #endregion tag box


    }
  }
}
