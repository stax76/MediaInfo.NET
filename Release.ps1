
$exePath = '.\src\bin\MediaInfoNET.exe'
$version = [Diagnostics.FileVersionInfo]::GetVersionInfo($exePath).FileVersion
$targetDir = [Environment]::GetFolderPath('Desktop') + '\MediaInfo.NET-' + $version
Copy-Item .\src\bin $targetDir -Recurse -Exclude System.Management.Automation.xml
& 'C:\Program Files\7-Zip\7z.exe' a -tzip -mx9 "$targetDir.zip" -r "$targetDir\*"
