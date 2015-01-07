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

    #region Implementation of INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName) {
      if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion  
  }

  /// <summary>
  /// model for tree roots
  /// </summary> 
  class TagItem : TreeItem {
    public bool System { get; set; }    // sys tags Deleted and All have this set
    public int Count { get; set; }      // count of notes
    public List<NoteItem> Notes { get; set; } // childs list
    public int Index { get; set; }      // order in list
    public bool Expanded { get; set; }  // should it be expanded on start

    public TagItem() {                  // init list
      Notes = new List<NoteItem>();
    }
  }

  /// <summary>
  /// model for tree leafes
  /// </summary>
  class NoteItem : TreeItem {
    public float ModifyDate { get; set; } // unixtime of last modify
    public string DateShort {             // short string of last modify
      get {
         return ModifyDate.ToString();
      }
    }
  }

}
