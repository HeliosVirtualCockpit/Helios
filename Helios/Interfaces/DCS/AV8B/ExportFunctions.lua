-- Exports.Lua from Helios AV-8B interface
function driver.processHighImportance(mainPanelDevice)
	-- Send Altimeter Values	
	helios.send(2051, string.format("%0.4f;%0.4f;%0.4f", mainPanelDevice:get_argument_value(355), mainPanelDevice:get_argument_value(354), mainPanelDevice:get_argument_value(352)))
	helios.send(2059, string.format("%0.2f;%0.2f;%0.2f;%0.3f", mainPanelDevice:get_argument_value(356), mainPanelDevice:get_argument_value(357), mainPanelDevice:get_argument_value(358), mainPanelDevice:get_argument_value(359)))		
end

function driver.processLowImportance(mainPanelDevice)

	li = helios.parseIndication(5)
	if li then
        helios.send(2092, string.format("%2s",helios.ensureString(li.ufc_left_position)))
        helios.send(2094, string.format("%7s",helios.ensureString(li.ufc_right_position)))
        helios.send(2095, string.format("%2s",string.sub(helios.ensureString(li.ufc_chnl_1_m) .. helios.ensureString(li.ufc_chnl_1_v),-2)))
		helios.send(2096, string.format("%2s",string.sub(helios.ensureString(li.ufc_chnl_2_m) .. helios.ensureString(li.ufc_chnl_2_v),-2)))
	end

 	local li = helios.parseIndication(6)
	if li then
		helios.send(2082, string.format("%-4s",helios.ensureString(li.ODU_Option_1_Text)))
		helios.send(2083, string.format("%-4s",helios.ensureString(li.ODU_Option_2_Text)))
		helios.send(2084, string.format("%-4s",helios.ensureString(li.ODU_Option_3_Text)))
		helios.send(2085, string.format("%-4s",helios.ensureString(li.ODU_Option_4_Text)))
		helios.send(2086, string.format("%-4s",helios.ensureString(li.ODU_Option_5_Text)))
		helios.send(2087, string.format("%1s",helios.ensureString(li.ODU_Option_1_Slc):gsub(":","!")))  -- ":" is reserved
		helios.send(2088, string.format("%1s",helios.ensureString(li.ODU_Option_2_Slc):gsub(":","!")))  -- ":" is reserved
		helios.send(2089, string.format("%1s",helios.ensureString(li.ODU_Option_3_Slc):gsub(":","!")))  -- ":" is reserved
		helios.send(2090, string.format("%1s",helios.ensureString(li.ODU_Option_4_Slc):gsub(":","!")))  -- ":" is reserved
		helios.send(2091, string.format("%1s",helios.ensureString(li.ODU_Option_5_Slc):gsub(":","!")))  -- ":" is reserved
		-- -- test command 00000000*2096=25:2095=13:2087=!:2088=!:2089=!:2090=!:2091=!:2082=BLUE:2083=FIN :2084=BIMA:2085=2019:2086=test:2094=123.567:2092=~0:2093=-:326=1:336=1:197=1:365=1:196=1: 
	end
 	local li = helios.parseIndication(7)  --V/UHF Radio and ACNIP
	if li then
		helios.send(2100, string.format("%2s",helios.ensureString(li.uvhf_channel)))

		helios.send(2101, string.format("%7s",helios.ensureString(li.uvhf_freq_left)))
		helios.send(2102, string.format("%s",helios.ensureString(li.acnip_1_label_mode)))
		helios.send(2103, string.format("%s",helios.ensureString(li.acnip_1_mode)))
		helios.send(2104, string.format("%s",helios.ensureString(li.acnip_1_label_code)))
		helios.send(2105, string.format("%s",helios.ensureString(li.acnip_1_code)))		
		helios.send(2106, string.format("%s",helios.ensureString(li.acnip_2_label_mode)))
		helios.send(2107, string.format("%s",helios.ensureString(li.acnip_2_mode)))
		helios.send(2108, string.format("%s",helios.ensureString(li.acnip_2_label_code)))
		helios.send(2109, string.format("%s",helios.ensureString(li.acnip_2_code)))
	end	

    helios.send(2001, string.format("%.0f",mainPanelDevice:get_argument_value(253) * 1000+mainPanelDevice:get_argument_value(254) * 100+mainPanelDevice:get_argument_value(255) * 10))     -- Engine Duct
    helios.send(2002, string.format("%.0f",mainPanelDevice:get_argument_value(256) * 10000+mainPanelDevice:get_argument_value(257) * 1000+mainPanelDevice:get_argument_value(258) * 100+mainPanelDevice:get_argument_value(259) * 10))     -- Engine RPM
    helios.send(2003, string.format("%.0f",mainPanelDevice:get_argument_value(260) * 1000+mainPanelDevice:get_argument_value(261) * 100+mainPanelDevice:get_argument_value(262) * 10))    -- Engine FF
    helios.send(2004, string.format("%.0f",mainPanelDevice:get_argument_value(263) * 1000+mainPanelDevice:get_argument_value(264) * 100+mainPanelDevice:get_argument_value(265) * 10)) -- Engine JPT
    helios.send(2005, string.format("%.0f",mainPanelDevice:get_argument_value(267) * 100+mainPanelDevice:get_argument_value(268) * 10)) -- Engine Stab
    helios.send(2006, string.format("%.0f",mainPanelDevice:get_argument_value(269) * 100+mainPanelDevice:get_argument_value(270) * 10)) -- Engine H2O
    helios.send(2019, string.format("%.4f",mainPanelDevice:get_argument_value(386) * 100+mainPanelDevice:get_argument_value(387) * 10)) -- SMC Fuze
    helios.send(2020, string.format("%.0f",mainPanelDevice:get_argument_value(392) * 1000+mainPanelDevice:get_argument_value(393) * 100+mainPanelDevice:get_argument_value(394) * 10))    -- SMC Interval
    helios.send(2022, string.format("%.0f",mainPanelDevice:get_argument_value(389) * 100+mainPanelDevice:get_argument_value(390) * 10))    -- SMC Quantity
    helios.send(2021, string.format("%.0f",mainPanelDevice:get_argument_value(391) * 10))    -- SMC Mult
    helios.send(2010, string.format("%.0f",mainPanelDevice:get_argument_value(367) * 10000+mainPanelDevice:get_argument_value(368) * 1000+mainPanelDevice:get_argument_value(369) * 100+mainPanelDevice:get_argument_value(370) * 10))    -- Fuel Total
    helios.send(2011, string.format("%.0f",mainPanelDevice:get_argument_value(371) * 10000+mainPanelDevice:get_argument_value(372) * 1000+mainPanelDevice:get_argument_value(373) * 100+mainPanelDevice:get_argument_value(374) * 10))    -- Fuel Left Tank
    helios.send(2012, string.format("%.0f",mainPanelDevice:get_argument_value(375) * 10000+mainPanelDevice:get_argument_value(376) * 1000+mainPanelDevice:get_argument_value(377) * 100+mainPanelDevice:get_argument_value(378) * 10))    -- Fuel Right Tank
    helios.send(2013, string.format("%.0f",mainPanelDevice:get_argument_value(381) * 10000+mainPanelDevice:get_argument_value(382) * 1000+mainPanelDevice:get_argument_value(383) * 100+mainPanelDevice:get_argument_value(384) * 10))    -- Fuel Bingo
    helios.send(2014, string.format("%.0f",mainPanelDevice:get_argument_value(455) * 100+mainPanelDevice:get_argument_value(456) * 10))    -- Flap Position
    helios.send(2015, string.format("%.0f",mainPanelDevice:get_argument_value(550) * 1000+mainPanelDevice:get_argument_value(551) * 100+mainPanelDevice:get_argument_value(552) * 10))    -- Pressure Brake
    helios.send(2016, string.format("%.0f",mainPanelDevice:get_argument_value(553) * 1000+mainPanelDevice:get_argument_value(554) * 100+mainPanelDevice:get_argument_value(555) * 10))    -- Pressure Hyd 1
    helios.send(2017, string.format("%.0f",mainPanelDevice:get_argument_value(556) * 1000+mainPanelDevice:get_argument_value(557) * 100+mainPanelDevice:get_argument_value(558) * 10))    -- Pressure Hyd 2
end

