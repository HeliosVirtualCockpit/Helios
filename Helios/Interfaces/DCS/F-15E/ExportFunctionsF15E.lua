-- Exports.Lua from Helios DCS F-15E Strike Eagle
function driver.processHighImportance(mainPanelDevice)
	-- Send Altimeter Values	
	helios.send(2051, string.format("%0.4f;%0.4f;%0.4f", mainPanelDevice:get_argument_value(355), mainPanelDevice:get_argument_value(354), mainPanelDevice:get_argument_value(352)))
	helios.send(2059, string.format("%0.2f;%0.2f;%0.2f;%0.3f", mainPanelDevice:get_argument_value(356), mainPanelDevice:get_argument_value(357), mainPanelDevice:get_argument_value(358), mainPanelDevice:get_argument_value(359)))		

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
"UFC_CC_04",
"UFC_SC_09",
"UFC_SC_05",
"UFC_CC_05",
"UFC_SC_08",
"UFC_SC_06",
"UFC_CC_06",
"UFC_SC_07",
"UFC_DISPLAY"
} 
function driver.processLowImportance(mainPanelDevice)		
	local li = helios.parseIndication(8) -- 8 Pilot UFC / ODU 
	if li then
        for i=0, 18 do
            driver.processOduText(li, 2100 + i,oduVarNames[i])
        end
    end
    li = helios.parseIndication(20) -- 20 WSO UFC / ODU 
	if li then
        for i=0, 18 do
            driver.processOduText(li, 2200 + i,oduVarNames[i])
        end
    end

--		helios.send(2100, string.format("%s", helios.ensureString(li.UFC_SC_01):gsub(":","!")))
--		helios.send(2101, string.format("%s", helios.ensureString(li.UFC_CC_01):gsub(":","!")))
--		helios.send(2102, string.format("%s", helios.ensureString(li.UFC_SC_12):gsub(":","!")))
--		helios.send(2103, string.format("%s", helios.ensureString(li.UFC_SC_02):gsub(":","!")))
--		helios.send(2104, string.format("%s", helios.ensureString(li.UFC_CC_02):gsub(":","!")))
--		helios.send(2105, string.format("%s", helios.ensureString(li.UFC_SC_11):gsub(":","!")))
--		helios.send(2106, string.format("%s", helios.ensureString(li.UFC_SC_03):gsub(":","!")))
--		helios.send(2107, string.format("%s", helios.ensureString(li.UFC_CC_03):gsub(":","!")))
--		helios.send(2108, string.format("%s", helios.ensureString(li.UFC_SC_10):gsub(":","!")))
--		helios.send(2109, string.format("%s", helios.ensureString(li.UFC_SC_04):gsub(":","!")))
--		helios.send(2110, string.format("%s", helios.ensureString(li.UFC_CC_04):gsub(":","!")))
--		helios.send(2111, string.format("%s", helios.ensureString(li.UFC_SC_09):gsub(":","!")))
--		helios.send(2112, string.format("%s", helios.ensureString(li.UFC_SC_05):gsub(":","!")))
--		helios.send(2113, string.format("%s", helios.ensureString(li.UFC_CC_05):gsub(":","!")))
--		helios.send(2114, string.format("%s", helios.ensureString(li.UFC_SC_08):gsub(":","!")))
--      helios.send(2115, string.format("%s", helios.ensureString(li.UFC_SC_06):gsub(":","!")))
--		helios.send(2116, string.format("%s", helios.ensureString(li.UFC_CC_06):gsub(":","!")))
--		helios.send(2117, string.format("%s", helios.ensureString(li.UFC_SC_07):gsub(":","!")))
--		helios.send(2118, string.format("%s", helios.ensureString(li.UFC_DISPLAY):gsub(":","!")))
        --helios.send(21??, string.format("%s", helios.ensureString(li.UFC_SC_05A):gsub(":","!")))
		--helios.send(21??, string.format("%s", helios.ensureString(li.UFC_SC_08A):gsub(":","!")))

        --UFC_CC_01 	 ""
        --UFC_CC_02 	 ""
        --UFC_CC_03 	 ""
        --UFC_CC_04 	 ""
        --UFC_DISPLAY 	 ""
        --UFC_LL_INPUT_DEG 	 "°"
        --UFC_LL_INPUT_MIN 	 "."
        --UFC_SC_01 	 ""
        --UFC_SC_02 	 ""
        --UFC_SC_02A 	 "°"
        --UFC_SC_02B 	 "."
        --UFC_SC_03 	 ""
        --UFC_SC_03ILS 	 "."
        --UFC_SC_03L1 	 "°"
        --UFC_SC_03L2 	 "."
        --UFC_SC_04 	 ""
        --UFC_SC_05 	 ""
        --UFC_SC_05A 	 "."
        --UFC_SC_06 	 ""
        --UFC_SC_07 	 ""
        --UFC_SC_08 	 ""
        --UFC_SC_08A 	 "."
        --UFC_SC_09_D2A 	":"
        --UFC_SC_09_D2B 	":"
        --UFC_SC_09 	 ""
        --UFC_SC_10_SPB 	 "°"
        --UFC_SC_10 	 ""
        --UFC_SC_11_D2A 	":"
        --UFC_SC_11_D2B 	":"
        --UFC_SC_11_SPA 	":"
        --UFC_SC_11_SPB 	":"
        --UFC_SC_11 	 ""
        --UFC_SC_12 	 ""
        --UFC_SC_R1 	 "."
        --UFC_SC_R23R3 	 "."
end
function driver.processOduText(li, argVal, variableName)
    --if li[variableName] ~= nil then 
    	helios.send(argVal, string.format("%s", helios.ensureString(li[variableName]):gsub(":","!"))) 
    --end
end