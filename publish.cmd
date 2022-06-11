@echo off
REM output location (you will need to change this for your local folder structure or mklink to where you want the files)
REM must be relative path because of submodule git log commands below
set HELIOS_SHARE_FOLDER=..\HeliosBuilds

REM arguments and validation
set HELIOS_BUILT_VERSION=%1
set HELIOS_REFERENCE_TAG=%2
git rev-parse %HELIOS_BUILT_VERSION%
if %errorlevel% neq 0 (
	echo Failed to check version git tag.  First parameter should be tag of build to publish
	exit /b %errorlevel%
)
git rev-parse %HELIOS_REFERENCE_TAG%
if %errorlevel% neq 0 (
	echo Failed to check reference git tag.  Second parameter should be tag of build relative to which we collect logs
	exit /b %errorlevel%
)

REM publish tag
git push origin %HELIOS_BUILT_VERSION%

REM publish installer files to direct share for testers and developers
mkdir %HELIOS_SHARE_FOLDER%\%HELIOS_BUILT_VERSION%
copy "Helios Installer\Release\*.msi" %HELIOS_SHARE_FOLDER%\%HELIOS_BUILT_VERSION%\ 
copy "Helios Installer\Release32\*.msi" %HELIOS_SHARE_FOLDER%\%HELIOS_BUILT_VERSION%\ 
copy "Keypress Receiver Installer\Release\*.msi" %HELIOS_SHARE_FOLDER%\%HELIOS_BUILT_VERSION%\ 
copy "Tools Installer\Release\*.msi" %HELIOS_SHARE_FOLDER%\%HELIOS_BUILT_VERSION%\ 

REM collect and format log
FOR /F %%i IN ('git rev-parse %HELIOS_BUILT_VERSION%') DO @set COMMIT=%%i
git log --date=short --decorate-refs="1.*" --format="##### [%%h](https://github.com/HeliosVirtualCockpit/Helios/commit/%%H) by %%an on %%ad %%d%%n%%w(0,4,4)%%B  %%n" %HELIOS_REFERENCE_TAG%..%HELIOS_BUILT_VERSION% > %HELIOS_SHARE_FOLDER%\%HELIOS_BUILT_VERSION%\changes_%COMMIT%.md
pushd "InterfaceFiles\Interfaces"
git log --date=short --decorate-refs="1.*" --format="##### [%%h](https://github.com/HeliosVirtualCockpit/HeliosInterfaces/commit/%%H) by %%an on %%ad %%d%%n%%w(0,4,4)%%B  %%n" %HELIOS_REFERENCE_TAG%..%HELIOS_BUILT_VERSION% >> ..\..\%HELIOS_SHARE_FOLDER%\%HELIOS_BUILT_VERSION%\changes_%COMMIT%.md
popd 

if "%3" == "nogithub" goto end

REM assemble the release assets for github (tar.exe included in Windows 10)
if exist "..\Releases\Helios\%HELIOS_BUILT_VERSION%\Assets" (
	rmdir /s /q "..\Releases\Helios\%HELIOS_BUILT_VERSION%"
)
mkdir ..\Releases\Helios\%HELIOS_BUILT_VERSION%\Assets
tar -a -c -f "..\Releases\Helios\%HELIOS_BUILT_VERSION%\Assets\Helios_Installers.zip" -C "Helios Installer\Release" *.msi -C "..\..\Keypress Receiver Installer\Release" *.msi -C "..\..\Tools Installer\Release" *.msi
tar -a -c -f "..\Releases\Helios\%HELIOS_BUILT_VERSION%\Assets\Helios32Bit_Installers.zip" -C "Helios Installer\Release32" *.msi
echo Helios %HELIOS_BUILT_VERSION% > "..\Releases\Helios\%HELIOS_BUILT_VERSION%\changes.md"
echo.>> "..\Releases\Helios\%HELIOS_BUILT_VERSION%\changes.md"
echo # Release %HELIOS_BUILT_VERSION% >> "..\Releases\Helios\%HELIOS_BUILT_VERSION%\changes.md"
echo ## User Notes >> "..\Releases\Helios\%HELIOS_BUILT_VERSION%\changes.md"
echo [EDIT REQUIRED: create user-readable notes from following Developer Notes and then update the Change Notes in Wiki from these] >> "..\Releases\Helios\%HELIOS_BUILT_VERSION%\changes.md"
echo.>> "..\Releases\Helios\%HELIOS_BUILT_VERSION%\changes.md"
echo ## Developer Notes >> "..\Releases\Helios\%HELIOS_BUILT_VERSION%\changes.md"
type %HELIOS_SHARE_FOLDER%\%HELIOS_BUILT_VERSION%\changes_%COMMIT%.md >> "..\Releases\Helios\%HELIOS_BUILT_VERSION%\changes.md"
echo.>> "..\Releases\Helios\%HELIOS_BUILT_VERSION%\changes.md"
echo Full change notes from previous releases here: https://github.com/HeliosVirtualCockpit/Helios/wiki/Change-Log >> "..\Releases\Helios\%HELIOS_BUILT_VERSION%\changes.md"

REM create draft on github (requires https://github.com/github/hub/releases/latest)
gh release create --draft %HELIOS_BUILT_VERSION% "..\Releases\Helios\%HELIOS_BUILT_VERSION%\Assets\Helios_Installers.zip#Helios Installers" "..\Releases\Helios\%HELIOS_BUILT_VERSION%\Assets\Helios32Bit_Installers.zip#Helios Installers for 32-bit Systems (untested)" -F "..\Releases\Helios\%HELIOS_BUILT_VERSION%\changes.md"

:end
REM clean up (except HELIOS_BUILT_VERSION)
set HELIOS_REFERENCE_TAG=
set HELIOS_SHARE_FOLDER=