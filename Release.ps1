
$targetDir = [Environment]::GetFolderPath('Desktop') + '\MediaInfo.NET'
New-Item -Path $targetDir -ItemType Directory
Copy-Item 'D:\Projekte\CS\mpv.net\mpv.net\bin\x64\MediaInfo.dll' "$targetDir\MediaInfo.dll"
Copy-Item .\src\bin\Debug\* $targetDir
