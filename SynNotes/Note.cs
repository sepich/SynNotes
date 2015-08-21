using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using ScintillaNET;

namespace SynNotes {
  public partial class Form1 : Form {

    /// <summary>
    /// used for rendering right part of the screen, managing currently opened Note
    /// </summary> 
    class Note {
      private Form1 f;                       // main form 
      public NoteItem Item { get; set; }     // currently opened note
      private List<Label> Labels;            // tag labels displayed      
      private int syncnum;                   // ver of opened note for auto update

      public Note(Form1 form) {
        Labels = new List<Label>();
        f = form;
      }

      /// <summary>
      /// show note for selected item
      /// </summary>
      public void ShowSelected() {
        if (Item != null && f.tree.RowHeight < 0) Item.TopLine = f.scEdit.Lines.FirstVisibleIndex; //not search, save previous note top line
        var n = f.tree.SelectedObject as NoteItem;
        if (n == null) return;
        if (n != Item || syncnum != n.SyncNum) {
          Item = n;
          syncnum = n.SyncNum;
        }
        else return; // don't redraw same note (in search mode)
        f.scEdit.DocumentChange -= f.scEdit_DocumentChange;

        using (SQLiteCommand cmd = new SQLiteCommand(f.sql)) {
          cmd.CommandText = "SELECT content, lexer, topline FROM notes WHERE id=" + Item.Id;
          using (SQLiteDataReader rdr = cmd.ExecuteReader()) {
            while (rdr.Read()) {              
              f.scEdit.Text = rdr.GetString(0);              
              if (rdr.IsDBNull(1)) { //use tag's lexer
                var lex = "Bash";
                foreach (var tag in Item.Tags) if (!String.IsNullOrEmpty(tag.Lexer)) {
                  lex = tag.Lexer;
                  break;
                }
                SetLanguage(lex);
                f.btnLexer.Text = "^" + lex;
                Item.Lexer = null;
              }
              else {
                Item.Lexer = rdr.GetString(1);
                SetLanguage(Item.Lexer);
                f.btnLexer.Text = Item.Lexer;                
              }              
              if (Item.TopLine == -1 && !rdr.IsDBNull(2) ) Item.TopLine = rdr.GetInt32(2);
              f.scEdit.Lines.FirstVisibleIndex = Item.TopLine;
            }
          }
        }
        f.scEdit.Modified = false;
        f.scEdit.DocumentChange += f.scEdit_DocumentChange;
        f.Text = GetTitle();
        drawTags();        
        //highlight search term and scroll to it
        if (f.tbSearch.ForeColor == SystemColors.WindowText && f.tbSearch.Text.Length > 0) {
          var top = f.scEdit.Lines.Count;
          foreach (var item in f.tbSearch.Text.Split(' ')) {
            if (item.Length == 0) continue;
            foreach (Range r in f.scEdit.FindReplace.FindAll(item)) {
              r.SetIndicator(0);
              if (r.StartingLine.Number < top) top = r.StartingLine.Number;
            }
          }
          f.scEdit.Lines.FirstVisibleIndex = top;
        }
      }

      /// <summary>
      /// set scintilla lexer and styles
      /// </summary>
      public void SetLanguage(string lang) {
        f.scEdit.Styles.ResetDefault();
        foreach (var s in f.lexers["globals"]) {
          if (s.id != 0) {
            f.scEdit.Styles[s.id].ForeColor = s.fgcolor;
            f.scEdit.Styles[s.id].BackColor = s.bgcolor;
            if (!String.IsNullOrEmpty(s.fontname)) f.scEdit.Styles[s.id].FontName = s.fontname;
            if (s.fontsize > 0) f.scEdit.Styles[s.id].Size = s.fontsize;
            f.scEdit.Styles[s.id].Bold = s.bold;
            f.scEdit.Styles[s.id].Italic = s.italic;
            f.scEdit.Styles[s.id].Underline = s.underline;
          }
        }
        f.scEdit.Styles.ClearAll(); // i.e. Apply to all
        f.scEdit.Lexing.LexerName = lang;
        if (Glob.Lexers.Contains(lang)) {
          //styles
          if(f.lexers.ContainsKey(lang)) foreach (var s in f.lexers[lang]) {
            if (s.id != 0) {
              f.scEdit.Styles[s.id].ForeColor = s.fgcolor;
              f.scEdit.Styles[s.id].BackColor = s.bgcolor;
              if (!String.IsNullOrEmpty(s.fontname)) f.scEdit.Styles[s.id].FontName = s.fontname;
              if (s.fontsize > 0) f.scEdit.Styles[s.id].Size = s.fontsize;
              f.scEdit.Styles[s.id].Bold = s.bold;
              f.scEdit.Styles[s.id].Italic = s.italic;
              f.scEdit.Styles[s.id].Underline = s.underline;
            }
          }
          //keywords
          if (f.keywords.ContainsKey(lang)) for (int i =0; i<f.keywords[lang].Count; i++) {
            f.scEdit.Lexing.SetKeywords(i, f.keywords[lang][i]);
          }
        }
      }

