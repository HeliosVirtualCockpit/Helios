-- Exports.Lua from Helios Mirage-2000C interface
function driver.processHighImportance(mainPanelDevice)
	-- Send Altimeter Values	
	helios.send(2051, string.format("%0.4f;%0.4f;%0.4f", mainPanelDevice:get_argument_value(306), mainPanelDevice:get_argument_value(307), mainPanelDevice:get_argument_value(305)))
	helios.send(2059, string.format("%0.2f;%0.2f;%0.2f;%0.3f", mainPanelDevice:get_argument_value(310), mainPanelDevice:get_argument_value(311), mainPanelDevice:get_argument_value(312), mainPanelDevice:get_argument_value(313)))		
	-- Send ADI Ball Values	
	helios.send(2050, string.format("%0.4f;%0.4f;%0.4f", mainPanelDevice:get_argument_value(316), mainPanelDevice:get_argument_value(318), mainPanelDevice:get_argument_value(317)))

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
		if li.PCN_UL_SEG0 then
			helios.send(2068, string.format("%6s", helios.ensureString(decode7Seg(string.format("%6s%6s%6s%6s%6s%6s%6s",li.PCN_UL_SEG2,li.PCN_UL_SEG3,li.PCN_UL_SEG4,li.PCN_UL_SEG5,li.PCN_UL_SEG0,li.PCN_UL_SEG1,li.PCN_UL_SEG6),6))))
			helios.send(2069, string.format("%7s", helios.ensureString(li.PCN_UL_SEG7:gsub("([a-z])","."))))
		end      
		if li.PCN_UR_SEG0 then
			helios.send(2070, string.format("%6s", helios.ensureString(decode7Seg(string.format("%6s%6s%6s%6s%6s%6s%6s",li.PCN_UR_SEG2,li.PCN_UR_SEG3,li.PCN_UR_SEG4,li.PCN_UR_SEG5,li.PCN_UR_SEG0,li.PCN_UR_SEG1,li.PCN_UR_SEG6),6))))
			helios.send(2071, string.format("%7s", helios.ensureString(li.PCN_UR_SEG7:gsub("([a-z])","."))))
		end

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
		if li.PCN_BL_SEG0 then
	        helios.send(2080, string.format("%s", helios.ensureString(decode7Seg(string.format("%2s%2s%2s%2s%2s%2s%2s",li.PCN_BL_SEG2,li.PCN_BL_SEG3,li.PCN_BL_SEG4,li.PCN_BL_SEG5,li.PCN_BL_SEG0,li.PCN_BL_SEG1,li.PCN_BL_SEG6),2))))
		end
		if li.PCN_BR_SEG0 then
	        helios.send(2081, string.format("%s", helios.ensureString(decode7Seg(string.format("%2s%2s%2s%2s%2s%2s%2s",li.PCN_BR_SEG2,li.PCN_BR_SEG3,li.PCN_BR_SEG4,li.PCN_BR_SEG5,li.PCN_BR_SEG0,li.PCN_BR_SEG1,li.PCN_BR_SEG6),2))))
		end
    end
    -- EVF structured data
    li = helios.parseIndication(11)
    if li then
        helios.send(2082, string.format("%-2s", helios.ensureString(li["evf-digits"])))
    end

end
function decode7Seg( segmentData, digitCount)
	local segDecode = {
	  ["****** "] = "0",
	  [" **    "] = "1",
	  ["** ** *"] = "2",
	  ["****  *"] = "3",
	  [" **  **"] = "4",
	  ["* ** **"] = "5",
	  ["* *****"] = "6",
	  ["***    "] = "7",
	  ["*******"] = "8",
	  ["**** **"] = "9",
	  ["       "] = " "
	}
	local segments = ""
	segmentData = segmentData:gsub("([a-z])","*")
	for j= 1, digitCount do
		local seg = ""
		for i= 0,digitCount*6, digitCount  do
			seg = seg .. segmentData:sub(j+i,j+i)
		end
		local decodedSeg = helios.ensureString(segDecode[seg])
		if decodedSeg == "" then
			-- Likely to be due to the fake RAZBAM display error so we'll make an attempt to guess a correction
			-- This is not fool-proof and can result in incorrect digits
			for i = 1 , 7 do
				local tempSeg = seg
				if tempSeg:sub(i,i) == " " then
					tempSeg = ("%s%s%s"):format(seg:sub(1,i-1), "*", seg:sub(i+1))
					if helios.ensureString(segDecode[tempSeg]) == "" then
							decodedSeg = "X" 
						else
							decodedSeg = segDecode[tempSeg]
							break
					end
				end
			end
		end
		segments = segments .. decodedSeg
	end
	return segments
end
