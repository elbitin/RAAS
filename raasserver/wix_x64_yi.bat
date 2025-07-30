@echo off
call ..\setversions.bat
wix build -ext WixToolset.UI.wixext -ext WixToolset.Firewall.wixext -ext WixToolset.Util.wixext -ext WixToolset.Netfx.wixext -culture yi -loc RAASServer_yi.wxl RAASServer_x64.wxs  -out RAASServer_x64_yi.msi
pause
