# SynNotes
Simple syntax highlighted Notes manager with incremental full-text search and GMaill like tags as folders.  
Most of the time app basically hides in the system tray. Then you push global hotkey, and it appears with last Note opened and Search field already focused. After you found data needed hide the app back by pressing `ESC`.
![overview](https://habrastorage.org/files/997/03c/569/99703c5690cf4969b315e3148280e73d.png)  
![search](https://habrastorage.org/files/d97/725/631/d97725631d1444918e80b4be49792697.png)  
When you have some notes created - you probably would like to sync them to other your workstations/mobile devices. Also, versioning and cloud backups would be nice. All that provided if you enable sync with your [Simplenote](http://simplenote.com/) account

#### Used
 - C# (XP users should have [.Net4.0](https://www.microsoft.com/en-US/download/details.aspx?id=17851) installed)
 - [Scintilla.NET](http://scintillanet.codeplex.com/)
 - [System.Data.SQLite](http://system.data.sqlite.org/index.html/doc/trunk/www/index.wiki)
 - [ObjectListView](http://objectlistview.sourceforge.net/cs/index.html)
 - [Simplenote](http://simplenote.com//)
 
#### Download
You can download compiled binaries in [Releases](https://github.com/sepich/SynNotes/releases) section  
Changelog is available in [Commits](https://github.com/sepich/SynNotes/commits/master) section

#### Installation
Just unpack and use. App does not use registry and tries to keep db and ini-file config alongside with .exe file. If app dir is not writable (you unpacked it to `%ProgramFiles%`) then `%AppData%\SynNotes` directory used instead.

#### Configuration
That is very simple app and it will be so. No bells'n'whistles for reduced memory footprint (c# lol;). 

###### Sync
Simplenote sync is configured when you first press `Sync` button:  
![sync](https://habrastorage.org/files/129/d81/f54/129d81f549f246d19178c158011d5a34.png)  
Or at any time by Right-Clicking of that button. No any data exchange is performed when `Sync Frequency` set to `Manual` until you press the `Sync` button.

###### Settings
Some configuration could be done by editing `settings.ini`.

This part is for *Global Hotkeys* . Default is `Win+tilde` for Search, and no Hotkey assigned to just show app
```ini
[Keys]
HotkeyShow=
HotkeySearch=Win+`
```

App support themes of [Notepad++](http://notepad-plus-plus.org/) format which are stored in `/themes` folder. So, you can edit them or create new with visual editor of NP++. Then enable it like this:
```ini
[Scintilla]
Theme=Visual Studio Dark.xml
```

###### Lexers
 - When Note has no explicit Lexer (language to highlight syntax) selected it will inherit it from it's tags. 
 - If Note has multiple tags assigned, privilege has Tag which is higher in tree (you can arrange tags by drag'n'drop) In this case name of Lexer would be prefixed by ^
 - If both Note and all it's Tags has no Lexer assigned `bash` is used by default
