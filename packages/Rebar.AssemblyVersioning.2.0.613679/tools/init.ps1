param($installPath, $toolsPath, $package, $project)

# adds after.{solutionName}.sln.targets file if it doesn't exist
# adds import of AssemblyVersioning.targets file to above target file

$assemblyVersioningLabel = $package
$assemblyVersioningTargetsFilePath = Join-Path "tools" "AssemblyVersioning.targets"
$afterTargetsFilePath = ""

function DoesAfterTargetsHaveImport {
    param($afterTargetsRoot)
    
    $hasImport = $false
    foreach($pie in $afterTargetsRoot.Imports) {
        # see if it has the expected label
        if($pie -ne $null -and $pie.Label -ne $null -and $pie.Label.Trim() -ceq $assemblyVersioningLabel) {
            if ($pie.Condition -ne 'false') { 
                $hasImport = $true 
            }
            break
        }               
    }
    
    #Write-Host "DoesAfterTargetsHaveImport returns $hasImport"
    return $hasImport
}

function AddImportToAfterTargets {
    param($afterTargetsRoot)
   
    $packageFolder = Split-Path $(Split-Path $toolsPath -parent) -leaf
    $packagePath = Join-Path "packages" $packageFolder
    $relativeTargetsPath = Join-Path $packagePath $assemblyVersioningTargetsFilePath
    
    # find it if it already exists, otherwise create it
    if ($afterTargetsRoot.Imports -ne $null) {
        foreach($pie in $afterTargetsRoot.Imports) {
            # see if it has the expected label
            if($pie -ne $null -and $pie.Label -ne $null -and $pie.Label.Trim() -ceq $assemblyVersioningLabel) {
                $importElement = $pie
            }
        }
    }

    if ($importElement -eq $null) {
        $importElement = $afterTargetsRoot.AddImport($assemblyVersioningLabel)
    }

    # set import values
    $importElement.Project = $relativeTargetsPath
    $importElement.Label = $assemblyVersioningLabel
    $importElement.Condition= ("Exists('{0}')" -f $relativeTargetsPath)
    $afterTargetsRoot.Save() | Out-Null
}

function CreateAfterTargetsFile {    
    $solution = Get-Interface $dte.Solution ([EnvDTE80.Solution2])
    $solutionFileName = $solution.FileName
    $arr = $solutionFileName.Split('\')
    $solutionPath = $solutionFileName.Substring(0, $solution.FileName.LastIndexOf('\')) 
    $solutionName = $arr[$arr.GetUpperBound(0)]

    $afterTargetsTemplateFilePath = Join-Path $toolsPath "after._solutionName_.sln.targets"

    $afterTargetsFileName = "after.$solutionName.targets"
    $afterTargetsFilePath = Join-Path $solutionPath $afterTargetsFileName
    $afterTargetsExists = Test-Path $afterTargetsFilePath 

    if(!($afterTargetsExists)) {
        Write-Debug ("    Creating {0}" -f $afterTargetsFilePath) | Out-Null
        # create a new file there 
        Copy-Item $afterTargetsTemplateFilePath $afterTargetsFilePath
    }
    
    $afterTargetsRoot = [Microsoft.Build.Construction.ProjectRootElement]::Open($afterTargetsFilePath)
    # now we need to see if the file has the import that we are looking to add
    $afterTargetsHasImport = DoesAfterTargetsHaveImport -afterTargetsRoot $afterTargetsRoot

    if(!($afterTargetsHasImport)) {
        # we need to add an import to that file now
        AddImportToAfterTargets -afterTargetsRoot $afterTargetsRoot
    }
}

CreateAfterTargetsFile