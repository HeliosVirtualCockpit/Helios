-- Exports.Lua from Helios C-130J interface

function driver.processHighImportance(mainPanelDevice)
	helios.send(2960, string.format("%0.4f;0.0;%0.4f", mainPanelDevice:get_argument_value(122), mainPanelDevice:get_argument_value(123))) -- ADI Ball

end

function driver.processLowImportance(mainPanelDevice)
    li = helios.parseIndication(16) -- 16 Pilot Ref Mode Heading
    if li then
		if li.ref_symbol_period then 
	        helios.send(2900, string.format("%3s.%1s", helios.ensureString(li.ref_mode_value):sub(1,-2):gsub(":", "!"),helios.ensureString(li.ref_mode_value):sub(-1):gsub(":", "!")))
		else
	        helios.send(2900, string.format("%5s", helios.ensureString(li.ref_mode_value):gsub(":", "!")))
		end
	else
		helios.send(2900,"     ")
    end
    li = helios.parseIndication(17) -- 17 Copilot Ref Mode Heading
    if li then
		if li.ref_symbol_period then 
	        helios.send(2901, string.format("%3s.%1s", helios.ensureString(li.ref_mode_value):sub(1,-2):gsub(":", "!"),helios.ensureString(li.ref_mode_value):sub(-1):gsub(":", "!")))
		else
	        helios.send(2901, string.format("%5s", helios.ensureString(li.ref_mode_value):gsub(":", "!")))
		end
	else
		helios.send(2901,"     ")
    end

    li = helios.parseIndication(23) -- 23 DC Voltage  09/01/26 note: this requires a patch to operate correctly
    if li then
	        helios.send(2902, string.format("%-2s.%s", helios.ensureString(li.dc_voltage):gsub(":", "!"), helios.ensureString(li.dc_voltage_dec):gsub(":", "!")))
	else
	        helios.send(2902, "    ")
    end
	ExtractFuelIndicationValue(2940, 24, 5)		-- Fuel Total  09/01/26 note: this requires a patch to operate correctly
	ExtractFuelIndicationValue(2942, 25, 4)		-- Fuel 1 Main 09/01/26 note: this requires a patch to operate correctly
	ExtractFuelIndicationValue(2944, 26, 4)		-- Fuel 2 Main 09/01/26 note: this requires a patch to operate correctly
	ExtractFuelIndicationValue(2946, 27, 4)		-- Fuel 3 Main 09/01/26 note: this requires a patch to operate correctly
	ExtractFuelIndicationValue(2948, 28, 4)		-- Fuel 4 Main 09/01/26 note: this requires a patch to operate correctly
	ExtractFuelIndicationValue(2950, 29, 4)		-- Fuel L Aux  09/01/26 note: this requires a patch to operate correctly
	ExtractFuelIndicationValue(2952, 30, 4)		-- Fuel R Aux  09/01/26 note: this requires a patch to operate correctly
	ExtractFuelIndicationValue(2954, 31, 4)		-- Fuel L Ext  09/01/26 note: this requires a patch to operate correctly
	ExtractFuelIndicationValue(2956, 32, 4)		-- Fuel R Ext  09/01/26 note: this requires a patch to operate correctly

	helios.send(2914, ExtractIndicationValue(33, 4))		-- APU % RPM
	helios.send(2915, ExtractIndicationValue(34, 4))		-- APU EGT
	helios.send(2916, ExtractIndicationValue(35, 3))		-- Bleed Air Pressure
	li = helios.parseIndication(36) -- Flight Air Con   09/01/26 note: this requires a patch to operate correctly
    if li then
	        helios.send(2917, string.format("%-2s", helios.ensureString(li.act):gsub(":", "!")))
	        helios.send(2918, string.format("%-2s", helios.ensureString(li.tgt):gsub(":", "!")))
	else
	        helios.send(2917, "  ")
	        helios.send(2918, "  ")
    end

	li = helios.parseIndication(37) -- Cargo Air Con  09/01/26 note: this requires a patch to operate correctly
    if li then
	        helios.send(2919, string.format("%-2s", helios.ensureString(li.act):gsub(":", "!")))
	        helios.send(2920, string.format("%-2s", helios.ensureString(li.tgt):gsub(":", "!")))
	else
	        helios.send(2919, "  ")
	        helios.send(2920, "  ")
    end
	helios.send(2921 ,ExtractIndicationValue(38, 5))		-- Pressurization Rate
	helios.send(2922 ,ExtractIndicationValue(39, 5))		-- Pressurization Cabin Alt
	li = helios.parseIndication(40) -- Pressurization Difference   09/01/26 note: this requires a patch to operate correctly
    if li then
	        helios.send(2923, string.format("%-2s.%s", helios.ensureString(li.diff_press):gsub(":", "!"), helios.ensureString(li.diff_press_dec):gsub(":", "!")))
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
		li = helios.parseIndication(ii)
		if li then
			for _, v in pairs(li) do
			  if v ~= nil and v ~= "" then
				return(string.format("%" .. just .. "s", v):gsub(":", "!"))
			  end
			end
		end	
	return ""
end

function ExtractFuelIndicationValue(argId, ii, just)
		li = helios.parseIndication(ii)
		if li then
			helios.send(argId, string.format("%" .. just .. "s", helios.ensureString(li.amt):gsub(":", "!")))
			helios.send(argId + 1, string.format("%" .. just .. "s", helios.ensureString(li.transfer):gsub(":", "!")))
		end	
end
