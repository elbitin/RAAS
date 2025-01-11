call ..\setversions.bat
@echo on
cls
msbuild.exe .\nsext\nsext.sln /t:Restore
msbuild.exe .\nsext\nsext.sln /t:Build /property:Configuration=Release /property:Platform=x64
.\nsext\x64\Release\tests.exe
msbuild.exe .\nsext\nsext.sln /t:Restore
msbuild.exe .\nsext\nsext.sln /t:Build /property:Configuration=Release /property:Platform=Win32
.\nsext\Release\tests.exe
pause