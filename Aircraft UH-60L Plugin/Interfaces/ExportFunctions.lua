-- Exports.Lua from Helios UH-60L interface
-- Version: 1.6.6190.0000
-- Commit: 3ab7cb14cd15532aefc5eb573f816a0a0e2fd63a
--
-- ** This code requires patches to be applied to the UH-60L module in order to identify exported data **
--

driver.state = {}
driver.NoPatchesMessageAlreadySent = false
function driver.processHighImportance(mainPanelDevice)

	-- Pilot VSI ball combined movement values
	helios.send(2049, string.format("%0.3f;0.0;%0.3f", mainPanelDevice:get_argument_value(420), mainPanelDevice:get_argument_value(421)))
	-- Copilot VSI ball combined movement values
	helios.send(2050, string.format("%0.3f;0.0;%0.3f", mainPanelDevice:get_argument_value(433), mainPanelDevice:get_argument_value(434)))

    -- Pilot Barometric Altimeter
    helios.send(
        2051,
        string.format(
            "%0.4f;%0.4f;%0.4f",
            mainPanelDevice:get_argument_value(62),
            mainPanelDevice:get_argument_value(61),
            mainPanelDevice:get_argument_value(60)
        )
    )
    helios.send(
        2052,
        string.format(
            "%0.2f;%0.2f;%0.2f;%0.3f",
            mainPanelDevice:get_argument_value(64),
            mainPanelDevice:get_argument_value(65),
            mainPanelDevice:get_argument_value(66),
            mainPanelDevice:get_argument_value(67)
        )
    )
    -- Copilot Barometric Altimeter
    helios.send(
        2053,
        string.format(
            "%0.4f;%0.4f;%0.4f",
            mainPanelDevice:get_argument_value(72),
            mainPanelDevice:get_argument_value(71),
            mainPanelDevice:get_argument_value(70)
        )
    )
    helios.send(
        2054,
        string.format(
            "%0.2f;%0.2f;%0.2f;%0.3f",
            mainPanelDevice:get_argument_value(74),
            mainPanelDevice:get_argument_value(75),
            mainPanelDevice:get_argument_value(76),
            mainPanelDevice:get_argument_value(77)
        )
    )
	-- Pilot RADAR Altimeter
    helios.send(
        2055,
        string.format(
            "%0.2f;%0.2f;%0.2f;%0.3f",
            mainPanelDevice:get_argument_value(174),
            mainPanelDevice:get_argument_value(175),
            mainPanelDevice:get_argument_value(176),
            mainPanelDevice:get_argument_value(177)
        )
    )
	-- Copilot RADAR Altimeter
    helios.send(
        2056,
        string.format(
            "%0.2f;%0.2f;%0.2f;%0.3f",
            mainPanelDevice:get_argument_value(187),
            mainPanelDevice:get_argument_value(188),
            mainPanelDevice:get_argument_value(189),
            mainPanelDevice:get_argument_value(190)
        )
    )
end

driver.UH60Lindicator = {
                            {
                                {0,50,100,150,200,250,300,350,400,450,500,550,600,650,700,750,800,850,900,950,1000,1050,1100,1150,1200,1250,1300,1350,1400,1450,1500},
							    {0,50,100,150,200,250,300,350,400,450,500,550,600,650,700,750,800,850,900,950,1000,1050,1100,1150,1200,1250,1300,1350,1400,1450,1500},
							    {-50,-40,-30,-20,-10,0,10,20,30,40,45,50,55,60,65,70,75,80,85,90,95,100,105,110,115,120,130,140,150,160},
							    {0,5,10,15,20,25,30,32.5,35,37.5,40,42.5,45,47.5,50,52.5,55,57.5,60,62.5,65,67.5,70,80,90,100,110,130,150,170},
							    {-50,-40,-30,-20,-10,0,10,20,30,40,50,60,70,75,80,85,90,95,100,105,110,115,120,125,130,135,150,160,170},
							    {-50,-40,-30,-20,-10,0,10,20,30,40,50,60,70,75,80,85,90,95,100,105,110,115,120,125,130,135,150,160,170},
							    {10,15,20,25,30,35,40,42.5,45,47.5,50,52.5,55,57.5,60,62.5,65,67.5,70,72.5,75,77.5,80,82.5,85,87.5,90,100,110,120},
							    {10,15,20,25,30,35,40,42.5,45,47.5,50,52.5,55,57.5,60,62.5,65,67.5,70,72.5,75,77.5,80,82.5,85,87.5,90,100,110,120},
							    {0,50,100,150,200,250,300,350,400,425,450,475,500,525,550,575,600,625,650,675,700,725,750,775,800,825,850,875,900,925},
							    {0,50,100,150,200,250,300,350,400,425,450,475,500,525,550,575,600,625,650,675,700,725,750,775,800,825,850,875,900,925},
							    {0,10,20,30,40,45,50,55,60,65,70,72,74,76,78,80,82,84,86,88,90,92,94,96,98,100,102,104,106,108},
							    {0,10,20,30,40,45,50,55,60,65,70,72,74,76,78,80,82,84,86,88,90,92,94,96,98,100,102,104,106,108}
                            },
							{
                                {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40},
							    {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40},
							    {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40},
							    {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29},
							    {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29}
                            },
							{
                                {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40},
							    {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40},
							    {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40},
							    {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29},
							    {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29}
                            }
                       }

