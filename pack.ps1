$ErrorActionPreference = "Stop"
$mainFolder = Resolve-Path (Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)
$nugetExe = "$mainFolder\.nuget\nuget.exe"
$msbuildExe = join-path -path (Get-ItemProperty "HKLM:\software\Microsoft\MSBuild\ToolsVersions\4.0").MSBuildToolsPath -childpath "msbuild.exe"

Remove-Item $mainFolder\Release -recurse -force -ErrorAction 0
Remove-Item $mainFolder\NET40 -recurse -force -ErrorAction 0
Remove-Item $mainFolder\*.nupkg
& "$nugetExe" restore $mainFolder\NConfiguration.sln

& "$msbuildExe" /m /target:"Clean,Build" /p:Configuration=Release /p:Platform="Any CPU" $mainFolder\NConfiguration.sln
& "$msbuildExe" /m /target:"Clean,Build" /p:Configuration=Release_NET40 /p:Platform="Any CPU" $mainFolder\NConfiguration.sln

& "$nugetExe" Pack $mainFolder/NConfiguration/NConfiguration.nuspec