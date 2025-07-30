@echo off
call ..\setversions.bat
wix build -ext WixToolset.UI.wixext -ext WixToolset.Firewall.wixext -ext WixToolset.Util.wixext -ext WixToolset.Netfx.wixext -culture sn -loc RAASServer_sn.wxl RAASServer_x64.wxs  -out RAASServer_x64_sn.msi
pause
