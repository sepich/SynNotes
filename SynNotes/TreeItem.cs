using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SynNotes {
  class TreeItem : INotifyPropertyChanged {
    public long Id { get; set; }        // sqlite item id
    public string Name {                // label
      get { return name; }
      set {
        if (name == value) return;
        name = value;
        this.OnPropertyChanged("Name");
      } 
    }
    private string name;
    public string Lexer { get; set; }  // lexer name

    #region Implementation of INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    internal void OnPropertyChanged(string propertyName) {
      if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion  
  }

  /// <summary>
  /// model for tree roots
  /// </summary> 
  class TagItem : TreeItem {
    public TagItem(List<NoteItem> NotesList) { // init list
      notes = NotesList;
    }

    private List<NoteItem> notes;
    public bool System { get; set; }    // sys tags Deleted and All have this set
    public bool Expanded { get; set; }  // should it be expanded on start
    public int Index { get; set; }      // order in list
    public List<NoteItem> Notes {       // notes of this tag
      get {
        if (!this.System) return notes.FindAll(x => !x.Deleted && x.Tags.Contains(this));
        else {
          if (base.Name == Glob.All) return notes.FindAll(x => !x.Deleted);
          else return notes.FindAll(x => x.Deleted);
        }
      }
    }
    public int Count {                  // count of notes
      get { return Notes.Count; }
    }
  }

  /// <summary>
  /// model for tree leafes
  /// </summary>
  class NoteItem : TreeItem {
    public NoteItem() {                     // init list
      Tags = new List<TagItem>();
      Deleted = false;
      TopLine = -1;
    }

    public List<TagItem> Tags { get; set; } // assigned tags objects
    public float ModifyDate { get; set; }   // unixtime of last modify
    public string DateShort {               // short string of last modify
      get {
        var now = (float)(DateTime.UtcNow.Subtract(Epoch)).TotalSeconds;
        var diff = now - ModifyDate;
        var dt = Epoch.AddSeconds(ModifyDate).ToLocalTime();
        if (diff < 24 * 60 * 60) return dt.ToString("HH:mm"); // this day
        else if (DateTime.Today.Year != dt.Year) return dt.ToString("MMM yy "); //another year
        else return dt.ToString("d MMM"); // this year
      }
    }
    private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public bool Deleted { get; set; }       // is deleted
    public string Snippet { get; set; }     // search match preview
    public int TopLine { get; set; }        // to scroll text to the same place when it was
  }

}
