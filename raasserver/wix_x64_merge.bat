@echo off
call ..\setversions.bat
copy RAASServer_x64_en-US.msi RAASServer_%RAASServerVersion%_x64.msi
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_sv-SE.msi" -out sv-se.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_ar.msi" -out ar.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_bg.msi" -out bg.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_bs.msi" -out bs.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_cs.msi" -out cs.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_de.msi" -out de.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_el.msi" -out el.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_es.msi" -out es.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_et.msi" -out et.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_fi.msi" -out fi.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_fr.msi" -out fr.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_hr.msi" -out hr.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_hu.msi" -out hu.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_it.msi" -out it.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_ja.msi" -out ja.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_ko.msi" -out ko.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_lt.msi" -out lt.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_lv.msi" -out lv.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_nl.msi" -out nl.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_nl.msi" -out nb-NO.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_pt-BR.msi" -out pt-BR.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_pt-PT.msi" -out pt-PT.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_ru.msi" -out ru.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_sk.msi" -out sk.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_sn.msi" -out sn.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_sq.msi" -out sq.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_th.msi" -out th.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_tl.msi" -out tl.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_tr.msi" -out tr.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_zh-CN.msi" -out zh-CN.mst
wix msi transform -t language "RAASServer_%RAASServerVersion%_x64.msi" "RAASServer_x64_zh-TW.msi" -out zh-TW.mst
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" sv-se.mst 1053
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" ar.mst 1025
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" bg.mst 1026
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" zh-TW.mst 1028
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" cs.mst 1029
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" de.mst 1031
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" el.mst 1032
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" es.mst 1034
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" fi.mst 1035
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" fr.mst 1036
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" hu.mst 1038
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" it.mst 1040
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" ja.mst 1041
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" ko.mst 1042
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" nl.mst 1043
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" nb-NO.mst 1044
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" pt-BR.mst 1046
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" ru.mst 1049
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" hr.mst 1050
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" sk.mst 1051
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" sq.mst 1052
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" th.mst 1054
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" tr.mst 1055
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" et.mst 1061
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" lv.mst 1062
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" lt.mst 1063
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" vi.mst 1066
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" mt.mst 1082
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" ne.mst 1121
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" tl.mst 1124
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" zh-CN.mst 2052
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" pt-PT.mst 2070
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" bs.mst 5146
cscript ..\scripts\WiSubStg.vbs "RAASServer_%RAASServerVersion%_x64.msi" sn.mst 1033
cscript ..\scripts\WiLangId.vbs "RAASServer_%RAASServerVersion%_x64.msi" Package 1033,1053,1053,1025,1026,1028,1029,1031,1032,1034,1035,1036,1038,1040,1041,1042,1043,1044,1046,1049,1050,1051,1054,1055,1061,1062,1063,1121,1124,2052,2070,5146,4096,1033,52
pause