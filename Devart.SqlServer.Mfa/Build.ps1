function Get-Version {
    param (
        [String] $VersionFile,
        [Int] $BuildNumber
    )
    $text = Get-Content -Path $VersionFile
    $versionText = [Regex]::new('AssemblyVersion[(].(\d.\d)').Match($text).Groups[1].Value
    $version = [Version]::new($versionText)
    return "{0}.{1}.{2}" -f $version.Major, $version.Minor, $BuildNumber
}

function Push-Package {
    param (
        [String] $PackageFile,
        [String] $NexusUrl,
        [String] $NexusApiKey
    )
    Write-Output "Push package: '$PackageFile'" 
    dotnet nuget push $PackageFile --source $NexusUrl --api-key $NexusApiKey
}

function Pack-Package {
    param (
        [String] $NuspecFile,
        [String] $Version
    )
    Write-Output "Create package: '$NuspecFile' ($Version)"
    nuget pack $NuspecFile -Version $Version
}

# Environment Vars
$nexusApiKey = $env:DBFORGEBUILD_DBFNEXUS_API_KEY
$nexusUrl = $env:DBFORGENUGET_NEXUS_REPOSITORY_URL
$buildNumber = 0

# vars
$nugetPackageName = "Devart.SqlServer.Mfa"
$nuspecFileName = "$nugetPackageName.nuspec"
$versionFile = Resolve-Path -Path 'version.cs'
$version = Get-Version -VersionFile $versionFile -BuildNumber $buildNumber
$nugetPackageFile = "{0}.{1}.nupkg" -f $nugetPackageName, $version


Pack-Package -NuspecFile $nuspecFileName -Version $version
Push-Package -PackageFile $nugetPackageFile -NexusUrl $nexusUrl -NexusApiKey $nexusApiKey