      /// <summary>
      /// save current note
      /// </summary>
      public void Save() {
        if (Item == null) return;
        Item.ModifyDate = (DateTime.UtcNow.Subtract(Glob.Epoch)).TotalSeconds;
        var t=GetTitle();
        if (Item.Name != t) {
          Item.Name = t;
          foreach (var tag in Item.Tags) f.tree.RefreshObject(tag); //refresh tag to resort notes (if note name changed)
          f.Text = t;
        }
        //save text
        try {
          using (SQLiteTransaction tr = f.sql.BeginTransaction()) {
            using (SQLiteCommand cmd = new SQLiteCommand(f.sql)) {
              cmd.CommandText = "UPDATE notes SET modifydate=?, title=?, content=?, topline=? WHERE id=?";
              cmd.Parameters.AddWithValue(null, Item.ModifyDate);
              cmd.Parameters.AddWithValue(null, Item.Name);
              cmd.Parameters.AddWithValue(null, f.scEdit.Text);
              cmd.Parameters.AddWithValue(null, Item.TopLine);
              cmd.Parameters.AddWithValue(null, Item.Id);
              cmd.ExecuteNonQuery();
            }
            tr.Commit();
          }
          f.scEdit.Modified = false;
        }
        catch {
          f.statusText.Text = Glob.Unsaved;
        }
      }

      /// <summary>
      /// get title from active scintilla text (or provided text)
      /// </summary>
      public string GetTitle(string Text=null) {
        if (Text == null) Text = f.scEdit.Text;
        if (Text.Length == 0) return "(blank)";
        var len1 = Text.Length > 100 ? 100 : Text.Length;
        var title = Text.Substring(0, len1).Trim();
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
      public void drawTags() {
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
      public void ParseTags(String tagstr=null, NoteItem note=null) {
        bool draw = false; //called not for current item, don't draw tags/autocompl
        if (tagstr == null) {
          tagstr = f.tagBox.Text;
          f.tagBox.Text = "";
          draw = true; 
        }
        if (note == null) note = Item;
        var isSearch = f.tree.RowHeight > 0;
        
        using (SQLiteTransaction tr = f.sql.BeginTransaction()) {
          using (SQLiteCommand cmd = new SQLiteCommand(f.sql)) {
            //cleanup existing tags, as it is replace
            if (!draw) {
              cmd.CommandText = "DELETE FROM nt WHERE note=" + note.Id;
              cmd.ExecuteNonQuery();
              note.Tags.Clear();
            }
            var added = false;
            foreach (var tag in tagstr.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)) {
              if (String.IsNullOrEmpty(tag)) continue;
              if (note.Tags.Exists(x => x.Name.ToLower() == tag.ToLower())) continue;     //skip already assigned
              var tagItem = f.tags.Find(x => !x.System && x.Name.ToLower() == tag.ToLower()); //search if exist
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
                if (!isSearch) {
                  f.tree.AddObject(tagItem);
                  f.cName.Renderer = f.fancyRenderer; //OLV drop renderer when Roots refreshed
                  if (draw) f.tree.SelectedObject = note;
                }
                f.tags.Add(tagItem);
              }
              cmd.CommandText = "INSERT INTO nt(note,tag) VALUES(" + note.Id + "," + tagItem.Id + ")";
              cmd.ExecuteNonQuery();
              note.Tags.Add(tagItem);
              if (!isSearch) f.tree.RefreshObject(tagItem);
              if (draw) drawTag(tagItem.Name);
              added = true;
            }
            if (draw && added) { // there was some tags actually added, update modifydate for sync
              note.ModifyDate = (DateTime.UtcNow.Subtract(Glob.Epoch)).TotalSeconds;
              cmd.CommandText = "UPDATE notes SET modifydate=? WHERE id=" + note.Id;
              cmd.Parameters.AddWithValue(null, note.ModifyDate);
              cmd.ExecuteNonQuery();
            }
          } 
          tr.Commit();
        }
        if (draw) FillAutocomplete(); //refill autocomplete except for parsed tags
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
        Item.ModifyDate = (DateTime.UtcNow.Subtract(Glob.Epoch)).TotalSeconds;
        //save to db
        using (SQLiteTransaction tr = f.sql.BeginTransaction()) {
          using (SQLiteCommand cmd = new SQLiteCommand(f.sql)) {
            cmd.CommandText = "DELETE FROM nt WHERE note="+Item.Id+" AND tag="+tag.Id;
            cmd.ExecuteNonQuery();
            cmd.CommandText = "UPDATE notes SET modifydate=? WHERE id=" + Item.Id; //update modifydate for sync
            cmd.Parameters.AddWithValue(null, Item.ModifyDate);
            cmd.ExecuteNonQuery();
          }
          tr.Commit();
        }
        //update tree
        if (Item.Tags.Count == 0) f.tree.SelectObject(f.tagAll, true); //set focus to All when Note lost last tag
        f.tree.RefreshObject(f.tagAll);
        f.tree.RefreshObject(tag);
        FillAutocomplete();
      }
      #endregion tag box


    }
  }
}
