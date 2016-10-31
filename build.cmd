@echo off
set config=%1
if "%config%" == "" (
   set config=Release
)

set version=1.0.44
if not "%PackageVersion%" == "" (
   set version=%PackageVersion%
)

set nuget=
if "%nuget%" == "" (
	set nuget=tools\nuget.exe
)

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild" MicroFlow.sln /t:Rebuild /p:Configuration="%config%" /m /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

set package_id="MicroFlow"

%nuget% pack "MicroFlow.nuspec" -sym -NoPackageAnalysis -Version %version% -Properties "Configuration=%config%;PackageId=%package_id%"

set package_meta_id="MicroFlow.Meta"

%nuget% pack "MicroFlow.Meta.nuspec" -sym -NoPackageAnalysis -Version %version% -Properties "Configuration=%config%;PackageId=%package_meta_id%"
