param($installPath, $toolsPath, $package, $project)

function Resolve-ProjectName {
    param(
        [parameter(ValueFromPipelineByPropertyName = $true)]
        [string[]]$ProjectName
    )
    
    if($ProjectName) {
        $projects = Get-Project $ProjectName
    }
    else {
        # All projects by default
        $projects = Get-Project
    }
    
    $projects
}

function Get-MSBuildProject {
    param(
        [parameter(ValueFromPipelineByPropertyName = $true)]
        [string[]]$ProjectName
    )
    Process {
        (Resolve-ProjectName $ProjectName) | % {
            $path = $_.FullName
            @([Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($path))[0]
        }
    }
}

function Set-MSBuildProperty {
    param(
        [parameter(Position = 0, Mandatory = $true)]
        $PropertyName,
        [parameter(Position = 1, Mandatory = $true)]
        $PropertyValue
    )
    Process {
        (Resolve-ProjectName $ProjectName) | %{
            $buildProject = $_ | Get-MSBuildProject
            $buildProject.SetProperty($PropertyName, $PropertyValue) | Out-Null
            $_.Save()
        }
    }
}

$solution = Get-Interface $dte.Solution ([EnvDTE80.Solution2])

$ProjectName = $solution.Projects | ?{$_.ProjectName -eq $project}

$solutionFolder = $solution.Projects | ?{$_.ProjectName -eq 'Solution Items'}
$solutionItemsFolder = $solutionFolder.ProjectItems

# get the files in the Tools folder and install them
$toolsPath = (Split-Path -parent $MyInvocation.MyCommand.Definition)
$files = Get-ChildItem $toolsPath -Exclude *.ps1

$files | foreach { (Get-Interface $solutionItemsFolder.Item($_.Name) "EnvDTE.ProjectItem").Delete() } 

if ($buildProject.ConditionedProperties.Configuration -contains 'Release') {
    # $propertyGroupCondition = " '$" + "(Configuration)' != 'Debug' "
    $propertyGroupCondition = " '$" + "(Configuration)" + '|' + '$' + "(Platform)' == 'Release|AnyCPU' "
    $buildProject = Get-MSBuildProject $ProjectName
    $releasePropGroup = $buildProject.Xml.PropertyGroups | ?{ $_.Condition -eq $propertyGroupCondition }

    $releasePropGroup.SetProperty("RunCodeAnalysis", "false")
    $releasePropGroup.SetProperty("CodeAnalysisRuleSet", "")
    $project.Save()
}
