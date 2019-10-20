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

Install the .NET Core 3.0 Runtime.

Right-click to show the context menu and choose Install to register common file associations, File Explorer will then show a MediaInfo menu item in the context menu. For custom file extensions use [Open with++](https://github.com/stax76/OpenWithPlusPlus).

On a 32-Bit system download the DLL from the MediaInfo website, only the 64-Bit DLL is included.

## Usage

Open media files with the context menu in File Explorer or open files via drag & drop.

The search feature allows to quickly find properties.

F11/F12 navigates to the previous or next file in the folder.

## Settings

### font
Monospaced font: Consolas, Courier New or if installed Cascadia Code.

### raw-view
raw-view shows developers the parameter names as they are used in the MediaInfo API.