
$targetDir = [Environment]::GetFolderPath('Desktop') + '\MediaInfo.NET'
New-Item -Path $targetDir -ItemType Directory
Copy-Item 'D:\Projekte\CS\mpv.net\mpv.net\bin\x64\MediaInfo.dll' "$targetDir\MediaInfo.dll"
Copy-Item .\src\bin\MediaInfoNET.exe "$targetDir\MediaInfoNET.exe"
Copy-Item .\src\bin\MediaInfoNET.exe.config "$targetDir\MediaInfoNET.exe.config"
Copy-Item .\src\bin\MediaInfoNET.pdb "$targetDir\MediaInfoNET.pdb"
