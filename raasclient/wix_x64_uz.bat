@echo off
call ..\setversions.bat
wix build -ext WixToolset.UI.wixext -ext WixToolset.Firewall.wixext -ext WixToolset.Util.wixext -ext WixToolset.Netfx.wixext -culture uz -loc RAASClient_uz.wxl RAASClient_x64.wxs  -out RAASClient_x64_uz.msi
pause
