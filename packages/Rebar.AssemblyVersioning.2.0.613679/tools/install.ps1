param($rootPath, $toolsPath, $package, $project)

# remove the ignore_me.txt file, which is only there to satisfy NuGet requirements
$dummyFile = "ignore_me.txt" 
$project.ProjectItems | ForEach { if ($_.Name -eq $dummyFile) { $_.Remove() } }
$projectPath = Split-Path $project.FullName -Parent
Join-Path $projectPath $dummyFile | Remove-Item