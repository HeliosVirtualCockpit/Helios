-- Exports.Lua from Helios Mirage-2000C interface
function driver.processHighImportance(mainPanelDevice)
	-- Send Altimeter Values	
	-- helios.send(2051, string.format("%0.4f;%0.4f;%0.4f", mainPanelDevice:get_argument_value(355), mainPanelDevice:get_argument_value(354), mainPanelDevice:get_argument_value(352)))
	-- helios.send(2059, string.format("%0.2f;%0.2f;%0.2f;%0.3f", mainPanelDevice:get_argument_value(356), mainPanelDevice:get_argument_value(357), mainPanelDevice:get_argument_value(358), mainPanelDevice:get_argument_value(359)))		
end

function driver.processLowImportance(mainPanelDevice)

    -- PCA_UR structured data
    li = helios.parseIndication(4) -- 4 for PCA_UR
    if li then
        helios.send(2060, string.format("%-3s%-3s%-3s%-3s%-3s", helios.ensureString(li.PCA_LCD_1_0), helios.ensureString(li.PCA_LCD_1_1), helios.ensureString(li.PCA_LCD_1_2), helios.ensureString(li.PCA_LCD_1_3), helios.ensureString(li.PCA_LCD_1_4)))
    end
    -- PCA_BR structured data
    li = helios.parseIndication(5) -- 5 for PCA_BR
    if li then
        helios.send(2061, string.format("%-3s%-3s%-3s%-3s%-3s", helios.ensureString(li.PCA_LCD_2_0), helios.ensureString(li.PCA_LCD_2_1), helios.ensureString(li.PCA_LCD_2_2), helios.ensureString(li.PCA_LCD_2_3), helios.ensureString(li.PCA_LCD_2_4)))
    end
    -- VHF Comm structured data
    li = helios.parseIndication(8)
    if li then
        helios.send(2062, string.format("%.8s", helios.ensureString(li.text_COM_VHF):reverse():sub(1,8):reverse()))
    end
    -- UHF Comm structured data
    li = helios.parseIndication(7)
    if li then
        helios.send(2063, string.format("%.8s", helios.ensureString(li.text_COM_UHF1):reverse():sub(1,8):reverse()))
        helios.send(2064, string.format("%.8s", helios.ensureString(li.text_COM_UHF2):reverse():sub(1,8):reverse()))
    end
    -- PPA structured data
    li = helios.parseIndication(6)
    if li then
        helios.send(2065, string.format("%-2s", helios.ensureString(li.text_PPA_QTY)))
        helios.send(2066, string.format("%-2s", helios.ensureString(li.text_PPA_INT)))
    end
    -- Fuel structured data
    li = helios.parseIndication(3)
    if li then
        helios.send(2067, string.format("%-3s", helios.ensureString(li.txt_fuel_g)))
    end
    -- PCN U structured data
    li = helios.parseIndication(9)
    if li then
        helios.send(2068, string.format("%6s", helios.ensureString(li.PCN_UL_DIGITS)))
        helios.send(2069, string.format("%6s", helios.ensureString(li.PCN_UL_POINTS)))
        helios.send(2070, string.format("%6s", helios.ensureString(li.PCN_UR_DIGITS)))
        helios.send(2071, string.format("%6s", helios.ensureString(li.PCN_UR_POINTS)))
        helios.send(2072, string.format("%1d", li["PCN_UL_N"] and 1 or 0))
        helios.send(2073, string.format("%1d", li["PCN_UL_S"] and 1 or 0))
        helios.send(2074, string.format("%1d", li["PCN_UR_E"] and 1 or 0))
        helios.send(2075, string.format("%1d", li["PCN_UR_W"] and 1 or 0))
        helios.send(2076, string.format("%1d", li["PCN_UL_P"] and 1 or 0))
        helios.send(2077, string.format("%1d", li["PCN_UL_M"] and 1 or 0))
        helios.send(2078, string.format("%1d", li["PCN_UR_P"] and 1 or 0))
        helios.send(2079, string.format("%1d", li["PCN_UR_M"] and 1 or 0))
    end    
    -- PCN B structured data
    li = helios.parseIndication(10)
    if li then
        helios.send(2080, string.format("%s", helios.ensureString(li.PCN_BL_DIGITS)))
        helios.send(2081, string.format("%s", helios.ensureString(li.PCN_BR_DIGITS)))
    end
    -- EVF structured data
    li = helios.parseIndication(11)
    if li then
        helios.send(2082, string.format("%-2s", helios.ensureString(li["evf-digits"])))
    end

end
