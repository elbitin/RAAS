del raasclientcontextmenu\raasclientcontextmenu.package\bin\AnyCPU\Release\raasclient.msix
MakeAppx.exe pack /d raasclientcontextmenu\raasclientcontextmenu.package\bin\AnyCPU\Release /p raasclientcontextmenu\raasclientcontextmenu.package\bin\AnyCPU\Release\raasclient.msix /nv
SignTool.exe sign /debug /sha1 58f177ee5fda39aadcf499fa7fa06cab79dfe3a1 /tr http://timestamp.sectigo.com /td sha256 /fd sha256 /n "Elbitin" raasclientcontextmenu\raasclientcontextmenu.package\bin\AnyCPU\Release\raasclient.msix
pause