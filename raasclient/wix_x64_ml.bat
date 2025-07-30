@echo off
call ..\setversions.bat
wix build -ext WixToolset.UI.wixext -ext WixToolset.Firewall.wixext -ext WixToolset.Util.wixext -ext WixToolset.Netfx.wixext -culture ml -loc RAASClient_ml.wxl RAASClient_x64.wxs  -out RAASClient_x64_ml.msi
pause
