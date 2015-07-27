param($installPath, $toolsPath, $package, $project)

# removes import of AssemblyVersioning.targets from after.{solutionName}.sln.targets file

$assemblyVersioningLabel = $package

Write-Host "Uninstall called..."

$solution = Get-Interface $dte.Solution ([EnvDTE80.Solution2])
$solutionFileName = $solution.FileName
$arr = $solutionFileName.Split('\')
$solutionPath = $solutionFileName.Substring(0, $solution.FileName.LastIndexOf('\')) 
$solutionName = $arr[$arr.GetUpperBound(0)]

$afterTargetsFileName = "after.$solutionName.targets"
$afterTargetsFilePath = Join-Path $solutionPath $afterTargetsFileName
$afterTargetsExists = Test-Path $afterTargetsFilePath 

if (!($afterTargetsExists)) {
    Write-Debug "     Could not find $afterTargetsFilePath"
    return
}

$afterTargetsRoot = [Microsoft.Build.Construction.ProjectRootElement]::Open($afterTargetsFilePath)

Write-Host "     Uninstall looking for label $assemblyVersioningLabel"
foreach($pie in $afterTargetsRoot.Imports) {
    # see if it has the expected label
    if($pie -ne $null -and $pie.Label -ne $null -and $pie.Label.Trim() -ceq $assemblyVersioningLabel) {
        $pie.Condition = ('false')
        $afterTargetsRoot.Save() | Out-Null
        break
    }               
}
