call ..\setversions.bat
@echo off
cls
msbuild.exe .\rmvshares\rmvshares.sln /t:Restore
msbuild.exe .\rmvshares\rmvshares.sln /t:Build /property:Configuration=Release /property:Platform=x64 /property:Deterministic=True
msbuild.exe .\keepalive\keepalive.sln /t:Restore
msbuild.exe .\keepalive\keepalive.sln /t:Build /property:Configuration=Release /property:Platform=x64 /property:Deterministic=True
msbuild.exe .\autostart\autostart.sln /t:Restore
msbuild.exe .\autostart\autostart.sln /t:Build /property:Configuration=Release /property:Platform=x64 /property:Deterministic=True
msbuild.exe .\shortcutssvc\shortcutssvc.sln /t:Restore
msbuild.exe .\shortcutssvc\shortcutssvc.sln /t:Build /property:Configuration=Release /property:Platform=x64 /property:Deterministic=True
msbuild.exe .\searchandrun\searchandrun.sln /t:Restore
msbuild.exe .\searchandrun\searchandrun.sln /t:Build /property:Configuration=Release /property:Deterministic=True
msbuild.exe .\raassrv\raassrv.sln /t:Restore
msbuild.exe .\raassrv\raassrv.sln /t:Build /property:Configuration=Release /property:Deterministic=True
msbuild.exe .\appnames\appnames.sln /t:Restore
msbuild.exe .\appnames\appnames.sln /t:Build /property:Configuration=Release /property:Deterministic=True
pause