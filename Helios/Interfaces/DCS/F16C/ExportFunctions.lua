-- Exports.Lua from Helios F-16C Viper interface

function driver.processHighImportance(mainPanelDevice)
	-- Send Altimeter Values	
	helios.send(2051, string.format("%0.4f;%0.4f;%0.4f", mainPanelDevice:get_argument_value(606), mainPanelDevice:get_argument_value(605), mainPanelDevice:get_argument_value(479)))
	helios.send(2059, string.format("%0.2f;%0.2f;%0.3f", mainPanelDevice:get_argument_value(607), mainPanelDevice:get_argument_value(608), mainPanelDevice:get_argument_value(609)))		
end

function driver.processLowImportance(mainPanelDevice)

end