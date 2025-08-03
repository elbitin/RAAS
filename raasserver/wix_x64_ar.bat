@echo off
call ..\setversions.bat
wix build -ext WixToolset.UI.wixext -ext WixToolset.Firewall.wixext -ext WixToolset.Util.wixext -ext WixToolset.Netfx.wixext -culture ar-SA -loc RAASServer_ar.wxl RAASServer_x64.wxs  -out RAASServer_x64_ar.msi
pause
