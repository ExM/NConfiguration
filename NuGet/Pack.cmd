%~d0
cd %~dp0
%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe /m /target:Build /p:Configuration=Release /p:Platform="Any CPU" ..\NConfiguration.sln

del *.nupkg

NuGet.exe Pack NConfiguration.nuspec
