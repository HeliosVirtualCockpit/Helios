-- Exports.Lua from Helios F-16C Viper interface

function driver.processHighImportance(mainPanelDevice)
	-- Send Altimeter Values	
	helios.send(2051, string.format("%0.4f;%0.4f;%0.5f", mainPanelDevice:get_argument_value(52), mainPanelDevice:get_argument_value(53), mainPanelDevice:get_argument_value(51)))
	helios.send(2059, string.format("%0.2f;%0.2f;%0.2f;%0.3f", mainPanelDevice:get_argument_value(56), mainPanelDevice:get_argument_value(57), mainPanelDevice:get_argument_value(58), mainPanelDevice:get_argument_value(59)))		
	-- Calcuate HSI Value
	helios.send(2029, string.format("%0.2f;%0.2f;%0.4f", mainPanelDevice:get_argument_value(29), mainPanelDevice:get_argument_value(30), mainPanelDevice:get_argument_value(31)))
	-- Send ADI Ball Values	
	helios.send(2050, string.format("%0.4f;0.0;%0.4f", mainPanelDevice:get_argument_value(17), mainPanelDevice:get_argument_value(18)))


    local li = helios.parseIndication(16) -- 16 for CMDS
    if li then
        --
        -- CMDS data
        --
        helios.send(2000, string.format("%s", helios.ensureString(li.CMDS_O1_Amount)))        		
        helios.send(2001, string.format("%s", helios.ensureString(li.CMDS_O2_Amount)))
        helios.send(2002, string.format("%s", helios.ensureString(li.CMDS_CH_Amount)))
        helios.send(2003, string.format("%s", helios.ensureString(li.CMDS_FL_Amount)))
    else
        helios.send(2000, "")        		
        helios.send(2001, "")
        helios.send(2002, "")
        helios.send(2003, "")
    end
    local li = helios.parseIndication(10) -- 10 for UHF channel
    if li then
        --
        -- UHF channel data
        --
        helios.send(2004, string.format("%2s", helios.ensureString(li.txtPresetChannel)))
    else
        helios.send(2004, "  ")
    end
    local li = helios.parseIndication(11) -- 11 for UHF Freq
    if li then
        --
        -- UHF freq data
        --
        helios.send(2005, string.format("%3s%1s%3s", helios.ensureString(li.txtPresetChannel):sub(1,3), helios.ensureString(li.txtDot), helios.ensureString(li.txtFreqStatus):sub(4,6)))
    else
        helios.send(2005,"       ")
    end
end

function driver.processLowImportance(mainPanelDevice)
        -- Fuel Totalizer
	    helios.send(2090, string.format(
            "%0.5f;%0.5f;%0.5f",
            mainPanelDevice:get_argument_value(730),
            mainPanelDevice:get_argument_value(731),
            mainPanelDevice:get_argument_value(732)
			)
		)
        -- Fuel Flow
	    helios.send(2091, string.format(
            "%0.5f;%0.5f;%0.5f",
            mainPanelDevice:get_argument_value(88),
            mainPanelDevice:get_argument_value(89),
            mainPanelDevice:get_argument_value(90)
			)
		)

end