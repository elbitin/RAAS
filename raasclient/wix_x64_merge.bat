@echo off
call ..\setversions.bat
copy RAASClient_x64_en-US.msi RAASClient_%RAASClientVersion%_x64.msi
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_sv-SE.msi" -out sv-se.mst
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" sv-se.mst 1053
cscript ..\scripts\WiLangId.vbs "RAASClient_%RAASClientVersion%_x64.msi" Package 1033,1053
pause