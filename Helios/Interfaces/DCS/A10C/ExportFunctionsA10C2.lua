-- Exports.Lua from Helios A-10C II Interface
function driver.processHighImportance(mainPanelDevice)
	-- Send Altimeter Values	
	helios.send(2051, string.format("%0.4f;%0.4f;%0.5f", mainPanelDevice:get_argument_value(52), mainPanelDevice:get_argument_value(53), mainPanelDevice:get_argument_value(51)))
	helios.send(2059, string.format("%0.2f;%0.2f;%0.2f;%0.3f", mainPanelDevice:get_argument_value(56), mainPanelDevice:get_argument_value(57), mainPanelDevice:get_argument_value(58), mainPanelDevice:get_argument_value(59)))		
	-- Calcuate HSI Value
	helios.send(2029, string.format("%0.2f;%0.2f;%0.4f", mainPanelDevice:get_argument_value(29), mainPanelDevice:get_argument_value(30), mainPanelDevice:get_argument_value(31)))
	-- Calculate Total Fuel
	helios.send(2090, string.format("%0.2f;%0.2f;%0.5f", mainPanelDevice:get_argument_value(90), mainPanelDevice:get_argument_value(91), mainPanelDevice:get_argument_value(92)))
	-- instruments pure values												  
	--helios.send(3012, string.format("%0.3f", mainPanelDevice:get_argument_value(12)))
	local li
    li = helios.parseIndication(7) -- CMSP
    if li then
        helios.send(2400, string.format("%-16s", helios.ensureString(li.txt_UP)))
        helios.send(2401, string.format("%-4s", helios.ensureString(li.txt_DOWN1)))
        helios.send(2402, string.format("%-4s", helios.ensureString(li.txt_DOWN2)))
        helios.send(2403, string.format("%-4s", helios.ensureString(li.txt_DOWN3)))
        helios.send(2404, string.format("%-4s", helios.ensureString(li.txt_DOWN4)))
    end
    li = helios.parseIndication(8) -- CMSC
    if li then
        helios.send(2405, string.format("%-8s", helios.ensureString(li.txt_MWS)))
        helios.send(2406, string.format("%-8s", helios.ensureString(li.txt_CHAFF_FLARE)))
        helios.send(2407, string.format("%-8s", helios.ensureString(li.txt_JMR)))
    end
    li = helios.parseIndication(18) -- ARC-210 (Work in progress)
    if li then
        helios.send(2414, string.format("%s%s%s", helios.ensureString(li["freq_label_mhz"]), helios.ensureString(li["dot_mark"]),helios.ensureString(li["freq_label_khz"])))
        helios.send(2415, string.format("%s", helios.ensureString(li["PREV"])))                 -- Previous Label
        helios.send(2416, string.format("%s", helios.ensureString(li["prev_manual_freq"])))     -- Previous Frequency
        helios.send(2417, string.format("%s", helios.ensureString(li["modulation_label"])))     -- AM/FM mode
        helios.send(2418, string.format("%s", helios.ensureString(li["ky_submode_label"])))
        helios.send(2419, string.format("%s", helios.ensureString(li["comsec_mode"])))          -- Communications Security Mode
        helios.send(2420, string.format("%s", helios.ensureString(li["comsec_submode"])))       -- Communications Security Sub-Mode
        helios.send(2421, string.format("%s", helios.ensureString(li["txt_RT"])))               -- RT label
    end
end

function driver.processLowImportance(mainPanelDevice)
	-- Get Radio Frequencies
	local lUHFRadio = GetDevice(54)
	helios.send(2000, string.format("%7.3f", lUHFRadio:get_frequency()/1000000))
	-- ILS Frequency
	--helios.send(2251, string.format("%0.1f;%0.1f", mainPanelDevice:get_argument_value(251), mainPanelDevice:get_argument_value(252)))
	-- TACAN Channel
	helios.send(2263, string.format("%0.2f;%0.2f;%0.2f", mainPanelDevice:get_argument_value(263), mainPanelDevice:get_argument_value(264), mainPanelDevice:get_argument_value(265)))
	local li
    li = helios.parseIndication(4) -- Clock
    if li then
        helios.send(2408, string.format("%s%s", helios.ensureString(li.txtC), helios.ensureString(li.txtET)))
        helios.send(2409, string.format("%-2s.%-2s", helios.ensureString(li.txtHours), helios.ensureString(li.txtMinutes)))
        helios.send(2410, string.format("%-2s", helios.ensureString(li.txtSeconds)))
    end
    li = helios.parseIndication(10) -- Channel Number
    if li then
        helios.send(2411, string.format("%-2s", helios.ensureString(li.txtPresetChannel)))
    end
    li = helios.parseIndication(11) -- Radio Frequency
    if li then
		local lUHFDisplay = helios.ensureString(li.txtFreqStatus)
        helios.send(2412, string.format("%.3s.%.3s", lUHFDisplay, lUHFDisplay:sub(-3)))
    end
    li = helios.parseIndication(12) -- UHF Repeater
    if li then
        helios.send(2413, string.format("%-7s", helios.ensureString(li.txtFreqStatus) .. helios.ensureString(li.txtPresetChannel)))
    end	
end