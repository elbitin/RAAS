call ..\setversions.bat
@echo off
cls
msbuild.exe .\raasclientcontextmenu\raasclientcontextmenu.sln /t:Clean /property:Configuration=Release
msbuild.exe .\customactions\customactions.sln /t:Clean /property:Configuration=Release /property:Platform=x86
del .\raasclientcontextmenu\raasclientcontextmenu.package\bin\AnyCPU\Release\raasclient.msix
del .\raasclientcontextmenu\raasclientcontextmenu.package\openonremotehostcommandverb.dll
msbuild.exe .\raasclient\raasclient.sln /t:Clean /property:Configuration=Release
msbuild.exe .\remotesc\remotesc.sln /t:Clean /property:Configuration=Release
msbuild.exe .\servercfg\servercfg.sln /t:Clean /property:Configuration=Release
msbuild.exe .\rdesktop\rdesktop.sln /t:Clean /property:Configuration=Release /property:Platform=x64
msbuild.exe .\connectshares\connectshares.sln /t:Clean /property:Configuration=Release
msbuild.exe .\connectsharesadmin\connectsharesadmin.sln /t:Clean /property:Configuration=Release
msbuild.exe .\shortcuts\shortcuts.sln /t:Clean /property:Configuration=Release
msbuild.exe .\openremote\openremote.sln /t:Clean /property:Configuration=Release
msbuild.exe .\cmenuh\cmenuh.sln /t:Clean /property:Configuration=Release /property:Platform=x64
msbuild.exe .\cmenuh\cmenuh.sln /t:Clean /property:Configuration=Release /property:Platform=Win32
msbuild.exe .\dmenuh\dmenuh.sln /t:Clean /property:Configuration=Release /property:Platform=x64
msbuild.exe .\dmenuh\dmenuh.sln /t:Clean /property:Configuration=Release /property:Platform=Win32
msbuild.exe .\nsext\nsext.sln /t:Clean /property:Configuration=Release /property:Platform=x64
msbuild.exe .\nsext\nsext.sln /t:Clean /property:Configuration=Release /property:Platform=x86
if exist .\RAAS_x64.msi del .\RAAS_x64.msi
pause