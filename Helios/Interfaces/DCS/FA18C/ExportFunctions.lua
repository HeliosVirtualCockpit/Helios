-- Exports.Lua from Helios F/A-18C Interface
-- Exports.Lua from Helios F/A-18C Interface
print("Helios Aircraft Exports:  F/A-18C\n")

function ProcessHighImportance(mainPanelDevice)
	-- Send Altimeter Values	
	SendData(2051, string.format("%0.4f;%0.4f;%0.4f", mainPanelDevice:get_argument_value(220), mainPanelDevice:get_argument_value(219), mainPanelDevice:get_argument_value(218)))
	SendData(2059, string.format("%0.2f;%0.2f;%0.3f", mainPanelDevice:get_argument_value(223), mainPanelDevice:get_argument_value(222), mainPanelDevice:get_argument_value(221)))		
	-- Calcuate HSI Value
	-- SendData(2029, string.format("%0.2f;%0.2f;%0.4f", mainPanelDevice:get_argument_value(29), mainPanelDevice:get_argument_value(30), mainPanelDevice:get_argument_value(31)))


--    --IFEI textures
--    local IFEI_Textures_table = {}
--    for i=1,16 do IFEI_Textures_table[i] = 0 end
--
--
--    -- getting the IFEI data
--    local li = parse_indication(5)  -- 5 for IFEI
--    if li then
--        --IFEI data
--
        SendData("2052", string.format("%s",check(li.txt_BINGO)))
        SendData("2053", string.format("%s",check(li.txt_CLOCK_H)))
        SendData("2054", string.format("%s",check(li.txt_CLOCK_M)))
        SendData("2055", string.format("%s",check(li.txt_CLOCK_S)))
        SendData("2056", string.format("%s",check(li.txt_DD_1)):gsub(":","|"))
        SendData("2057", string.format("%s",check(li.txt_DD_2)):gsub(":","|"))
        SendData("2058", string.format("%s",check(li.txt_DD_3)):gsub(":","|"))
        SendData("2060", string.format("%s",check(li.txt_DD_4)):gsub(":","|"))
        SendData("2061", string.format("%s",check(li.txt_FF_L)))
        SendData("2062", string.format("%s",check(li.txt_FF_R)))
        SendData("2063", string.format("%s",check(li.txt_FUEL_DOWN)))
        SendData("2064", string.format("%s",check(li.txt_FUEL_UP)))
        SendData("2065", string.format("%s",check(li.txt_OilPress_L)))
        SendData("2066", string.format("%s",check(li.txt_OilPress_R)))
        SendData("2067", string.format("%s",check(li.txt_RPM_L)))
        SendData("2068", string.format("%s",check(li.txt_RPM_R)))
        SendData("2069", string.format("%s",check(li.txt_TEMP_L)))
        SendData("2070", string.format("%s",check(li.txt_TEMP_R)))
        SendData("2071", string.format("%s",check(li.txt_TIMER_S)))
        SendData("2072", string.format("%s",check(li.txt_TIMER_M)))
        --SendData("2073", string.format("%s",check(li.txt_TIMER_H)))
        --SendData("2074", string.format("%s",check(li.txt_Codes)))
        --SendData("2075", string.format("%s",check(li.txt_SP)))
        --SendData("2076", string.format("%s",check(li.txt_DrawChar)))
        --SendData("2077", string.format("%s",check(li.txt_T)))
        --SendData("2078", string.format("%s",check(li.txt_TimeSetMode)))
--
--        --IFEI textures
--
----      IFEI_Textures_table[1]  =check_num(li.RPMTexture)
----      IFEI_Textures_table[2]  =check_num(li.TempTexture)
----      IFEI_Textures_table[3]  =check_num(li.FFTexture )
----      IFEI_Textures_table[4]  =check_num(li.NOZTexture)
----      IFEI_Textures_table[5]  =check_num(li.OILTexture)
----      IFEI_Textures_table[6]  =check_num(li.BINGOTexture)
----      IFEI_Textures_table[7]  =check_num(li.LScaleTexture)
----      IFEI_Textures_table[8]  =check_num(li.RScaleTexture)
----      IFEI_Textures_table[9]  =check_num(li.L0Texture)
----      IFEI_Textures_table[10] =check_num(li.R0Texture)
----      IFEI_Textures_table[11] =check_num(li.L50Texture)
----      IFEI_Textures_table[12] =check_num(li.R50Texture)
----      IFEI_Textures_table[13] =check_num(li.L100Texture)
----      IFEI_Textures_table[14] =check_num(li.R100Texture)
----      IFEI_Textures_table[15] =check_num(li.LPointerTexture)
----      IFEI_Textures_table[16] =check_num(li.RPointerTexture)
--
----
--    end
--
	-- getting the UFC data
	local li = parse_indication(6)  -- 6 for UFC
	if li then
--      SendData("2098", string.format("%s",li))
			
--      SendData("2080", string.format("%s",check(li.UFC_MainDummy)))
--      SendData("2081", string.format("%s",check(li.UFC_mask)))
        SendData("2082", string.format("%s",check(li.UFC_OptionDisplay1)))
        SendData("2083", string.format("%s",check(li.UFC_OptionDisplay2)))
        SendData("2084", string.format("%s",check(li.UFC_OptionDisplay3)))
        SendData("2085", string.format("%s",check(li.UFC_OptionDisplay4)))
        SendData("2086", string.format("%s",check(li.UFC_OptionDisplay5)))
        SendData("2087", string.format("%s",check(li.UFC_OptionCueing1)):gsub(":","|"))
        SendData("2088", string.format("%s",check(li.UFC_OptionCueing2)):gsub(":","|"))
        SendData("2089", string.format("%s",check(li.UFC_OptionCueing3)):gsub(":","|"))
        SendData("2090", string.format("%s",check(li.UFC_OptionCueing4)):gsub(":","|"))
        SendData("2091", string.format("%s",check(li.UFC_OptionCueing5)):gsub(":","|"))
        SendData("2092", string.format("%s",check(li.UFC_ScratchPadString1Display)))
        SendData("2093", string.format("%s",check(li.UFC_ScratchPadString2Display)))
        SendData("2094", string.format("%s",check(li.UFC_ScratchPadNumberDisplay)))
        SendData("2095", string.format("%s",check(li.UFC_Comm1Display)):gsub("`","1"):gsub("~","2"):gsub(" ",""))
        SendData("2096", string.format("%s",check(li.UFC_Comm2Display)):gsub("`","1"):gsub("~","2"):gsub(" ",""))
	end
	
	--SendData("2098", string.format("%s", table.concat(IFEI_Textures_table,", ") ) )    -- IFEI Textures

end


function ProcessLowImportance(mainPanelDevice)
	-- Get Radio Frequencies
	--local lUHFRadio = GetDevice(54)
	--SendData(2000, string.format("%7.3f", lUHFRadio:get_frequency()/1000000))
	-- ILS Frequency
	--SendData(2251, string.format("%0.1f;%0.1f", mainPanelDevice:get_argument_value(251), mainPanelDevice:get_argument_value(252)))
	-- TACAN Channel
	--SendData(2263, string.format("%0.2f;%0.2f;%0.2f", mainPanelDevice:get_argument_value(263), mainPanelDevice:get_argument_value(264), mainPanelDevice:get_argument_value(265)))

end
