call ..\setversions.bat
@echo off
cls
msbuild.exe .\rmvshares\rmvshares.sln /t:Clean /property:Configuration=Release /property:Platform=x64 /property:Deterministic=True
msbuild.exe .\keepalive\keepalive.sln /t:Clean /property:Configuration=Release /property:Platform=x64 /property:Deterministic=True
msbuild.exe .\autostart\autostart.sln /t:Clean /property:Configuration=Release /property:Platform=x64 /property:Deterministic=True
msbuild.exe .\shortcutssvc\shortcutssvc.sln /t:Clean /property:Configuration=Release /property:Platform=x64 /property:Deterministic=True
msbuild.exe .\searchandrun\searchandrun.sln /t:Clean /property:Configuration=Release /property:Deterministic=True
msbuild.exe .\raassrv\raassrv.sln /t:Clean /property:Configuration=Release /property:Deterministic=True
msbuild.exe .\appnames\appnames.sln /t:Clean /property:Configuration=Release /property:Deterministic=True
if exist .\RAASServer_x64.msi del .\RAASServer_x64.msi
pause