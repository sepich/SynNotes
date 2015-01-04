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
      private List<string> Tags { get; set; }// note tag names list
      private List<Label> Labels;            // tag labels displayed

      public Note(Form1 form) {
        Tags = new List<string>();
        Labels = new List<Label>();
        f = form;
      }

      /// <summary>
      /// show note for selected item
      /// </summary>
      public void ShowSelected() {
        if (f.tree.SelectedItem != null && f.tree.SelectedObject is NoteItem) Item = (NoteItem)f.tree.SelectedObject;
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

        //load tags
        Tags.Clear();
        using (SQLiteCommand cmd = new SQLiteCommand(f.sql)) {
          cmd.CommandText = "SELECT t.name FROM tags t LEFT JOIN nt c ON t.id=c.tag WHERE c.note=" + Item.Id;
          using (SQLiteDataReader rdr = cmd.ExecuteReader()) {
            while (rdr.Read()) {
              Tags.Add(rdr.GetString(0));
            }
          }
        }
        drawTags();
      }

      /// <summary>
      /// save current note
      /// </summary>
      public void Save() {
        if (Item == null) return;
        var ut = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        var title = GetTitle();
        //save text
        using (SQLiteTransaction tr = f.sql.BeginTransaction()) {
          using (SQLiteCommand cmd = new SQLiteCommand(f.sql)) {
            cmd.CommandText = "UPDATE notes SET modifydate=?, title=?, content=? WHERE id=?";
            cmd.Parameters.AddWithValue(null, ut);
            cmd.Parameters.AddWithValue(null, title);
            cmd.Parameters.AddWithValue(null, f.scEdit.Text);
            cmd.Parameters.AddWithValue(null, Item.Id);
            cmd.ExecuteNonQuery();
            //TODO update fts
          }
          tr.Commit();
        }

        //refresh tree
        f.scEdit.Modified = false;
        Item.Name = title;
        f.tree.RefreshObject(Item);
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
      /// (re)draw tags from string list
      /// </summary>
      private void drawTags() {
        //cleanup
        if (Labels.Count > 0) 
          foreach (Label l in Labels) {
            f.splitContainer1.Panel2.Controls.Remove(l);
            l.Dispose();
          }
        f.tagBox.Left = 37;
        f.tagBox.Text = "";

        // create labels
        foreach (var tag in Tags) {          
          Label l = new Label();
          l.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
          l.Location = f.tagBox.Location;
          l.Font = f.tagBox.Font;
          l.BackColor = Color.Gainsboro;
          l.AutoSize = true;
          f.splitContainer1.Panel2.Controls.Add(l);
          l.Text = tag;
          f.tagBox.Left += l.Width + 5;
          l.BringToFront();
          Labels.Add(l); // used for deletion
        }        
      }

      /// <summary>
      /// parse textbox for tags to list
      /// </summary>
      public void ParseTags(bool repaint = false) {
        var newtags = new List<string>();
        foreach (var tag in f.tagBox.Text.Split(new char[] { ' ', ',', ';' })) {
          if (tag == "") continue;
          if (Tags.Contains(tag, StringComparer.CurrentCultureIgnoreCase)) continue; //skip already assigned
          Tags.Add(tag);
          // check if new
          if (f.roots.Any(x => x.Name.ToLower() == tag.ToLower())) continue;
          newtags.Add(tag);
        }

        // save to db if there are new tags
        if(newtags.Count>0){
          using (SQLiteTransaction tr = f.sql.BeginTransaction()) {
            using (SQLiteCommand cmd = new SQLiteCommand(f.sql)) {
              foreach (var tag in newtags) {
                cmd.Parameters.Clear();
                cmd.CommandText = "INSERT INTO tags(name,`index`) VALUES(?,?)";                
                cmd.Parameters.AddWithValue(null, tag);
                cmd.Parameters.AddWithValue(null, f.roots.Count - 1);
                cmd.ExecuteNonQuery();
                var id=f.sql.LastInsertRowId;

                cmd.CommandText = "INSERT INTO nt(note,tag) VALUES("+Item.Id.ToString()+","+id.ToString()+")";
                cmd.ExecuteNonQuery();

                //refresh tree
                var newtag = new TagItem();
                newtag.Name = tag;
                newtag.Id = id;
                newtag.Count = 1;
                f.roots.Insert(f.roots.Count - 1, newtag);
                f.tree.AddObject(newtag);
              }
            }
            tr.Commit();
          }
        }

        f.tagBox.Text = "";
        FillAutocomplete(); //refill autocomplete except for parsed tags
      }

      /// <summary>
      /// fill autocomplete with unused tags
      /// </summary>
      public void FillAutocomplete() {
        f.tagBox.AutoCompleteCustomSource.Clear();
        if (f.roots.Count > 0) {          
          foreach (var tag in f.roots) {
            if (tag.System) continue; //skip system tags
            if (Tags.Contains(tag.Name, StringComparer.CurrentCultureIgnoreCase)) continue; //skip already used tags
            f.tagBox.AutoCompleteCustomSource.Add(tag.Name);
          }
        }
      }

      #endregion tag box
    }
  }
}
