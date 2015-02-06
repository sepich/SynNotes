# SynNotes
Simple syntax highlighted Notes manager with incremental full-text search and GMaill like tags as folders.  
Most of the time app basically hides in the system tray. Then you push global hotkey, and it appears with last Note opened and Search field already focused. After you found data needed hide the app back by pressing `ESC`.
![overview](http://habrastorage.org/files/34c/10f/8da/34c10f8da396435882ea93d20c77364b.png)
![search](http://habrastorage.org/files/562/b9a/c7c/562b9ac7cb764f89a34ecd63fc65719a.png)

#### Used
 - C#
 - [Scintilla.NET](http://scintillanet.codeplex.com/)
 - [System.Data.SQLite](http://system.data.sqlite.org/index.html/doc/trunk/www/index.wiki)
 - [ObjectListView](http://objectlistview.sourceforge.net/cs/index.html)
 
#### Todo
 - [Simplenote](http://simple-note.appspot.com/) sync
 
#### Notes
That is very simple app and it will be so. No bells'n'whistles for reduced memory footprint (c# lol;). 

##### Configuration
Some configuration could be done by editing `settings.ini` which is located:
 - in executable directory if it is available for writing
 - `%AppData%\SynNotes` if executable dir is not available for writing (ex: `Programm Files`)

For *Global Hotkeys* edit Keys part (default is `Win+tilde` for Search):
```ini
[Keys]
HotkeyShow=
HotkeySearch=Win+`
```

For fans of dark backrounds basic dark theme included. You can enable it like this:
```ini
[Scintilla]
Theme=Visual Studio Dark.xml
```

##### Lexers
 - When Note has no explicit Lexer (language to highlight syntax) selected it will inherit it from it's tags. 
 - If Note has multiple tags assigned, privilege has Tag which is higher in tree (you can arrange tags by drag'n'drop) In this case name of Lexer would be prefixed by ^
 - If both Note and all it's Tags has no Lexer assigned `bash` is used by default

##### Download
You can download compiled binaries at [Releases](https://github.com/sepich/SynNotes/releases) section  
Changelog is available in [commits](https://github.com/sepich/SynNotes/commits/master) section
