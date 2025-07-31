@echo off
call ..\setversions.bat
copy RAASClient_x64_en-US.msi RAASClient_%RAASClientVersion%_x64.msi
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_sv-SE.msi" -out sv-se.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_af.msi" -out af.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_am.msi" -out am.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ar.msi" -out ar.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_az.msi" -out az.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_be.msi" -out be.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_bg.msi" -out bg.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_bs.msi" -out bs.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ca.msi" -out ca.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_co.msi" -out co.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_cs.msi" -out cs.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_cy.msi" -out cy.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_da.msi" -out da.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_de.msi" -out de.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_el.msi" -out el.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_en.msi" -out en.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_eo.msi" -out eo.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_es.msi" -out es.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_et.msi" -out et.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_eu.msi" -out eu.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_fa.msi" -out fa.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_fi.msi" -out fi.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_fr.msi" -out fr.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ga.msi" -out ga.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_gd.msi" -out gd.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_gl.msi" -out gl.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_gu.msi" -out gu.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ha.msi" -out ha.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_haw.msi" -out haw.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_hi.msi" -out hi.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_hr.msi" -out hr.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ht.msi" -out ht.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_hu.msi" -out hu.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_hy.msi" -out hy.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_id.msi" -out id.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ig.msi" -out ig.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_is.msi" -out is.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_it.msi" -out it.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_iw.msi" -out iw.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ja.msi" -out ja.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_jw.msi" -out jw.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ka.msi" -out ka.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_kk.msi" -out kk.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_km.msi" -out km.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_kn.msi" -out kn.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ko.msi" -out ko.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ku.msi" -out ku.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ky.msi" -out ky.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_la.msi" -out la.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_lo.msi" -out lo.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_lt.msi" -out lt.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_lv.msi" -out lv.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_mg.msi" -out mg.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_mi.msi" -out mi.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_mk.msi" -out mk.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ml.msi" -out ml.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_mn.msi" -out mn.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_mr.msi" -out mr.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ms.msi" -out ms.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_mt.msi" -out mt.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ne.msi" -out ne.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_nl.msi" -out nl.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_no.msi" -out no.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_pa.msi" -out pa.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_pl.msi" -out pl.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ps.msi" -out ps.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_pt-BR.msi" -out pt-BR.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_pt-PT.msi" -out pt-PT.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ro.msi" -out ro.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ru.msi" -out ru.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_sd.msi" -out sd.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_si.msi" -out si.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_sk.msi" -out sk.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_sl.msi" -out sl.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_sn.msi" -out sn.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_so.msi" -out so.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_sq.msi" -out sq.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_sr.msi" -out sr.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_su.msi" -out su.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_sw.msi" -out sw.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ta.msi" -out ta.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_te.msi" -out te.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_tg.msi" -out tg.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_th.msi" -out th.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_tk.msi" -out tk.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_tl.msi" -out tl.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_tr.msi" -out tr.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_tt.msi" -out tt.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ug.msi" -out ug.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_uk.msi" -out uk.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_ur.msi" -out ur.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_uz.msi" -out uz.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_vi.msi" -out vi.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_xh.msi" -out xh.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_yi.msi" -out yi.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_yo.msi" -out yo.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_zh-CN.msi" -out zh-CN.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_zh-TW.msi" -out zh-TW.mst
wix msi transform -t language "RAASClient_%RAASClientVersion%_x64.msi" "RAASClient_x64_zu.msi" -out zu.mst
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" sv-se.mst 1053
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ar.mst 1025
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" bg.mst 1026
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ca.mst 1027
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" zh-TW.mst 1028
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" cs.mst 1029
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" da.mst 1030
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" de.mst 1031
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" el.mst 1032
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" es.mst 1034
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" fi.mst 1035
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" fr.mst 1036
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" iw.mst 1037
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
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" uk.mst 1058
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" be.mst 1059
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" sl.mst 1060
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" et.mst 1061
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" lv.mst 1062
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" lt.mst 1063
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" tg.mst 1064
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" fa.mst 1065
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" vi.mst 1066
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" hy.mst 1067
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" eu.mst 1069
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" mk.mst 1071
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" zu.mst 1077
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" af.mst 1078
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ka.mst 1079
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" hi.mst 1081
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" mt.mst 1082
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" yi.mst 1085
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ms.mst 1086
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" kk.mst 1087
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ky.mst 1088
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" sw.mst 1089
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" tk.mst 1090
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" uz.mst 1091
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" tt.mst 1092
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" pa.mst 1094
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" gu.mst 1095
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ta.mst 1097
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" te.mst 1098
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" kn.mst 1099
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ml.mst 1100
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" mr.mst 1102
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" mn.mst 1104
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" cy.mst 1106
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" lo.mst 1108
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" gl.mst 1110
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" sd.mst 1113
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" si.mst 1115
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" am.mst 1118
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ne.mst 1121
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ps.mst 1123
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" tl.mst 1124
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ha.mst 1128
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" yo.mst 1130
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" qu.mst 1131
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ig.mst 1136
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" haw.mst 1141
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" la.mst 1142
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" so.mst 1143
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ug.mst 1152
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" mi.mst 1153
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" zh-CN.mst 2052
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" nn.mst 2068
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" pt-PT.mst 2070
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" sr.mst 2074
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ur.mst 2080
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ms.mst 2110
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" az.mst 2092
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" bs.mst 5146
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" co.mst 131
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" eo.mst 4096
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ga.mst 2108
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" gd.mst 1169
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ht.mst 15372
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" jw.mst 4096
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" jv.mst 4096
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" km.mst 1107
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" ku.mst 146
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" mg.mst 4096
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" sn.mst 1033
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" su.mst 4096
cscript ..\scripts\WiSubStg.vbs "RAASClient_%RAASClientVersion%_x64.msi" xh.mst 52
cscript ..\scripts\WiLangId.vbs "RAASClient_%RAASClientVersion%_x64.msi" Package 1033,1053
pause