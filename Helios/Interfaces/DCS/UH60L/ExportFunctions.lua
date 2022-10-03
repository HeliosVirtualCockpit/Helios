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


--[[
function driver.processLowImportance(mainPanelDevice)

end
]]