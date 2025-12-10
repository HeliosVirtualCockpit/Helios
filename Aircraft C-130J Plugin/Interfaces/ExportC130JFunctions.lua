-- Exports.Lua from Helios C-130J interface

function driver.processHighImportance(mainPanelDevice)

end

function driver.processLowImportance(mainPanelDevice)

    li = helios.parseIndication(16) -- 16 Pilot Ref Mode Heading
    if li then
        helios.send(2900, string.format("%s", helios.ensureString(li.ref_mode_value):gsub(":", "!")))
	else
		helios.send(2900,"")
    end
    li = helios.parseIndication(17) -- 17 Copilot Ref Mode Heading
    if li then
        helios.send(2901, string.format("%s", helios.ensureString(li.ref_mode_value):gsub(":", "!")))
	else
		helios.send(2901,"")
    end

    li = helios.parseIndication(23) -- 23 DC Voltage
    if li then
	        helios.send(2902, string.format("%-2s.%s", helios.ensureString(li.dc_voltage):gsub(":", "!"), helios.ensureString(li.dc_voltage_dec):gsub(":", "!")))
	else
	        helios.send(2902, "    ")
    end
    helios.send(2903 ,ExtractIndicationValue(16, 5))		-- Pilot Ref Mode
	helios.send(2904 ,ExtractIndicationValue(17, 5))		-- Copilot Ref Mode
	helios.send(2905 ,ExtractIndicationValue(24, 5))		-- Fuel Total
	helios.send(2906 ,ExtractIndicationValue(25, 4))		-- Fuel 1 Main
	helios.send(2907 ,ExtractIndicationValue(26, 4))		-- Fuel 2 Main
	helios.send(2908 ,ExtractIndicationValue(27, 4))		-- Fuel 3 Main
	helios.send(2909 ,ExtractIndicationValue(28, 4))		-- Fuel 4 Main
	helios.send(2910 ,ExtractIndicationValue(29, 4))		-- Fuel L Aux
	helios.send(2911 ,ExtractIndicationValue(30, 4))		-- Fuel R Aux
	helios.send(2912 ,ExtractIndicationValue(31, 4))		-- Fuel L Ext
	helios.send(2913 ,ExtractIndicationValue(32, 4))		-- Fuel R Ext
	helios.send(2914 ,ExtractIndicationValue(33, 4))		-- APU % RPM
	helios.send(2915 ,ExtractIndicationValue(34, 4))		-- APU EGT
	helios.send(2916 ,ExtractIndicationValue(35, 3))		-- Bleed Air Pressure
	li = helios.parseIndication(36) -- Flight Air Con
    if li then
	        helios.send(2917, string.format("%-2s", helios.ensureString(li.act):gsub(":", "!")))
	        helios.send(2918, string.format("%-2s", helios.ensureString(li.tgt):gsub(":", "!")))
	else
	        helios.send(2917, "  ")
	        helios.send(2918, "  ")
    end

	li = helios.parseIndication(37) -- Cargo Air Con
    if li then
	        helios.send(2919, string.format("%-2s", helios.ensureString(li.act):gsub(":", "!")))
	        helios.send(2920, string.format("%-2s", helios.ensureString(li.tgt):gsub(":", "!")))
	else
	        helios.send(2919, "  ")
	        helios.send(2920, "  ")
    end
	helios.send(2921 ,ExtractIndicationValue(38, 5))		-- Pressurization Rate
	helios.send(2922 ,ExtractIndicationValue(39, 5))		-- Pressurization Cabin Alt
	li = helios.parseIndication(40) -- Pressurization Difference
    if li then
	        helios.send(2923, string.format("%-2s.%s", helios.ensureString(li.diff_press):gsub(":", "!"), helios.ensureString(li.dc_voltage_dec):gsub(":", "!")))
	else
	        helios.send(2923, "    ")
    end
	helios.send(2924 ,ExtractIndicationValue(41, 5))		-- LGD/CONST					 
	helios.send(2925 ,ExtractIndicationValue(42, 3))		-- Fuel Pressure				 
	helios.send(2926 ,ExtractIndicationValue(43, 4))		-- Aux Hydraulic Pump
	--[[
    li = helios.encodeIndication(??) -- ARC-210
    if li then
            helios.send(2927, string.format("%s", li))      -- Encode and send everything
    end
	]]
end

function ExtractIndicationValue(ii, just)
		li = parse_indication(ii)
		if li then
			for _, v in pairs(li) do
			  if v ~= nil and v ~= "" then
				return(string.format("%" .. just .. "s", v):gsub(":", "!"))
			  end
			end
		end	
	return ""
end