-- Exports.Lua from Helios UH-60L interface

function driver.processHighImportance(mainPanelDevice)
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
end

driver.UH60Lindicator = {{{0,50,100,150,200,250,300,350,400,450,500,550,600,650,700,750,800,850,900,950,1000,1050,1100,1150,1200,1250,1300,1350,1400,1450},
							{0,50,100,150,200,250,300,350,400,450,500,550,600,650,700,750,800,850,900,950,1000,1050,1100,1150,1200,1250,1300,1350,1400,1450},
							{-50,-40,-30,-20,-10,0,10,20,30,40,45,50,55,60,65,70,75,80,85,90,95,100,105,110,115,120,130,140,150,160},
							{0,5,10,15,20,25,30,32.5,35,37.5,40,42.5,45,47.5,50,52.5,55,57.5,60,62.5,65,67.5,70,80,90,100,110,130,150,170},
							{-50,-40,-30,-20,-10,0,10,20,30,40,50,60,70,75,80,85,90,95,100,105,110,115,120,125,130,135,150,160,170},
							{-50,-40,-30,-20,-10,0,10,20,30,40,50,60,70,75,80,85,90,95,100,105,110,115,120,125,130,135,150,160,170},
							{10,15,20,25,30,35,40,42.5,45,47.5,50,52.5,55,57.5,60,62.5,65,67.5,70,72.5,75,77.5,80,82.5,85,87.5,90,100,110,120},
							{10,15,20,25,30,35,40,42.5,45,47.5,50,52.5,55,57.5,60,62.5,65,67.5,70,72.5,75,77.5,80,82.5,85,87.5,90,100,110,120},
							{0,50,100,150,200,250,300,350,400,425,450,475,500,525,550,575,600,625,650,675,700,725,750,775,800,825,850,875,900,925},
							{0,50,100,150,200,250,300,350,400,425,450,475,500,525,550,575,600,625,650,675,700,725,750,775,800,825,850,875,900,925},
							{0,10,20,30,40,45,50,55,60,65,70,72,74,76,78,80,82,84,86,88,90,92,94,96,98,100,102,104,106,108},
							{0,10,20,30,40,45,50,55,60,65,70,72,74,76,78,80,82,84,86,88,90,92,94,96,98,100,102,104,106,108}},
							{{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40},
							{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40},
							{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40},
							{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29},
							{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29}},
							{{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40},
							{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40},
							{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40},
							{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29},
							{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29}}
							}
