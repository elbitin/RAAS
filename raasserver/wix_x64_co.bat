@echo off
call ..\setversions.bat
wix build -ext WixToolset.UI.wixext -ext WixToolset.Firewall.wixext -ext WixToolset.Util.wixext -ext WixToolset.Netfx.wixext -culture co -loc RAASServer_co.wxl RAASServer_x64.wxs  -out RAASServer_x64_co.msi
pause
