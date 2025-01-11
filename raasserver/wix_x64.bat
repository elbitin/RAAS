@echo off
call ..\wixextensions.bat < nul
call wix_x64_en-US.bat < nul
call wix_x64_sv-SE.bat < nul
call wix_x64_merge.bat