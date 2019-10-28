
$targetDir = [Environment]::GetFolderPath('Desktop') + '\MediaInfo.NET'
New-Item $targetDir -ItemType Directory
Copy-Item .\src\bin\Debug\* $targetDir -Exclude *.runtimeconfig.dev.json
