using System;
using System.Collections.Generic;
using System.Text;

namespace SynNotes {
  class TreeItem {
    public int id { get; set; }         // sqlite item id
    public string name { get; set; }    // label
  }

  class TagItem : TreeItem {
    public bool isSystem { get; set; }  // sys tags Deleted and All have this set
    public int count { get; set; }      // count of notes
    public List<NoteItem> notes { get; set; } // childs list
    public TagItem() {                  // init list
      notes = new List<NoteItem>();
    }
  }

  class NoteItem : TreeItem {
    public float modifyDate { get; set; } // unixtime of last modify
    public string modifyDateS {           // short string of last modify
      get {
         return modifyDate.ToString();
      }
    }
  }

}
