@echo off
set config=%1
if "%config%" == "" (
   set config=Release
)

set version=0.5.4
if not "%PackageVersion%" == "" (
   set version=%PackageVersion%
)

set nuget=
if "%nuget%" == "" (
	set nuget=src\MicroFlow\.nuget\nuget.exe
)

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild" src\MicroFlow\MicroFlow.sln /t:Rebuild /p:Configuration="%config%" /m /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

set package_id="MicroFlow"

%nuget% pack "src\MicroFlow\MicroFlow.nuspec" -sym -NoPackageAnalysis -Version %version% -Properties "Configuration=%config%;PackageId=%package_id%"