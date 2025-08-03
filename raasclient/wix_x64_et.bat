@echo off
call ..\setversions.bat
wix build -ext WixToolset.UI.wixext -ext WixToolset.Firewall.wixext -ext WixToolset.Util.wixext -ext WixToolset.Netfx.wixext -culture et-EE -loc RAASClient_et.wxl RAASClient_x64.wxs  -out RAASClient_x64_et.msi
pause
