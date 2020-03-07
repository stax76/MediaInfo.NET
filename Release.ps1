
$exePath = '.\src\bin\x86\Debug\MediaInfoNET.exe'
$version = [Diagnostics.FileVersionInfo]::GetVersionInfo($exePath).FileVersion
$targetDir = [Environment]::GetFolderPath('Desktop') + '\MediaInfo.NET-' + $version
Copy-Item .\src\bin\x86\Debug $targetDir -Recurse -Exclude *.runtimeconfig.dev.json
& 'C:\Program Files\7-Zip\7z.exe' a -t7z -mx9 "$targetDir.7z" -r "$targetDir\*"
