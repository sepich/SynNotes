using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynNotes {
  class Sync {
    public string Email { get; set; }
    public string Password { get; set; }
    public int Freq { get; set; } // minutes, 0=Manual
  }
}
