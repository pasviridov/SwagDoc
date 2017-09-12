@echo off

set SOLUTION=SwagDoc.sln
set VSPATH=C:\Program Files (x86)\Microsoft Visual Studio 14.0

set CONFIG=%1
if "%CONFIG%"=="" set CONFIG=Release

call "%VSPATH%\Common7\Tools\VsDevCmd.bat" x86
nuget restore
msbuild "%SOLUTION%" -p:Configuration="%CONFIG%";Platform="Any CPU"
