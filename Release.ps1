
$targetDir = [Environment]::GetFolderPath('Desktop') + '\MediaInfo.NET'
New-Item -Path $targetDir -ItemType Directory
Copy-Item .\src\bin\Debug\* $targetDir -Exclude *.runtimeconfig.dev.json
