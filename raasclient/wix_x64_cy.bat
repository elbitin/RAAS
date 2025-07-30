@echo off
call ..\setversions.bat
wix build -ext WixToolset.UI.wixext -ext WixToolset.Firewall.wixext -ext WixToolset.Util.wixext -ext WixToolset.Netfx.wixext -culture cy -loc RAASClient_cy.wxl RAASClient_x64.wxs  -out RAASClient_x64_cy.msi
pause
