using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SynNotes {

  public sealed class KeyHook : IDisposable {
    [DllImport("user32.dll")] private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    [DllImport("user32.dll")] private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    /// <summary>
    /// Represents the window that is used internally to get the messages.
    /// </summary>
    private class Window : NativeWindow, IDisposable {
      private static int WM_HOTKEY = 0x0312;

      public Window() {
        // create the handle for the window.
        this.CreateHandle(new CreateParams());
      }

      /// <summary>
      /// Overridden to get the notifications.
      /// </summary>
      /// <param name="m"></param>
      protected override void WndProc(ref Message m) {
        base.WndProc(ref m);

        // check if we got a hot key pressed.
        if (m.Msg == WM_HOTKEY) {
          // get the keys.
          Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
          ModifierKey modifier = (ModifierKey)((int)m.LParam & 0xFFFF);
          int id = m.WParam.ToInt32();

          // invoke the event to notify the parent.
          if (KeyPressed != null)
            KeyPressed(this, new KeyPressedEventArgs(modifier, key, id));
        }
      }

      public event EventHandler<KeyPressedEventArgs> KeyPressed;

      #region IDisposable Members

      public void Dispose() {
        this.DestroyHandle();
      }

      #endregion
    }

    private Window _window = new Window();

    public KeyHook() {
      // register the event of the inner native window.
      _window.KeyPressed += delegate(object sender, KeyPressedEventArgs args) {
        if (KeyPressed != null) KeyPressed(this, args);
      };
    }

    /// <summary>
    /// Registers a hot key in the system.
    /// </summary>
    /// <param name="modifier">The modifiers that are associated with the hot key.</param>
    /// <param name="key">The key itself that is associated with the hot key.</param>
    public bool RegisterHotKey(int id, ModifierKey modifier, uint key) {
      return RegisterHotKey(_window.Handle, id, (uint)modifier, key);
    }

    /// <summary>
    /// register hotkey for given string
    /// </summary>
    public void SetHotkey(int id, string keys) {
      if (keys.Length > 0) {
        try {
          var a = keys.Split('+');
          //get keycode
          uint k;
          if (a.Last()[0] == '`') k = (uint)Keys.Oemtilde.GetHashCode();
          else k = (uint)a.Last()[0];
          //get modifiers
          var m = new ModifierKey();
          for (var i = 0; i < a.Length - 1; i++) {
            m = m | (ModifierKey)Enum.Parse(typeof(ModifierKey), a[i].Trim());
          }
          //set key hook
          if (!RegisterHotKey(id, m, k)) MessageBox.Show("Error registering HotKey: " + keys);
        }
        catch {
          MessageBox.Show("Cannot parse '" + keys + "' as keys sequence");
        }
      }
    }

    /// <summary>
    /// A hot key has been pressed.
    /// </summary>
    public event EventHandler<KeyPressedEventArgs> KeyPressed;

    #region IDisposable Members

    public void Dispose() {
      // dispose the inner native window.
      _window.Dispose();
    }

    #endregion
  }

  /// <summary>
  /// Event Args for the event that is fired after the hot key has been pressed.
  /// </summary>
  public class KeyPressedEventArgs : EventArgs {
    private ModifierKey _modifier;
    private Keys _key;
    private int _id;

    internal KeyPressedEventArgs(ModifierKey modifier, Keys key, int id) {
      _modifier = modifier;
      _key = key;
      _id = id;
    }

    public ModifierKey Modifier {
      get { return _modifier; }
    }

    public Keys Key {
      get { return _key; }
    }

    public int id {
      get { return _id; }
    }
  }

  /// <summary>
  /// The enumeration of possible modifiers.
  /// </summary>
  [Flags]
  public enum ModifierKey : uint {
    Alt = 1,
    Ctrl = 2,
    Shift = 4,
    Win = 8
  }
}
