@echo off
call ..\setversions.bat
wix build -ext WixToolset.UI.wixext -ext WixToolset.Firewall.wixext -ext WixToolset.Util.wixext -ext WixToolset.Netfx.wixext -culture ja-jp -loc RAASServer_ja.wxl RAASServer_x64.wxs  -out RAASServer_x64_ja.msi
pause
