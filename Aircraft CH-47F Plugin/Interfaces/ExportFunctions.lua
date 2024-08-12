-- Exports.Lua from Helios CH-47F interface

function driver.processHighImportance(mainPanelDevice)
	-- Send Altimeter Values	
	helios.send(2051, string.format("%0.4f;%0.4f;%0.4f", mainPanelDevice:get_argument_value(606), mainPanelDevice:get_argument_value(605), mainPanelDevice:get_argument_value(479)))
	helios.send(2059, string.format("%0.2f;%0.2f;%0.3f", mainPanelDevice:get_argument_value(607), mainPanelDevice:get_argument_value(608), mainPanelDevice:get_argument_value(609)))		
	-- RADAR Altimeter Digital Display
    helios.send(
        2055,
        string.format(
            "%0.2f;%0.2f;%0.2f;%0.3f",
            mainPanelDevice:get_argument_value(1200),
            mainPanelDevice:get_argument_value(1201),
            mainPanelDevice:get_argument_value(1202),
            mainPanelDevice:get_argument_value(1203)
        )
    )

end

function driver.processLowImportance(mainPanelDevice)

    -- structured data
--    li = helios.parseIndication(16) -- 16 Pilot Keyboard Unit
--    if li then
--        helios.send(2080, string.format("%s", helios.ensureString(li.Standby_text):gsub(":", "!")))
--	else
--		helios.send(2080,"")
--    end

end