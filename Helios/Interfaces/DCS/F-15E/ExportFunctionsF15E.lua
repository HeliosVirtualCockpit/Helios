-- Exports.Lua from Helios DCS F-15E Strike Eagle
function driver.processHighImportance(mainPanelDevice)
	-- Send Altimeter Values	
	helios.send(2051, string.format("%0.4f;%0.4f;%0.4f", mainPanelDevice:get_argument_value(355), mainPanelDevice:get_argument_value(354), mainPanelDevice:get_argument_value(352)))                                                --  Altitude
	helios.send(2059, string.format("%0.2f;%0.2f;%0.2f;%0.3f", mainPanelDevice:get_argument_value(356), mainPanelDevice:get_argument_value(357), mainPanelDevice:get_argument_value(358), mainPanelDevice:get_argument_value(359)))	-- QNH pressure 	

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
	local li = helios.parseIndication(8) -- 8 Pilot UFC / ODU 

	if li then
        for i=0, 5 do
			if i~= 4 then
				helios.send(2100 + i, string.format("%s;%s;%s;%s", i+1,helios.ensureString(li[oduVarNames[(3*i) + 1]]):gsub(":","!"),helios.ensureString(li[oduVarNames[(3*i) + 2]]):gsub(":","!"),helios.ensureString(li[oduVarNames[(3*i) + 3]]):gsub(":","!")))
			else
				helios.send(2100 + i, string.format("%s;%s;%s;%s;%s;%s", i+1,helios.ensureString(li[oduVarNames[(3*i) + 1]]):gsub(":","!"),helios.ensureString(li[oduVarNames[(3*i) + 2]]):gsub(":","!"),helios.ensureString(li[oduVarNames[(3*i) + 3]]):gsub(":","!"),helios.ensureString(li["UFC_SC_05A"]):gsub(":","!"),helios.ensureString(li["UFC_SC_08A"]):gsub(":","!")))
			end
        end
    end
    li = helios.parseIndication(20) -- 20 WSO UFC / ODU 
	if li then
        for i=0, 5 do
			if i~= 4 then
				helios.send(2140 + i, string.format("%s;%s;%s;%s", i+1,helios.ensureString(li[oduVarNames[(3*i) + 1]]):gsub(":","!"),helios.ensureString(li[oduVarNames[(3*i) + 2]]):gsub(":","!"),helios.ensureString(li[oduVarNames[(3*i) + 3]]):gsub(":","!")))
			else
				helios.send(2140 + i, string.format("%s;%s;%s;%s;%s;%s", i+1,helios.ensureString(li[oduVarNames[(3*i) + 1]]):gsub(":","!"),helios.ensureString(li[oduVarNames[(3*i) + 2]]):gsub(":","!"),helios.ensureString(li[oduVarNames[(3*i) + 3]]):gsub(":","!"),helios.ensureString(li["UFC_SC_05A"]]):gsub(":","!"),helios.ensureString(li["UFC_SC_08A"]]):gsub(":","!")))
			end
        end
    end

    helios.send(2010, string.format("%.0f",mainPanelDevice:get_argument_value(366) * 100000+mainPanelDevice:get_argument_value(367) * 10000+mainPanelDevice:get_argument_value(368) * 1000+mainPanelDevice:get_argument_value(369) * 100+mainPanelDevice:get_argument_value(370) * 10))    -- Fuel Total
    helios.send(2011, string.format("%.0f",mainPanelDevice:get_argument_value(371) * 10000+mainPanelDevice:get_argument_value(372) * 1000+mainPanelDevice:get_argument_value(373) * 100+mainPanelDevice:get_argument_value(374) * 10))                                                     -- Fuel Left Tank
    helios.send(2012, string.format("%.0f",mainPanelDevice:get_argument_value(375) * 10000+mainPanelDevice:get_argument_value(376) * 1000+mainPanelDevice:get_argument_value(377) * 100+mainPanelDevice:get_argument_value(378) * 10))                                                     -- Fuel Right Tank
    helios.send(2013, string.format("%.0f",mainPanelDevice:get_argument_value(381) * 10000+mainPanelDevice:get_argument_value(382) * 1000+mainPanelDevice:get_argument_value(383) * 100+mainPanelDevice:get_argument_value(384) * 10))                                                     -- Fuel Bingo    
    
end
