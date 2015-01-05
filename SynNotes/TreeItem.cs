using System;
using System.Collections.Generic;
using System.Text;

namespace SynNotes {
  class TreeItem {
    public long Id { get; set; }        // sqlite item id
    public string Name { get; set; }    // label
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
