@echo off
call ..\setversions.bat
wix build -ext WixToolset.UI.wixext -ext WixToolset.Firewall.wixext -ext WixToolset.Util.wixext -ext WixToolset.Netfx.wixext -culture sn -loc RAASClient_sn.wxl RAASClient_x64.wxs  -out RAASClient_x64_sn.msi
pause