driver.displayNames = {"TotalFuel","TGT1","TGT2","Ng1","Ng2","Torque1","Torque2"}
driver.barNames = {
					{"FUEL_L", "FUEL_R", "XMSNTEMP", "XMSNPRESS", "E1OILTEMP", "E2OILTEMP", "E1PRESS", "E2PRESS", "TGT1", "TGT2", "NG1", "NG2"},
					{"PLT_E1NP", "PLT_RRPM", "PLT_E2NP", "PLT_E1TRQ", "PLT_E2TRQ"},
					{"CPLT_E1NP", "CPLT_RRPM", "CPLT_E2NP", "CPLT_E1TRQ", "CPLT_E2TRQ"}
				  }
driver.state.indications = {}
function lprint(s)
	log.write("Helios.Export", log.INFO, s)
end
function driver.processLowImportance(mainPanelDevice)
local li
	do -- constrain variable scope
		-- Indicator 0 for the Center panel bar graphs and numeric displays
		li = helios.parseIndication(0)
		if li ~= driver.state.indications[0] then
			if li["FuelAmount"] == nil then 
				if driver.NoPatchesMessageAlreadySent == false then
					log.write("Helios.Export",log.ERROR,"Driver: UH-60L - Patches appear not to be applied to module.")
					driver.NoPatchesMessageAlreadySent = true
				end
			else
				for j = 1,#driver.barNames[1],1 do
					local n = {}
					for k, v in pairs(li) do
						if k:find(driver.barNames[1][j].."_segment_") ~= nil then
							local a = k:gsub(driver.barNames[1][j].."_segment_([%d.-]+)%s*","%1")
							table.insert(n, tonumber(a))
						end
					end
					if n ~= nil then
						table.sort(n)
						local startVal, endVal, startSeg, endSeg
						startVal = n[1]
						endVal = n[#n]
						for m = 1, #driver.UH60Lindicator[1][j] do
							if driver.UH60Lindicator[1][j][m] == n[1] then
								startSeg = m
							end
							if driver.UH60Lindicator[1][j][m] == n[#n] then
								endSeg = m
								break
							end
						end
						if startVal == nil or endVal== nil or startSeg == nil or endSeg == nil then 
							-- lprint(string.format("Nil Value from %s;%s;%s;%s;%s", driver.barNames[1][j], helios.ensureString(startVal), helios.ensureString(endVal), helios.ensureString(startSeg), helios.ensureString(endSeg)))
							helios.send(2064+j,"0;0;0;0") -- bar gauge						
						else
							helios.send(2064+j,string.format(string.format("%.1f;%.1f;%d;%s", startVal, endVal, startSeg, endSeg))) -- bar gauge
						end
					end
				end
				helios.send(2060,string.format("%s", helios.ensureString(li["FuelAmount"])))
				helios.send(2061,string.format("%s", helios.ensureString(li["E1TGT"])))
				helios.send(2062,string.format("%s", helios.ensureString(li["E2TGT"])))
				helios.send(2063,string.format("%s", helios.ensureString(li["E1NG"])))
				helios.send(2064,string.format("%s", helios.ensureString(li["E2NG"])))		
			end
			driver.state.indications[0] = li			
		end
	end
	
	for jj= 2,3,1 do
		-- Indicator 1 and 2 for the RPM and Torque bar graphs and numeric displays for Pilot and Copilot
		li = helios.parseIndication(jj-1)
		if li ~= driver.state.indications[jj] and li ~= nil then		
			for j = 1,#driver.barNames[jj],1 do
				local n = {}
				for k, v in pairs(li) do
					if k:find(driver.barNames[jj][j].."_segment_") ~= nil then
						local a = k:gsub(driver.barNames[jj][j].."_segment_([%d.-]+)%s*","%1")
						table.insert(n, tonumber(a))
					end
				end
				if n ~= nil then
					table.sort(n)
					local startVal, endVal, startSeg, endSeg
					startVal = n[1]
					endVal = n[#n]
					for m = 1, #driver.UH60Lindicator[jj][j] do
						if driver.UH60Lindicator[jj][j][m] == n[1] then
							startSeg = m
						end
						if driver.UH60Lindicator[jj][j][m] == n[#n] then
							endSeg = m
							break
						end
					end
					if startVal == nil or endVal== nil or startSeg == nil or endSeg == nil then 
						-- lprint(string.format("Nil Value from %s;%s;%s;%s;%s", driver.barNames[jj][j], helios.ensureString(startVal), helios.ensureString(endVal), helios.ensureString(startSeg), helios.ensureString(endSeg)))
						helios.send(2078+((jj-2)*7)+j,"0;0;0;0") -- bar gauge						
					else
						helios.send(2078+((jj-2)*7)+j,string.format("%.1f;%.1f;%d;%s", startVal, endVal, startSeg, endSeg)) -- bar gauge
					end
				end
			end
			helios.send(2077+((jj-2)*7),string.format("%s", helios.ensureString(li["E1TRQ_Text"])))
			helios.send(2078+((jj-2)*7),string.format("%s", helios.ensureString(li["E2TRQ_Text"])))			
			driver.state.indications[jj] = li
		end
	end
	do
		jj = 5
		local results={}
		-- Indicator 5 for the nav computer
		li = list_indication(jj)
		if li ~= driver.state.indications[jj] then
		    local z = li:gmatch("-----------------------------------------\n([^\n]+)\n([^\n]*)\n")
			local currentLine = 0
			while true do
				local name, value = z()
				if not name then
					break
				end
				if value ~= "" then
					if  currentLine < 4 then
						results["Line"..currentLine]= string.format("%-16s",value)
						currentLine = currentLine + 1
					else
						if value == "end" or value == "more" then
							results["Line3"] = string.format("%-12s%4s",results["Line3"]:sub(1,12),value)
						else
							results["Line1"] = string.format("%-11s%5s",results["Line1"]:sub(1,11),value)
						end

					end
				end
			end
			for ii=0,3,1 do
				if results["Line"..ii] then
					helios.send(2091+ii,string.format("%-16s", helios.ensureString(results["Line"..ii]:gsub(":","!"))))
				end
			end
			driver.state.indications[jj] = li
		end
	end
	
	for jj = 6,7,1 do
	-- These are the two chronographs  Reserve 4 codes for each Codes run 2095 - 2103 
		li = list_indication(jj)
		local results={}
		local currentLine = 0

		if li ~= driver.state.indications[jj] then
		    local z = li:gmatch("-----------------------------------------\n([^\n]+)\n([^\n]*)\n")
			while true do
				local name, value = z()
				if not name then
					break
				end
				if value ~= "" and value ~= ":" then
					if  currentLine < 3 then
						if type(value) == "number" then 
							results["Line"..currentLine]= string.format("%02d",value)
						else
							results["Line"..currentLine]= "  "
						end
						currentLine = currentLine + 1
					else
						if currentLine == 3 then
							results["Line3"] = string.format("%s",value)
						else
							results["Line4"] = value
						end
					end
				end

			end
			helios.send(2096+((jj-6)*2),string.format("%s!%s %s", helios.ensureString(results["Line0"]), helios.ensureString(results["Line1"]), helios.ensureString(results["Line2"])))			
			helios.send(2097+((jj-6)*2),string.format("%s", helios.ensureString(results["Line3"])))			

			driver.state.indications[jj] = li
		end
	end
	-- 8 are the frequency preset frequencies
	for jj = 9,10,1 do
		local results={}
		-- Indicators 9 and 10 are the AN/ARC-201 FM 1 &  2 displays
		li = list_indication(jj)
		if li ~= driver.state.indications[jj] then
		    local z = li:gmatch("-----------------------------------------\n([^\n]+)\n([^\n]*)\n")
			local currentLine = 0
			while true do
				local name, value = z()
				if not name then
					break
				end
				if value ~= "" then
						results["Line"]= string.format("%s",value)
				end
			end
			if results["Line"] then
				helios.send(2104+jj-9,string.format("%s", helios.ensureString(results["Line"]:gsub(":","!"))))
			end
			driver.state.indications[jj] = li
		end
	end	

	do
		jj = 11
		local results={}
		-- Indicator 11 for the AHRU display
		li = list_indication(jj)
		if li ~= driver.state.indications[jj] then
		    local z = li:gmatch("-----------------------------------------\n([^\n]+)\n([^\n]*)\n")
			local currentLine = 0
			while true do
				local name, value = z()
				if not name then
					break
				end
				if value ~= "" then
						results["Line"]= string.format("%-16s",value)
				end
			end
			if results["Line"] then
				helios.send(2106,string.format("%-16s", helios.ensureString(results["Line"]:gsub(":","!"))))
			end
			driver.state.indications[jj] = li
		end
	end	
	do
		jj = 12
		local results={}
		-- Indicator 12 for the Aux Fuel display
		li = list_indication(jj)
		if li ~= driver.state.indications[jj] then
		    local z = li:gmatch("-----------------------------------------\n([^\n]+)\n([^\n]*)\n")
			local currentLine = 0
			while true do
				local name, value = z()
				if not name then
					break
				end
				if value ~= "" then
						results["AuxFuel"..currentLine]= string.format("%s",value)
						currentLine = currentLine + 1
				end
			end
			for ii=0,3,1 do
				if results["AuxFuel"..ii] then
					helios.send(2107+ii,string.format("%s", helios.ensureString(results["AuxFuel"..ii]:gsub(":","!"):gsub("%%",""))))
				end
			end
			driver.state.indications[jj] = li
		end
	end	
	-- next code is 2111
end
function driver.clearState() 
	log.write("Helios.Export",log.DEBUG,"Driver: UH-60L - Driver clearState() called")
	driver.state.indications = {}
end