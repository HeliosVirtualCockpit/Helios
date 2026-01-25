function driver.processHighImportance(mainPanelDevice)
    -- Altimeter
    helios.send(
        2051,
        string.format(
            "%0.4f;%0.4f;%0.4f",
            mainPanelDevice:get_argument_value(220),
            mainPanelDevice:get_argument_value(219),
            mainPanelDevice:get_argument_value(218)
        )
    )
    helios.send(
        2059,
        string.format(
            "%0.2f;%0.2f;%0.3f",
            mainPanelDevice:get_argument_value(223),
            mainPanelDevice:get_argument_value(222),
            mainPanelDevice:get_argument_value(221)
        )
    )
	-- Send ADI Ball Values	
	helios.send(2050, string.format("%0.4f;0.0;%0.4f", mainPanelDevice:get_argument_value(205), mainPanelDevice:get_argument_value(206)))

    -- Calcuate HSI Value
    -- helios.send(2029, string.format("%0.2f;%0.2f;%0.4f", mainPanelDevice:get_argument_value(29), mainPanelDevice:get_argument_value(30), mainPanelDevice:get_argument_value(31)))

    -- IFEI structured data
    local li = helios.parseIndication(5) -- 5 for IFEI
    if li then
        --
        -- IFEI data
        --
        helios.send(2052, string.format("%s", helios.ensureString(li.txt_BINGO)))
        helios.send(2053, string.format("%s", helios.ensureString(li.txt_CLOCK_H)))
        helios.send(2054, string.format("%s", helios.ensureString(li.txt_CLOCK_M)))
        helios.send(2055, string.format("%s", helios.ensureString(li.txt_CLOCK_S)))
        helios.send(2056, string.format("%s", helios.textureToString(li.txt_DD_1)))
        helios.send(2057, string.format("%s", helios.textureToString(li.txt_DD_2)))
        helios.send(2058, string.format("%s", helios.textureToString(li.txt_DD_3)))
        helios.send(2060, string.format("%s", helios.textureToString(li.txt_DD_4)))
        helios.send(2061, string.format("%s", helios.ensureString(li.txt_FF_L)))
        helios.send(2062, string.format("%s", helios.ensureString(li.txt_FF_R)))
        helios.send(2063, string.format("%s", helios.ensureString(li.txt_FUEL_DOWN)))
        helios.send(2064, string.format("%s", helios.ensureString(li.txt_FUEL_UP)))
        helios.send(2065, string.format("%s", helios.ensureString(li.txt_OilPress_L)))
        helios.send(2066, string.format("%s", helios.ensureString(li.txt_OilPress_R)))
        helios.send(2067, string.format("%s", helios.ensureString(li.txt_RPM_L)))
        helios.send(2068, string.format("%s", helios.ensureString(li.txt_RPM_R)))
        helios.send(2069, string.format("%s", helios.ensureString(li.txt_TEMP_L)))
        helios.send(2070, string.format("%s", helios.ensureString(li.txt_TEMP_R)))
        helios.send(2073, string.format("%s", helios.ensureString(li.txt_TIMER_H)))
        helios.send(2072, string.format("%s", helios.ensureString(li.txt_TIMER_M)))
        helios.send(2071, string.format("%s", helios.ensureString(li.txt_TIMER_S)))
        helios.send(2074, string.format("%s", helios.ensureString(li.txt_Codes)))
        helios.send(2075, string.format("%s", helios.ensureString(li.txt_SP)))
        helios.send(2076, string.format("%s", helios.ensureString(li.txt_DrawChar))) -- not seen this used
        helios.send(2077, string.format("%s", helios.ensureString(li.txt_T)))
        helios.send(2078, string.format("%s", helios.ensureString(li.txt_TimeSetMode)))

        --
        -- IFEI textures
        --
        helios.send(4000, string.format("%s", helios.textureToString(li.RPMTexture)))
        helios.send(4001, string.format("%s", helios.textureToString(li.TempTexture)))
        helios.send(4002, string.format("%s", helios.textureToString(li.FFTexture)))
        helios.send(4003, string.format("%s", helios.textureToString(li.NOZTexture)))
        helios.send(4004, string.format("%s", helios.textureToString(li.OILTexture)))
        helios.send(4005, string.format("%s", helios.textureToString(li.BINGOTexture)))
        helios.send(4006, string.format("%s", helios.textureToString(li.LScaleTexture)))
        helios.send(4007, string.format("%s", helios.textureToString(li.RScaleTexture)))
        helios.send(4008, string.format("%s", helios.textureToString(li.L0Texture)))
        helios.send(4009, string.format("%s", helios.textureToString(li.R0Texture)))
        helios.send(4010, string.format("%s", helios.textureToString(li.L50Texture)))
        helios.send(4011, string.format("%s", helios.textureToString(li.R50Texture)))
        helios.send(4012, string.format("%s", helios.textureToString(li.L100Texture)))
        helios.send(4013, string.format("%s", helios.textureToString(li.R100Texture)))
        helios.send(4014, string.format("%s", helios.textureToString(li.LPointerTexture)))
        helios.send(4015, string.format("%s", helios.textureToString(li.RPointerTexture)))
        helios.send(4016, string.format("%s", helios.textureToString(li.ZTexture)))
        helios.send(4017, string.format("%s", helios.textureToString(li.LTexture)))
        helios.send(4018, string.format("%s", helios.textureToString(li.RTexture)))
    end

    -- UFC structured data
    li = helios.parseIndication(6) -- 6 for UFC
    if li then
        helios.send(2080, string.format("%s", helios.ensureString(li.UFC_MainDummy)))
        helios.send(2081, string.format("%s", helios.ensureString(li.UFC_mask)))
        helios.send(2082, string.format("%s", helios.ensureString(li.UFC_OptionDisplay1))) 
        helios.send(2083, string.format("%s", helios.ensureString(li.UFC_OptionDisplay2))) 
        helios.send(2084, string.format("%s", helios.ensureString(li.UFC_OptionDisplay3))) 
        helios.send(2085, string.format("%s", helios.ensureString(li.UFC_OptionDisplay4))) 
        helios.send(2086, string.format("%s", helios.ensureString(li.UFC_OptionDisplay5))) 
        helios.send(2087, string.format("%1s", helios.ensureString(li.UFC_OptionCueing1):gsub(":", "!"))) -- colon char is reserved
        helios.send(2088, string.format("%1s", helios.ensureString(li.UFC_OptionCueing2):gsub(":", "!"))) -- colon char is reserved
        helios.send(2089, string.format("%1s", helios.ensureString(li.UFC_OptionCueing3):gsub(":", "!"))) -- colon char is reserved
        helios.send(2090, string.format("%1s", helios.ensureString(li.UFC_OptionCueing4):gsub(":", "!"))) -- colon char is reserved
        helios.send(2091, string.format("%1s", helios.ensureString(li.UFC_OptionCueing5):gsub(":", "!"))) -- colon char is reserved
        helios.send(2092, string.format("%2s", helios.ensureString(li.UFC_ScratchPadString1Display))) -- 16 Segment
        helios.send(2093, string.format("%2s", helios.ensureString(li.UFC_ScratchPadString2Display))) -- 16 Segment
        helios.send(2094, string.format("%7s", helios.ensureString(li.UFC_ScratchPadNumberDisplay)))  -- 7 Segment numbers
        helios.send(2095, string.format("%2s", helios.ensureString(li.UFC_Comm1Display)))				-- 16 Segment
        helios.send(2096, string.format("%2s", helios.ensureString(li.UFC_Comm2Display)))				-- 16 Segment
    end
end

function driver.processLowImportance(mainPanelDevice) --luacheck: no unused args
    -- Radio Frequencies
    -- local lUHFRadio = GetDevice(54)
    -- helios.send(2000, string.format("%7.3f", lUHFRadio:get_frequency()/1000000))

    -- ILS Frequency
    -- helios.send(2251, string.format("%0.1f;%0.1f", mainPanelDevice:get_argument_value(251), mainPanelDevice:get_argument_value(252)))

    -- TACAN Channel
    -- helios.send(2263, string.format("%0.2f;%0.2f;%0.2f", mainPanelDevice:get_argument_value(263), mainPanelDevice:get_argument_value(264), mainPanelDevice:get_argument_value(265)))
end
