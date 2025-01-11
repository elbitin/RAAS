call ..\setversions.bat
copy RAASServer_x64_en-US.msi RAASServer_%RAASServerVersion%_x64.msi
wix msi transform -t language "RAASServer_%RAASClientVersion%_x64.msi" "RAASServer_x64_sv-SE.msi" -out sv-se.mst
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" sv-se.mst 1053
cscript ..\scripts\WiLangId.vbs "RAASServer_%RAASServerVersion%_x64.msi" Package 1033,1053
pause