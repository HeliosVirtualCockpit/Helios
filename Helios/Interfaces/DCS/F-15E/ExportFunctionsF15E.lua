-- Exports.Lua from Helios DCS F-15E Strike Eagle
function driver.processHighImportance(mainPanelDevice)
	-- Send Altimeter Values	
	helios.send(2051, string.format("%0.4f;%0.4f;%0.4f", mainPanelDevice:get_argument_value(355), mainPanelDevice:get_argument_value(354), mainPanelDevice:get_argument_value(352)))													--  Altitude (Pilot)
	helios.send(2059, string.format("%0.2f;%0.2f;%0.2f;%0.3f", mainPanelDevice:get_argument_value(356), mainPanelDevice:get_argument_value(357), mainPanelDevice:get_argument_value(358), mainPanelDevice:get_argument_value(359)))		-- QNH pressure 	
	helios.send(2251, string.format("%0.4f;%0.4f;%0.4f", mainPanelDevice:get_argument_value(1359), mainPanelDevice:get_argument_value(1358), mainPanelDevice:get_argument_value(1356)))													--  Altitude (WSO)
	helios.send(2259, string.format("%0.2f;%0.2f;%0.2f;%0.3f", mainPanelDevice:get_argument_value(1360), mainPanelDevice:get_argument_value(1361), mainPanelDevice:get_argument_value(1362), mainPanelDevice:get_argument_value(1363)))	-- QNH pressure 	
