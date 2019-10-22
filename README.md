# MediaInfo.NET

MediaInfo.NET is a alternative MediaInfo frontend.

![](Main.png)

## Features

- High DPI support
- A search/filter feature
- A tab bar showing each track in a dedicated tab
- Move to the next and previous file of the folder
- Raw view to show parameters as used in the MediaInfo API
- Dark User Interface

## Installation

**Install the .NET Core 3.0 Runtime.**

Run MediaInfo.NET and right-click to show the context menu, choose Install to register common file associations, File Explorer will then show a MediaInfo menu item in the context menu when a media file is right-clicked. For custom file extensions use [Open with++](https://github.com/stax76/OpenWithPlusPlus).

On a 32-Bit system download the DLL from the MediaInfo website, only the 64-Bit DLL is included.

## Usage

Open media files with the context menu in File Explorer or open files via drag & drop.

The search feature allows to quickly find properties.

F11/F12 navigates to the previous or next file in the folder.

## Settings

### font

A monospaced font.

### raw-view

raw-view shows developers the parameter names as they are used in the MediaInfo API.

### Defaults

font = consolas  
font-size = 14  
window-width = 750  
window-height = 550  
center-screen = yes  
raw-view = yes  
word-wrap = no  
