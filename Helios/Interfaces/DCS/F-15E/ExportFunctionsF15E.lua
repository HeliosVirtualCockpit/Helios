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
        for i=1, 6 do
            driver.processOduText(li, 2100 - 1 + ((3*(i-1))+1),oduVarNames[(3*(i-1))+1],oduVarNames[(3*(i-1))+3])
        end
    end
    li = helios.parseIndication(20) -- 20 WSO UFC / ODU 
	if li then
        for i=1, 6 do
            driver.processOduText(li, 2200 - 1 + ((3*(i-1))+1),oduVarNames[(3*(i-1))+1],oduVarNames[(3*(i-1))+3])
        end
    end
end
function driver.processOduText(li, argVal, variableName1, variableName2)
    --if li[variableName] ~= nil then 
    	helios.send(argVal, string.format("%.8s    %8s", (helios.ensureString(li[variableName1]):gsub(":","!").."        "), helios.ensureString(li[variableName2]):gsub(":","!"))) 
    --end
end