end
local oduVarNames = {
"UFC_SC_01",
"UFC_CC_01",
"UFC_SC_12",
"UFC_SC_02",
"UFC_CC_02",
"UFC_SC_11",
"UFC_SC_03",
"UFC_CC_03",
"UFC_SC_10",
"UFC_SC_04",
"UFC_CC_06",
"UFC_SC_09",
"UFC_SC_05",
"UFC_CC_05",
"UFC_SC_08",
"UFC_SC_06",
"UFC_CC_04",
"UFC_SC_07",
"UFC_DISPLAY"
} 
function driver.processLowImportance(mainPanelDevice)		
	local li = helios.parseIndication(9) -- 9 Pilot UFC / ODU 

	if li then
        for i=0, 5 do
			if i~= 4 then
				helios.send(2100 + i, string.format("%s;%s;%s;%s", i+1,helios.ensureString(li[oduVarNames[(3*i) + 1]]):gsub(":","!"),helios.ensureString(li[oduVarNames[(3*i) + 2]]):gsub(":","!"),helios.ensureString(li[oduVarNames[(3*i) + 3]]):gsub(":","!")))
			else
				helios.send(2100 + i, string.format("%s;%s;%s;%s;%s;%s", i+1,helios.ensureString(li[oduVarNames[(3*i) + 1]]):gsub(":","!"),helios.ensureString(li[oduVarNames[(3*i) + 2]]):gsub(":","!"),helios.ensureString(li[oduVarNames[(3*i) + 3]]):gsub(":","!"),helios.ensureString(li["UFC_SC_05A"]):gsub(":","!"),helios.ensureString(li["UFC_SC_08A"]):gsub(":","!")))
			end
        end
		helios.send(2109, string.format("%s", helios.ensureString(li["UFC_DISPLAY"]):gsub(":","!")))
    end
    li = helios.parseIndication(18) -- 18 WSO UFC / ODU 
	if li then
        for i=0, 5 do
			if i~= 4 then
				helios.send(2110 + i, string.format("%s;%s;%s;%s", i+1,helios.ensureString(li[oduVarNames[(3*i) + 1]]):gsub(":","!"),helios.ensureString(li[oduVarNames[(3*i) + 2]]):gsub(":","!"),helios.ensureString(li[oduVarNames[(3*i) + 3]]):gsub(":","!")))
			else
				helios.send(2110 + i, string.format("%s;%s;%s;%s;%s;%s", i+1,helios.ensureString(li[oduVarNames[(3*i) + 1]]):gsub(":","!"),helios.ensureString(li[oduVarNames[(3*i) + 2]]):gsub(":","!"),helios.ensureString(li[oduVarNames[(3*i) + 3]]):gsub(":","!"),helios.ensureString(li["UFC_SC_05A"]):gsub(":","!"),helios.ensureString(li["UFC_SC_08A"]):gsub(":","!")))
			end
        end
		helios.send(2119, string.format("%s", helios.ensureString(li["UFC_DISPLAY"]):gsub(":","!")))
    end
	-- Fuel Monitor Panel
    helios.send(2010, string.format("%.5f;%.5f;%.5f",mainPanelDevice:get_argument_value(368), mainPanelDevice:get_argument_value(369), mainPanelDevice:get_argument_value(370))) -- Fuel Total
    helios.send(2011, string.format("%.5f;%.5f",mainPanelDevice:get_argument_value(373), mainPanelDevice:get_argument_value(374))) -- Fuel Left Tank
    helios.send(2012, string.format("%.5f;%.5f",mainPanelDevice:get_argument_value(377), mainPanelDevice:get_argument_value(378)))	-- Fuel Right Tank
    helios.send(2013, string.format("%.5f;%.5f;%.5f",mainPanelDevice:get_argument_value(1374), mainPanelDevice:get_argument_value(1375), mainPanelDevice:get_argument_value(1376))) -- Fuel Total (WSO)
    -- Engine Monitor Panel
 	helios.send(2070, string.format("%.0f", mainPanelDevice:get_argument_value(1118) * 100000+mainPanelDevice:get_argument_value(1117)%1 * 10000+mainPanelDevice:get_argument_value(1116)%1 * 1000+mainPanelDevice:get_argument_value(1115)%1 * 100+mainPanelDevice:get_argument_value(1114)%1*10))	-- Left Fuel Flow
	helios.send(2071, string.format("%.0f", mainPanelDevice:get_argument_value(1126) * 1000+mainPanelDevice:get_argument_value(1125)%1 * 100+mainPanelDevice:get_argument_value(1124)%1 * 10))	-- Left Oil Pressure
	helios.send(2072, string.format("%.0f", mainPanelDevice:get_argument_value(1102) * 1000+mainPanelDevice:get_argument_value(1101)%1 * 100+mainPanelDevice:get_argument_value(1100)%1 * 10))	-- Left Engine RPM
	helios.send(2073, string.format("%.0f", mainPanelDevice:get_argument_value(1109) * 10000+mainPanelDevice:get_argument_value(1108)%1 * 1000+mainPanelDevice:get_argument_value(1107)%1 * 100+mainPanelDevice:get_argument_value(1106)%1 * 10))	-- Left Engine Temperature
	helios.send(2074, string.format("%.0f", mainPanelDevice:get_argument_value(1123) * 100000+mainPanelDevice:get_argument_value(1122)%1 * 10000+mainPanelDevice:get_argument_value(1121)%1 * 1000+mainPanelDevice:get_argument_value(1120)%1 * 100+mainPanelDevice:get_argument_value(1119)%1*10))	-- Right Fuel Flow
	helios.send(2075, string.format("%.0f", mainPanelDevice:get_argument_value(1129) * 1000+mainPanelDevice:get_argument_value(1128)%1 * 100+mainPanelDevice:get_argument_value(1127)%1 * 10))	-- Right Oil Pressure
	helios.send(2076, string.format("%.0f", mainPanelDevice:get_argument_value(1105) * 1000+mainPanelDevice:get_argument_value(1104)%1 * 100+mainPanelDevice:get_argument_value(1103)%1 * 10))	-- Right Engine RPM
 	helios.send(2077, string.format("%.0f", mainPanelDevice:get_argument_value(1113) * 10000+mainPanelDevice:get_argument_value(1112)%1 * 1000+mainPanelDevice:get_argument_value(1111)%1 * 100+mainPanelDevice:get_argument_value(1110)%1 * 10))	-- Right Engine Temperature  
end
