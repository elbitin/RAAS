@echo off
call ..\setversions.bat
wix build -ext WixToolset.UI.wixext -ext WixToolset.Firewall.wixext -ext WixToolset.Util.wixext -ext WixToolset.Netfx.wixext -culture pt-BR -loc RAASClient_pt-BR.wxl RAASClient_x64.wxs  -out RAASClient_x64_pt-BR.msi
pause
