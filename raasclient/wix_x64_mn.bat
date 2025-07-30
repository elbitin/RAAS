@echo off
call ..\setversions.bat
wix build -ext WixToolset.UI.wixext -ext WixToolset.Firewall.wixext -ext WixToolset.Util.wixext -ext WixToolset.Netfx.wixext -culture mn -loc RAASClient_mn.wxl RAASClient_x64.wxs  -out RAASClient_x64_mn.msi
pause