driver.displayNames = {"TotalFuel","TGT1","TGT2","Ng1","Ng2","Torque1","Torque2"}
driver.indicators = {}
function driver.processLowImportance(mainPanelDevice)
local li
	do -- constrain variable scope
		local results={}
		-- Indicator 0 for the Center panel bar graphs and numeric displays
		li = list_indication(0)
		if li ~= driver.indicators[0] then
			local currentGaugeNumber = 1
			local currentSegment = 1
			local segNextValue = 9999
			local segCursor = nil
			local segStart = 0
			local segNumberStart = 0
			local displayNum = 0

			--[[
			We need to pull out twelve bar graphs and 5 numeric displays (the easy bit).  There appears to be no delimiter
			between the bar graphs, and several of them have non-linear scales which poses an additional challenge so we use
			a table based approach to identify the specific gauge.  
			All of the temperatures start at their origins.
			This routine relies on the bar graphs all appearing in the same order!
			]]

			local m = li:gmatch("-----------------------------------------\n([^\n]+)\n([^\n]*)\n")
			
			while true do
				local name, value = m()
				if not name then
					results["gauge" .. currentGaugeNumber] = string.format("%s;%s;%s;%s", segStart, segNumberStart, segCursor, currentSegment)	
					break 
				end
				local seg = name:gsub("segment_","")
				local segNumeric

				if name == seg then
					-- non segment ie numeric display data
					if value ~= "" then
						displayNum = displayNum + 1
						results[driver.displayNames[displayNum]] = value
					end
				else
					-- process a segment entry
					segNumeric = tonumber(seg)
					if segCursor == nil then  	-- first time processing a segement
						segStart = segNumeric 	-- we must be a start entry
						segCursor = segStart	-- cursor set to the first segement
						segNextValue = segStart
					end
					if segStart == segCursor and segNextValue == segStart then
						for currSegment = 1,30,1 do
							if segNumeric == driver.UH60Lindicator[1][currentGaugeNumber][currSegment] then
								segNumberStart = currSegment
								segNextValue = driver.UH60Lindicator[1][currentGaugeNumber][currSegment+1]
								currentSegment = currSegment
								break
							end				
						end
					else
						if segNumeric == segNextValue then
							currentSegment = currentSegment + 1
							segNextValue = driver.UH60Lindicator[1][currentGaugeNumber][currentSegment+1]					
							segCursor = segNumeric
						else
							results["gauge" .. currentGaugeNumber] = string.format("%s;%s;%s;%s", segStart, segNumberStart, segCursor, currentSegment)
							currentGaugeNumber = currentGaugeNumber + 1	
							for currSegment = 1,30,1 do
								if segNumeric == driver.UH60Lindicator[1][currentGaugeNumber][currSegment] then 
									segNumberStart = currSegment
									segNextValue = driver.UH60Lindicator[1][currentGaugeNumber][currSegment+1]
									currentSegment = currSegment
									break
								end				
							end
							segStart = segNumeric
							segCursor = segStart					
						end
					end			
				end
			end

			helios.send(2060,string.format("%s", helios.ensureString(results["TotalFuel"])))
			helios.send(2061,string.format("%s", helios.ensureString(results["TGT1"])))
			helios.send(2062,string.format("%s", helios.ensureString(results["TGT2"])))
			helios.send(2063,string.format("%s", helios.ensureString(results["Ng1"])))
			helios.send(2064,string.format("%s", helios.ensureString(results["Ng2"])))
			for ii=1,12,1 do
				if results["gauge"..ii] then
					helios.send(2064+ii,string.format("%s", helios.ensureString(results["gauge"..ii]))) -- bar gauge
				end
			end
			driver.indicators[0] = li
		end
	end
	
	for jj= 1,2,1 do
		local results={}
		-- Indicator 1 and 2 for the RPM and Torque bar graphs and numeric displays for Pilot and Copilot
		li = list_indication(jj)
		if li ~= driver.indicators[jj] then

			local currentGaugeNumber = 1
			local currentSegment = 1
			local segNextValue = 9999
			local segCursor = nil
			local segStart = 0
			local segNumberStart = 0
			local displayNum = 5

			--[[
			We need to pull out five bar graphs and 2 numeric displays.  There appears to be no delimiter
			between the bar graphs so we use a table based approach to identify the specific gauge.  
			This routine relies on the bar graphs all appearing in the same order!
			]]

			local m = li:gmatch("-----------------------------------------\n([^\n]+)\n([^\n]*)\n")
			
			while true do
				local name, value = m()
				if not name then
					results["gauge" .. currentGaugeNumber] = string.format("%s;%s;%s;%s", segStart, segNumberStart, segCursor, currentSegment)	
					break 
				end
				local seg = name:gsub("segment_","")
				local segNumeric

				if name == seg then
					-- non segment ie numeric display data
					if value ~= "" then
						displayNum = displayNum + 1
						results[driver.displayNames[displayNum]] = value
					end
				else
					-- process a segment entry
					segNumeric = tonumber(seg)
					if segCursor == nil then  	-- first time processing a segement
						segStart = segNumeric 	-- we must be a start entry
						segCursor = segStart	-- cursor set to the first segement
						segNextValue = segStart
					end
					if segStart == segCursor and segNextValue == segStart then
						for currSegment = 1,30,1 do
							if segNumeric == driver.UH60Lindicator[jj+1][currentGaugeNumber][currSegment] then
								segNumberStart = currSegment
								segNextValue = driver.UH60Lindicator[jj+1][currentGaugeNumber][currSegment+1]
								currentSegment = currSegment
								break
							end				
						end
					else
						if segNumeric == segNextValue then
							currentSegment = currentSegment + 1
							segNextValue = driver.UH60Lindicator[jj+1][currentGaugeNumber][currentSegment+1]					
							segCursor = segNumeric
						else
							results["gauge" .. currentGaugeNumber] = string.format("%s;%s;%s;%s", segStart, segNumberStart, segCursor, currentSegment)
							currentGaugeNumber = currentGaugeNumber + 1	
							for currSegment = 1,30,1 do
								if segNumeric == driver.UH60Lindicator[jj+1][currentGaugeNumber][currSegment] then 
									segNumberStart = currSegment
									segNextValue = driver.UH60Lindicator[jj+1][currentGaugeNumber][currSegment+1]
									currentSegment = currSegment
									break
								end				
							end
							segStart = segNumeric
							segCursor = segStart					
						end
					end			
				end
			end
			helios.send(2077+((jj-1)*7),string.format("%s", helios.ensureString(results["Torque1"])))
			helios.send(2078+((jj-1)*7),string.format("%s", helios.ensureString(results["Torque2"])))
			for ii=1,5,1 do
				if results["gauge"..ii] then
					helios.send(2078+((jj-1)*7)+ii,string.format("%s", helios.ensureString(results["gauge"..ii]))) -- bar gauge
				end
			end
			driver.indicators[jj] = li
		end
	end
	do
		jj = 5
		local results={}
		-- Indicator 5 for the nav computer
		li = list_indication(jj)
		if li ~= driver.indicators[jj] then
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
			driver.indicators[jj] = li
		end
	end
	
	for jj = 6,7,1 do
	-- These are the two chronographs  Reserve 4 codes for each Codes run 2095 - 2103 
	end
	-- 8 are the frequency preset frequencies
	for jj = 9,10,1 do
		local results={}
		-- Indicators 9 and 10 are the AN/ARC-201 FM 1 &  2 displays
		li = list_indication(jj)
		if li ~= driver.indicators[jj] then
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
			driver.indicators[jj] = li
		end
	end	

	do
		jj = 11
		local results={}
		-- Indicator 11 for the AHRU display
		li = list_indication(jj)
		if li ~= driver.indicators[jj] then
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
			driver.indicators[jj] = li
		end
	end	
	do
		jj = 12
		local results={}
		-- Indicator 12 for the Aux Fuel display
		li = list_indication(jj)
		if li ~= driver.indicators[jj] then
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
					helios.send(2107+ii,string.format("%s", helios.ensureString(results["AuxFuel"..ii]:gsub(":","!"))))
				end
			end
			driver.indicators[jj] = li
		end
	end	
	-- next code is 2111
end