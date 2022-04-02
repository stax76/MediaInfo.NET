
### 7.3.0.0

- Fix crash using relative media file paths.

### 7.2.0.0

- Next/Previous uses only known media file types as defined in the settings
- Larger list of known media file types
- Bit depth parameter is hidden for DTS
- Rebuild of corrupted settings file
- MediaInfo 22.03


### 7.1.0.0

- MediaInfo 21.3
- Get-MediaInfo 3.7
- psy-rd moved to Analysis section


### 7.0.0.0

- Long path support.
- Theme colors can be passed via CLI, StaxRip uses it.
- The compact summary on the basic tab shows more audio
  format info, for instance DTS XLL instead of only DTS.
- Fixed only one video stream shown in Basic view.


6.4.0.0
-------

- Update and Folder View feature did not work on systems
  with restricted PowerShell execution policy.


5.4
---

- the navigation tab bar was only showing the format family
  like DTS, now it's showing the exact format like DTS XLL
- new: update check, must first be enabled in settings
- new: update feature, requires PowerShell 5 and curl,
       an up to date Windows 10 system has both included

5.3
---

- in normal view language names are now shown instead of two letter language codes
- MediaInfo API spelling errors were corrected
- duplicated entries are now removed
- critical fix: file associations did not work when UserChoice key was defined

5.2
---

- in the settings dialog the layout was improved,
  the font picker was replaced with a drop down and
  a link was added to open the settings folder
- using raw view, MediaInfo is now queried only once
  instead of twice which is marginally more efficient
- on startup MediaInfo.NET registers itself at
  HKCU\Software\Microsoft\Windows\CurrentVersion\App Paths\
  which enables third party apps to find and start MediaInfo.NET
- a assembly title attribute was added so the shell will use
  MediaInfo.NET instead of MediaInfoNET (for instance in the
  Explorer Open with menu)
- the website was improved and added to the context menu
- the about dialog shows now the MediaInfo version
- all message boxes were migrated to use the TaskDialog API
  which has a improved presentation and copy and supports
  links in case of an error

5.1
---

- new setting 'Theme' added with 'Light', 'Dark' and 'System' option,
  theme colors are not hard coded but defined in Settings.xml
- fix for window starting in background on very first start
- file association uninstall scans now all extensions and not only
  the ones that are defined in the settings
- encoding settings are now alphabetically ordered
- compact summary is no longer limited to raw view, it's now
  also available in normal view
 
5.0
---

- changelog added to repo
- if no file is open message is shown
- file can be opened from menu: Open File...
- file can be saved from menu: Save File...
- exclude setting added to hide defined parameters
- encoding presentation can be disabled
- compact summary can be disabled
- file types for setup can be customized
- column width can be customized
- new settings dialog with proper UI controls and greatly improved usability
- some dialogs use task dialog instead of msg box
- about dialog added
- settings directory can either be AppData, Portable or Custom
