-- Lookup tables for weapons store type display
driver.gStationTypes = {["9M127-1 Vikhr-M"] = "NC", ["Kh-25ML (AS-10 Karen)"] = "NC", ["S-8KOM HEAT/Frag"] = "HP", ["S-8TsM SM Orange"] = "HP", ["S-8OFP2 MPP"] = "HP", ["S-8OM IL"] = "HP", ["S-13"] = "HP", ["UPK-23-250"] = "NN", ["BKF - 12 x AO-2.5RT"] = "A6", ["BKF - 12 x PTAB-2.5KO"] = "A6",
				 ["FAB-250"] = "A6", ["FAB-250-M62 GP"] ="A6", ["FAB-500"] = "A6" }  

-- State data
driver.gTrigger = 0

function driver.processHighImportance(mainPanelDevice)

    -- getting the UV26 data
    local li = helios.parseIndication(7)  -- 7 for UV26
	if li then
		helios.send(2005, string.format("%s",helios.ensureString(li.txt_digits)))
	end

   -- getting the EKRAN data
    local li = helios.parseIndication(4)  -- 4 for EKRAN
	if li then
        helios.send(2006, string.format("%s",helios.ensureString(li.txt_queue)))
        helios.send(2007, string.format("%s",helios.ensureString(li.txt_failure)))
		helios.send(2008, string.format("%s",helios.ensureString(li.txt_memory)))
	end

	-- getting the PVI display data
    local li = helios.parseIndication(5)  -- 75 for PVI
	if li then
		helios.send(2009, string.format("%s",helios.ensureString(li.txt_VIT)))
		helios.send(2010, string.format("%s",helios.ensureString(li.txt_VIT_apostrophe1)))
        helios.send(2011, string.format("%s",helios.ensureString(li.txt_VIT_apostrophe2)))
		helios.send(2012, string.format("%s",helios.ensureString(li.txt_OIT_PPM)))
		helios.send(2013, string.format("%s",helios.ensureString(li.txt_NIT)))
		helios.send(2014, string.format("%s",helios.ensureString(li.txt_NIT_apostrophe1)))
		helios.send(2015, string.format("%s",helios.ensureString(li.txt_NIT_apostrophe2)))
		helios.send(2016, string.format("%s",helios.ensureString(li.txt_OIT_NOT)))
	end
end

function driver.processLowImportance(mainPanelDevice)
	local lWeaponSystem = GetDevice(12)
	local lEKRAN = GetDevice(10)
	local l828Radio = GetDevice(49)
	local lCannonAmmoCount = " "
	local lStationNumbers = lWeaponSystem:get_selected_weapon_stations()
	local lStationCount = " "
	local lStationType = " "	
	local lTargetingPower = mainPanelDevice:get_argument_value(433)
	local lTrigger = mainPanelDevice:get_argument_value(615)
	if lTrigger == 0 then
		driver.gTrigger = 1
	end
	if lTrigger == -1 then
		driver.gTrigger = 0
	end

	if lTargetingPower == 1 then
		lCannonAmmoCount = string.format("%02d",string.match(lWeaponSystem:get_selected_gun_ammo_count() / 10,"(%d+)"))
	
		if #lStationNumbers ~= 0 and driver.gTrigger == 0 then
			lStationCount = 0
			for i=1,#lStationNumbers do
				lStationCount = lStationCount + lWeaponSystem:get_weapon_count_on_station(lStationNumbers[i])
			end
			
			lStationCount = string.format("%02d", lStationCount);
			
			lStationType = driver.gStationTypes[lWeaponSystem:get_weapon_type_on_station(lStationNumbers[1])]
			if lStationType == nil then
				lStationType = " "
			end
		end
	end

	local lEkranText = lEKRAN:get_actual_text_frame()
	local lEkranSendString = string.sub(lEkranText,1,8).."\n"..string.sub(lEkranText,12,19).."\n"..string.sub(lEkranText,23,30).."\n"..string.sub(lEkranText,34,41) 
	
	helios.send(2001,lStationType)
	helios.send(2002,lStationCount)
	helios.send(2003,lCannonAmmoCount)
	helios.send(2004,lEkranSendString)
	helios.send(2017, string.format("%7.3f", l828Radio:get_frequency()/1000000))
end