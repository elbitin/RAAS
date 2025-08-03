@echo off
call ..\setversions.bat
wix build -ext WixToolset.UI.wixext -ext WixToolset.Firewall.wixext -ext WixToolset.Util.wixext -ext WixToolset.Netfx.wixext -culture sl-Sl -loc RAASClient_sl.wxl RAASClient_x64.wxs  -out RAASClient_x64_sl.msi
pause
