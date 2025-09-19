-- Exports.Lua from Helios A-10C Interface
function driver.processHighImportance(mainPanelDevice)
	-- Send Altimeter Values	
	helios.send(2051, string.format("%0.4f;%0.4f;%0.5f", mainPanelDevice:get_argument_value(52), mainPanelDevice:get_argument_value(53), mainPanelDevice:get_argument_value(51)))
	helios.send(2059, string.format("%0.2f;%0.2f;%0.2f;%0.3f", mainPanelDevice:get_argument_value(56), mainPanelDevice:get_argument_value(57), mainPanelDevice:get_argument_value(58), mainPanelDevice:get_argument_value(59)))		
	-- Send ADI Ball Values	
	helios.send(2050, string.format("%0.4f;0.0;%0.4f", mainPanelDevice:get_argument_value(17), mainPanelDevice:get_argument_value(18)))
	-- Calcuate HSI Value
	helios.send(2029, string.format("%0.2f;%0.2f;%0.4f", mainPanelDevice:get_argument_value(29), mainPanelDevice:get_argument_value(30), mainPanelDevice:get_argument_value(31)))
	-- Calculate Total Fuel
	helios.send(2090, string.format("%0.2f;%0.2f;%0.5f", mainPanelDevice:get_argument_value(90), mainPanelDevice:get_argument_value(91), mainPanelDevice:get_argument_value(92)))
	-- instruments pure values												  
	--helios.send(3012, string.format("%0.3f", mainPanelDevice:get_argument_value(12)))
end

function driver.processLowImportance(mainPanelDevice)
	-- Get Radio Frequencies
	local lUHFRadio = GetDevice(54)
	helios.send(2000, string.format("%7.3f", lUHFRadio:get_frequency()/1000000))
	-- ILS Frequency
	--helios.send(2251, string.format("%0.1f;%0.1f", mainPanelDevice:get_argument_value(251), mainPanelDevice:get_argument_value(252)))
	-- TACAN Channel
	helios.send(2263, string.format("%0.2f;%0.2f;%0.2f", mainPanelDevice:get_argument_value(263), mainPanelDevice:get_argument_value(264), mainPanelDevice:get_argument_value(265)))
end