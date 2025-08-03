@echo off
call ..\setversions.bat
copy RAASClient_x64_en-US.msi RAASClient_%RAASClientVersion%_x64.msi
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_sv-SE.msi" -out sv-se.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_af.msi" -out af.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ar.msi" -out ar.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_az.msi" -out az.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_bg.msi" -out bg.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_bs.msi" -out bs.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_co.msi" -out co.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_cs.msi" -out cs.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_cy.msi" -out cy.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_de.msi" -out de.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_el.msi" -out el.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_es.msi" -out es.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_et.msi" -out et.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_eu.msi" -out eu.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_fi.msi" -out fi.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_fr.msi" -out fr.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ga.msi" -out ga.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_gd.msi" -out gd.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_gl.msi" -out gl.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_haw.msi" -out haw.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_hr.msi" -out hr.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ht.msi" -out ht.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_hu.msi" -out hu.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_id.msi" -out id.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ig.msi" -out ig.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_is.msi" -out is.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_it.msi" -out it.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ja.msi" -out ja.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ko.msi" -out ko.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_lt.msi" -out lt.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_lv.msi" -out lv.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_mi.msi" -out mi.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_mt.msi" -out mt.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_nl.msi" -out nl.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_no.msi" -out no.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_pl.msi" -out pl.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_pt-BR.msi" -out pt-BR.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_pt-PT.msi" -out pt-PT.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ro.msi" -out ro.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ru.msi" -out ru.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_sk.msi" -out sk.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_sn.msi" -out sn.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_sq.msi" -out sq.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_sw.msi" -out sw.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_th.msi" -out th.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_tl.msi" -out tl.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_tr.msi" -out tr.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_tt.msi" -out tt.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_vi.msi" -out vi.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_xh.msi" -out xh.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_zh-CN.msi" -out zh-CN.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_zh-TW.msi" -out zh-TW.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_zu.msi" -out zu.mst
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" sv-se.mst 1053
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ar.mst 1025
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" bg.mst 1026
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" zh-TW.mst 1028
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" cs.mst 1029
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" de.mst 1031
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" el.mst 1032
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" es.mst 1034
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" fi.mst 1035
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" fr.mst 1036
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" hu.mst 1038
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" is.mst 1039
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" it.mst 1040
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ja.mst 1041
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ko.mst 1042
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" nl.mst 1043
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" no.mst 1044
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" pl.mst 1045
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" pt-BR.mst 1046
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ro.mst 1048
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ru.mst 1049
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" hr.mst 1050
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" sk.mst 1051
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" sq.mst 1052
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" th.mst 1054
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" tr.mst 1055
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ur.mst 1056
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" id.mst 1057
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" et.mst 1061
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" lv.mst 1062
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" lt.mst 1063
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" vi.mst 1066
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" eu.mst 1069
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" zu.mst 1077
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" af.mst 1078
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" mt.mst 1082
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" sw.mst 1089
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" tt.mst 1092
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" cy.mst 1106
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" gl.mst 1110
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ne.mst 1121
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" tl.mst 1124
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ig.mst 1136
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" haw.mst 1141
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" mi.mst 1153
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" zh-CN.mst 2052
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" pt-PT.mst 2070
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ms.mst 2110
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" az.mst 2092
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" bs.mst 5146
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" co.mst 131
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ga.mst 2108
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" gd.mst 1169
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ht.mst 15372
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" sn.mst 1033
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" xh.mst 52
cscript ..\scripts\WiLangId.vbs "RAASClient_%RAASClientVersion%_x64.msi" Package 1033,1053,1053
#,1025,1026,1028,1029,1031,1032,1034,1035,1036,1038,1039,1040,1041,1042,1043,1044,1045,1046,1048,1049,1050,1051,1052,1054,1055,1056,1057,1061,1062,1063,1066,1069,1077,1078,1082,1089,1092,1106,1110,1121,1124,1136,1141,1153,2052,2070,2110,2092,5146,131,2108,1169,15372,4096,1033,52