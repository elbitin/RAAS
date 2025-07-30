@echo off
call ..\setversions.bat
wix build -ext WixToolset.UI.wixext -ext WixToolset.Firewall.wixext -ext WixToolset.Util.wixext -ext WixToolset.Netfx.wixext -culture ko -loc RAASClient_ko.wxl RAASClient_x64.wxs  -out RAASClient_x64_ko.msi
pause
