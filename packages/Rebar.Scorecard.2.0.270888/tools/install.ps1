param($installPath, $toolsPath, $package, $project)

function Resolve-ProjectName{
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

function Get-MSBuildProperty {
    param(
        [parameter(Position = 0, Mandatory = $true)]
        $PropertyName
    )
    
    $buildProject = Get-MSBuildProject $ProjectName
    $buildProject.GetProperty($PropertyName)
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

function Add-SolutionDirProperty { 
        $path = $ProjectName.FullName
        $buildProject =@([Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($path))[0]
        
         if(!($buildProject.Xml.Properties | ?{ $_.Name -eq 'SolutionDir' })) {
            # Get the relative path to the solution
            $relativeSolutionPath = [NuGet.PathUtility]::GetRelativePath($project.FullName, $dte.Solution.Properties.Item("Path").Value)
            $relativeSolutionPath = [IO.Path]::GetDirectoryName($relativeSolutionPath)
            $relativeSolutionPath = [NuGet.PathUtility]::EnsureTrailingSlash($relativeSolutionPath)
            
            $solutionDirProperty = $buildProject.Xml.AddProperty("SolutionDir", $relativeSolutionPath)
            $solutionDirProperty.Condition = '$(SolutionDir) == '''' Or $(SolutionDir) == ''*Undefined*'''
            $_.Save()
     }
 }
$solution = Get-Interface $dte.Solution ([EnvDTE80.Solution2])
$ProjectName = $solution.Projects | ?{$_.ProjectName -eq $project}

$project.Save()

# Add-SolutionDirProperty
$path = $project.FullName
$buildProject =@([Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($path))[0]
if(!($buildProject.Xml.Properties | ?{ $_.Name -eq 'SolutionDir' })) {
    $relativeSolutionPath = [NuGet.PathUtility]::GetRelativePath($project.FullName, $dte.Solution.Properties.Item("Path").Value)
    $relativeSolutionPath = [IO.Path]::GetDirectoryName($relativeSolutionPath)
    $relativeSolutionPath = [NuGet.PathUtility]::EnsureTrailingSlash($relativeSolutionPath)

    $solutionDirProperty = $buildProject.Xml.AddProperty("SolutionDir", $relativeSolutionPath)
    $solutionDirProperty.Condition = '$(SolutionDir) == '''' Or $(SolutionDir) == ''*Undefined*'''
    $project.Save()

}

$relativeSolutionPath = [NuGet.PathUtility]::GetRelativePath($project.FullName, $dte.Solution.Properties.Item("Path").Value)
$relativeSolutionPath = [IO.Path]::GetDirectoryName($relativeSolutionPath)
$relativeSolutionPath = [NuGet.PathUtility]::EnsureTrailingSlash($relativeSolutionPath)

#$slnPath = $relativeSolutionPath.Replace("..\", "") +'packages\'+$package+'\tools\REBARScorecard.ruleset'
#$rulesetPath = '$(SolutionDir)'+ $slnPath
$slnPath = $relativeSolutionPath.Replace("..\", "") +'packages\'+$package.Id+'.'+$package.Version+'\tools\REBARScorecard.ruleset'
$rulesetPath = '$(SolutionDir)'+ $slnPath

#Set-MSBuildProperty 'CodeAnalysisRuleSet' $rulesetPath

if ($buildProject.ConditionedProperties.Configuration -contains 'Release') {
    # $propertyGroupCondition = " '$" + "(Configuration)' != 'Debug' "
    $propertyGroupCondition = " '$" + "(Configuration)" + '|' + '$' + "(Platform)' == 'Release|AnyCPU' "
    $buildProject = Get-MSBuildProject $ProjectName
    $releasePropGroup = $buildProject.Xml.PropertyGroups | ?{ $_.Condition -eq $propertyGroupCondition }

    $releasePropGroup.SetProperty("RunCodeAnalysis", $true)
    $releasePropGroup.SetProperty("CodeAnalysisRuleSet", $rulesetPath)
    $releasePropGroup.SetProperty("TreatWarningsAsErrors", $true)
    $releasePropGroup.SetProperty("DefineConstants", "TRACE;CODE_ANALYSIS")
    $project.Save()
}

# Get the open solution.
$solution = Get-Interface $dte.Solution ([EnvDTE80.Solution2])
$solutionItems = "Solution Items"

# If solution items folder doesn't exist, create it
$solutionFolder = $solution.Projects | ?{$_.ProjectName -eq 'Solution Items'}
if ($solutionFolder -eq $null) {
    $solution.AddSolutionFolder($solutionItems)
}

$solutionFolder = $solution.Projects | ?{$_.ProjectName -eq 'Solution Items'}
$solutionItemsFolder = $solutionFolder.ProjectItems

# get the files in the Tools folder and install them
$toolsPath = (Split-Path -parent $MyInvocation.MyCommand.Definition)
$files = Get-ChildItem $toolsPath -Exclude *.ps1

# Only add the files if they already exist
$files |% { $file = $_; if (! ($solutionItemsFolder |? Name -eq $file.Name)) { $solutionItemsFolder.AddFromFile( $file.Fullname ) } }

# remove the ignore_me.txt file, which is only there to satisfy NuGet requirements
$dummyFile = "ignore_me.txt" 
$project.ProjectItems | ForEach { if ($_.Name -eq $dummyFile) { $_.Remove() } }
$projectPath = Split-Path $project.FullName -Parent
Join-Path $projectPath $dummyFile | Remove-Item
