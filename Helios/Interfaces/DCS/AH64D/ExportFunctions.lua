-- Exports.Lua from Helios AH-64D interface

function driver.processHighImportance(mainPanelDevice)
	-- Send Altimeter Values	
	helios.send(2051, string.format("%0.4f;%0.4f;%0.4f", mainPanelDevice:get_argument_value(606), mainPanelDevice:get_argument_value(605), mainPanelDevice:get_argument_value(479)))
	helios.send(2059, string.format("%0.2f;%0.2f;%0.3f", mainPanelDevice:get_argument_value(607), mainPanelDevice:get_argument_value(608), mainPanelDevice:get_argument_value(609)))		
end

function driver.processLowImportance(mainPanelDevice)

    -- structured data
    li = helios.parseIndication(15) -- 15 Pilot Keyboard Unit
    if li then
        helios.send(2080, string.format("%s", helios.ensureString(li.Standby_text):gsub(":", "!")))
    end
    li = helios.parseIndication(14) -- 14 CP/G Keyboard Unit
    if li then
        helios.send(2081, string.format("%s", helios.ensureString(li.Standby_text):gsub(":", "!")))
    end
	if mainPanelDevice:get_argument_value(610) == -1.0 then
		-- clear the Chaff, Flare, and threat display if the CMWS is off
		helios.send(2082, "")
		helios.send(2083, "")
		for ii = 2084,2093 do helios.send(ii, "0.0") end
	else
		li = helios.parseIndication(24) -- 24 CMWS Unit
		if li then
			if li["#83#"]  and li["#83#"] ~= "" then -- Chaff & Flares
				helios.send(2082, string.format("%1s %s", helios.ensureString(li["#83#"]), helios.ensureString(li["#85#"])))
				helios.send(2083, string.format("%1s %s", helios.ensureString(li["#84#"]), helios.ensureString(li["#86#"])))
				-- The CMWS flags are held in variables which either are declared or not, and when the exist, they are an empty string
				helios.send(2084, string.format("%0.1f", li["#87#"] and 1 or 0)) -- Ready (this seems to have an inverse which is #89#)
				helios.send(2085, string.format("%0.1f", li["#88#"] and 1 or 0)) -- Dispense (this seems to have an inverse which is #90#)
				helios.send(2086, string.format("%0.1f", li["#17#"] and 1 or 0)) -- top left arrow display is #17#-#23#
				helios.send(2087, string.format("%0.1f", li["#10#"] and 1 or 0)) -- top right arrow display is #10#-#16#
				helios.send(2088, string.format("%0.1f", li["#24#"] and 1 or 0)) -- bottom left arrow display is #24#-#30#
				helios.send(2089, string.format("%0.1f", li["#31#"] and 1 or 0)) -- bottom right arrow display is #31#-#37#
				helios.send(2090, string.format("%0.1f", li["#38#"] and 1 or 0)) -- top yellow arrow
				helios.send(2091, string.format("%0.1f", li["#39#"] and 1 or 0)) -- left yellow arrow
				helios.send(2092, string.format("%0.1f", li["#40#"] and 1 or 0)) -- bottom yellow arrow
				helios.send(2093, string.format("%0.1f", li["#41#"] and 1 or 0)) -- right yellow arrow		
			elseif li["#42#"] and li["#42#"] ~= ""  then -- CMWS Status
				helios.send(2082, string.format("%s", helios.ensureString(li["#42#"])))
				helios.send(2083, string.format("%s", helios.ensureString(li["#43#"])))
				-- Suppress the threat direction when CMWS is in test mode
				for ii = 2084,2093 do helios.send(ii, "0.0") end
			else
			end
		end
	end

end