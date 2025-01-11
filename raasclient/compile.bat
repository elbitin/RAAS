@echo off
call ..\setversions.bat
msbuild.exe .\remotesc\remotesc.sln /t:Restore
msbuild.exe .\remotesc\remotesc.sln /t:Build /property:Configuration=Release /property:Deterministic=True
msbuild.exe .\rdesktop\rdesktop.sln /t:Restore /property:Configuration=Release /property:Platform=x64
msbuild.exe .\rdesktop\rdesktop.sln /t:Build /property:Configuration=Release /property:Deterministic=True /property:Platform=x64
msbuild.exe .\servercfg\servercfg.sln /t:Restore
msbuild.exe .\servercfg\servercfg.sln /t:Build /property:Configuration=Release /property:Deterministic=True
msbuild.exe .\openremote\openremote.sln /t:Restore
msbuild.exe .\openremote\openremote.sln /t:Build /property:Configuration=Release /property:Deterministic=True
msbuild.exe .\connectshares\connectshares.sln /t:Restore
msbuild.exe .\connectshares\connectshares.sln /t:Build /property:Configuration=Release /property:Deterministic=True
msbuild.exe .\connectsharesadmin\connectsharesadmin.sln /t:Restore
msbuild.exe .\connectsharesadmin\connectsharesadmin.sln /t:Build /property:Configuration=Release /property:Deterministic=True
msbuild.exe .\shortcuts\shortcuts.sln /t:Restore
msbuild.exe .\shortcuts\shortcuts.sln /t:Build /property:Configuration=Release /property:Deterministic=True
msbuild.exe .\rapps\rapps.sln /t:Restore
msbuild.exe .\rapps\rapps.sln /t:Build /property:Configuration=Release /property:Deterministic=True
msbuild.exe .\raasclient\raasclient.sln /t:Restore
msbuild.exe .\raasclient\raasclient.sln /t:Build /property:Configuration=Release /property:Deterministic=True
msbuild.exe .\cmenuh\cmenuh.sln /t:Build /property:Configuration=Release /property:Platform=x64 /property:Deterministic=True
msbuild.exe .\cmenuh\cmenuh.sln /t:Build /property:Configuration=Release /property:Platform=Win32 /property:Deterministic=True
msbuild.exe .\dmenuh\dmenuh.sln /t:Build /property:Configuration=Release /property:Platform=x64 /property:Deterministic=True
msbuild.exe .\dmenuh\dmenuh.sln /t:Build /property:Configuration=Release /property:Platform=Win32 /property:Deterministic=True
msbuild.exe .\nsext\nsext.sln /t:Build /property:Configuration=Release /property:Platform=x64 /property:Deterministic=True
msbuild.exe .\nsext\nsext.sln /t:Build /property:Configuration=Release /property:Platform=Win32 /property:Deterministic=True
msbuild.exe .\raasclientcontextmenu\raasclientcontextmenu.sln /t:Build /property:Configuration=Release /property:Platform="Any CPU" /property:Deterministic=True /p:AppxPackageSigningEnabled=false
msbuild.exe .\customactions\customactions.sln /t:Restore /property:Configuration=Release /property:Platform=x86
msbuild.exe .\customactions\customactions.sln /t:Build /property:Configuration=Release /property:Platform=x86 /property:Deterministic=True
pause