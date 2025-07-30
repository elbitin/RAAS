@echo off
call ..\setversions.bat
wix build -ext WixToolset.UI.wixext -ext WixToolset.Firewall.wixext -ext WixToolset.Util.wixext -ext WixToolset.Netfx.wixext -culture nl -loc RAASClient_nl.wxl RAASClient_x64.wxs  -out RAASClient_x64_nl.msi
pause
