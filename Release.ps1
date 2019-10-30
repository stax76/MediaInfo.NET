
$exePath = '.\src\bin\Debug\MediaInfoNET.exe'
$version = [Diagnostics.FileVersionInfo]::GetVersionInfo($exePath).FileVersion.TrimEnd('0.'.ToCharArray())
$targetDir = [Environment]::GetFolderPath('Desktop') + '\MediaInfo.NET-' + $version
New-Item $targetDir -ItemType Directory
Copy-Item .\src\bin\Debug\* $targetDir -Exclude *.runtimeconfig.dev.json
& "C:\Program Files\7-Zip\7z.exe" a -t7z -mx9 "$targetDir.7z" -r "$targetDir\*"
