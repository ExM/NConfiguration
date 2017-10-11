$ErrorActionPreference = "Stop"
$mainFolder = Resolve-Path (Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)
$paketExe = "$mainFolder\.paket\paket.exe"
$msbuildExe = join-path -path (Get-ItemProperty "HKLM:\software\Microsoft\MSBuild\ToolsVersions\4.0").MSBuildToolsPath -childpath "msbuild.exe"

Remove-Item $mainFolder\Release -recurse -force -ErrorAction 0
& "$paketExe" restore
& "$msbuildExe" /m /target:"Clean,Build" /p:Configuration=Release /p:Platform="Any CPU" $mainFolder\NConfiguration.sln
& "$msbuildExe" /m /target:"Clean,Build" /p:Configuration=Release_NET40 /p:Platform="Any CPU" $mainFolder\NConfiguration.sln
& "$paketExe" pack --template packet.template .\Release