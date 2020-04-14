
$exePath = '.\src\bin\MediaInfoNET.exe'
$version = [Diagnostics.FileVersionInfo]::GetVersionInfo($exePath).FileVersion
$targetDir = [Environment]::GetFolderPath('Desktop') + '\MediaInfo.NET-' + $version
Copy-Item .\src\bin $targetDir -Recurse -Exclude System.Management.Automation.xml
Copy-Item .\License.txt "$targetDir\License.txt"
& 'C:\Program Files\7-Zip\7z.exe' a -t7z -mx9 "$targetDir.7z" -r "$targetDir\*"
