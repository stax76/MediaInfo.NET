
MediaInfo.NET
=============

MediaInfo.NET is a Windows application that shows media file information.

![](img/Main.png)

![](img/Grid.png)


Features
--------

- High DPI support
- Search and highlight feature
- Tab bar showing each track in a dedicated tab
- Move to the next and previous file in the folder
- Raw view to show parameters as used in the MediaInfo API
- Customizable light and dark color theme
- Many settings to change the appearance
- Special presentation for encoding settings
- Compact summary
- Folder View powered by [Get-MediaInfo](https://github.com/stax76/Get-MediaInfo)
- Update check and PowerShell based update feature


Installation
------------

Run MediaInfo.NET and right-click to show the context menu, choose Install to register file associations, Windows File Explorer will then show a MediaInfo menu item in the context menu when a media file is right-clicked.

For power users there is [Open with++](https://github.com/stax76/OpenWithPlusPlus) for Windows File Explorer integration with icon support.

On x86 replace MediaInfo.dll with the x86 version found on the [MediaInfo website](https://mediaarea.net/en/MediaInfo).


Usage
-----

Open media files with the context menu in Windows File Explorer or via drag & drop or via context menu.

The theme colors are not hard coded but defined in Settings.xml.

Developers can enable raw view to show the parameter names as they are used in the MediaInfo API.

MediaInfo.NET registers itself in the registry at:

`HKCU\Software\Microsoft\Windows\CurrentVersion\App Paths\`

This enables third party apps to find and start MediaInfo.NET.


Support
-------

For bug report and feature requests the [issue tracker](../../issues) can be used.

For usage questions please use the [support thread](https://forum.videohelp.com/threads/394691-MediaInfo-NET) in the VideoHelp forum. This tool is not very popular so every feedback is welcome.


Related apps
------------

Find a list of related apps:

https://stax76.github.io/frankskare
