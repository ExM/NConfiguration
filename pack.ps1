$ErrorActionPreference = "Stop"
$mainFolder = Resolve-Path (Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)
$nugetExe = "$mainFolder\packages\NuGet.CommandLine.2.8.5\tools\nuget.exe"
$msbuildExe = join-path -path (Get-ItemProperty "HKLM:\software\Microsoft\MSBuild\ToolsVersions\4.0").MSBuildToolsPath -childpath "msbuild.exe"

Remove-Item $mainFolder\*.nupkg
& "$msbuildExe" /m /target:"Clean;Build" /p:Configuration=Release /p:Platform="Any CPU" $mainFolder\NConfiguration.sln
& "$nugetExe" Pack $mainFolder/NuGet/NConfiguration.nuspec
