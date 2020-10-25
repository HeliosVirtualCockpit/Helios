@echo off
if "%1" == "test" (
	set HELIOS_BUILT_VERSION=1.6.1000.0
) else (
	set HELIOS_BUILT_VERSION=%1

	REM make sure clean working directory

	REM step 1: untracked files
	REM result code not set correctly on windows it seems, so we have to check output
	set LSFILES=
	set gituntracked=git ls-files --exclude-standard --others
	FOR /F %%i IN ('%gituntracked%') DO @set LSFILES=%%i
	if "%LSFILES%" NEQ "" (
		git status --porcelain
		echo Untracked files in working directory.  Update your gitignore or commit files to git before building.
		exit /b 2
	)
	set LSFILES=
	set gituntracked=

	REM step 2: staged files
	git diff-index --quiet --cached HEAD --
	if ERRORLEVEL 1 (
		git status --porcelain
		echo Unfinished commit in index.  Complete commit or stash changes before building.
		exit /b 2
	)

	REM step 3: unstaged modifications 
	git diff-files --quiet
	if ERRORLEVEL 1 (
		git status --porcelain
		echo Working directory is not clean.  Stash changes before building.
		exit /b 2
	)

	REM create tag, make sure it is unique
	git tag %1
	if ERRORLEVEL 1 (
		echo Failed to place git tag.  Ensure you used a unique major.minor.build.revision number
		exit /b 2
	)
)

REM check for error exit from if scope
if ERRORLEVEL 2 (
	echo error %errorlevel%
	exit /b 1
)

REM clean up
set HELIOS_BUILD_FAILED=
del /q "Patching\Templates\Patching\Additional Simulator Viewports\*.htpl" 
del /q "Helios\Templates\Base\Simulator Viewports\*.htpl"
rmdir /s /q "Helios Installer\Release"
rmdir /s /q "Helios Installer\Release32"
rmdir /s /q "Keypress Receiver Installer\Release"
rmdir /s /q "Tools Installer\Release"
MSBuild.exe -binaryLogger:LogFile=clean.binlog -clp:WarningsOnly -warnAsMessage:MSB4078 -p:Configuration=Release;Platform=x64 -t:Clean Helios.sln
MSBuild.exe -binaryLogger:LogFile=clean32.binlog -clp:WarningsOnly -warnAsMessage:MSB4078 -p:Configuration=Release;Platform=AnyCPU32 -t:Clean Helios.sln
rmdir /s /q bin
type nul

REM build with version stamps
MSBuild.exe -binaryLogger:LogFile=prebuild.binlog -clp:WarningsOnly -warnAsMessage:MSB4078 -p:version=%HELIOS_BUILT_VERSION% -p:Configuration=Release;Platform=x64 BuildMeFirst.sln
if %errorlevel% neq 0 (
	echo build of "BuildMeFirst.sln" "Release|x64" failed.  Installers will not be built.
	exit /b 1
)
MSBuild.exe -binaryLogger:LogFile=build.binlog  -clp:WarningsOnly -warnAsMessage:MSB4078 -p:version=%HELIOS_BUILT_VERSION% -p:Configuration=NoInstallers;Platform=x64 Helios.sln
if %errorlevel% neq 0 (
	echo build of "NoInstallers|x64" failed.  Installers will not be built.
	exit /b 1
)
MSBuild.exe -binaryLogger:LogFile=build32.binlog  -clp:WarningsOnly -warnAsMessage:MSB4078 -p:version=%HELIOS_BUILT_VERSION% -p:Configuration=NoInstallers;Platform=AnyCPU32 Helios.sln
if %errorlevel% neq 0 (
	echo build of "NoInstallers|AnyCPU32" failed.  Installers will not be built.
	exit /b 1
)

REM modify installer projects to use correct version number in msi names that get baked into setup.exe
echo backing up "Helios Installer\Helios Installer.vdproj" to "Helios Installer\Helios Installer.vdproj.bak" 
move "Helios Installer\Helios Installer.vdproj" "Helios Installer\Helios Installer.vdproj.bak"
echo generating modified "Helios Installer\Helios Installer.vdproj" 
powershell -Command "(gc 'Helios Installer\Helios Installer.vdproj.bak') -replace '1\.6\.1000\.0\.msi', '%HELIOS_BUILT_VERSION%.msi' | Set-Content 'Helios Installer\Helios Installer.vdproj'"
echo backing up "Helios Installer\Helios32bit Installer.vdproj" to "Helios Installer\Helios32bit Installer.vdproj.bak" 
move "Helios Installer\Helios32bit Installer.vdproj" "Helios Installer\Helios32bit Installer.vdproj.bak"
echo generating modified "Helios Installer\Helios32bit Installer.vdproj" 
powershell -Command "(gc 'Helios Installer\Helios32bit Installer.vdproj.bak') -replace '1\.6\.1000\.0\.msi', '%HELIOS_BUILT_VERSION%.msi' | Set-Content 'Helios Installer\Helios32bit Installer.vdproj'"
echo backing up "Keypress Receiver Installer\Keypress Receiver Installer.vdproj" to "Keypress Receiver Installer\Keypress Receiver Installer.vdproj.bak" 
move "Keypress Receiver Installer\Keypress Receiver Installer.vdproj" "Keypress Receiver Installer\Keypress Receiver Installer.vdproj.bak"
echo generating modified "Keypress Receiver Installer\Keypress Receiver Installer.vdproj" 
powershell -Command "(gc 'Keypress Receiver Installer\Keypress Receiver Installer.vdproj.bak') -replace '1\.6\.1000\.0\.msi', '%HELIOS_BUILT_VERSION%.msi' | Set-Content 'Keypress Receiver Installer\Keypress Receiver Installer.vdproj'"
echo backing up "Tools Installer\Tools Installer.vdproj" to "Tools Installer\Tools Installer.vdproj.bak" 
move "Tools Installer\Tools Installer.vdproj" "Tools Installer\Tools Installer.vdproj.bak"
echo generating modified "Tools Installer\Tools Installer.vdproj" 
powershell -Command "(gc 'Tools Installer\Tools Installer.vdproj.bak') -replace '1\.6\.1000\.0\.msi', '%HELIOS_BUILT_VERSION%.msi' | Set-Content 'Tools Installer\Tools Installer.vdproj'"

REM build installers; this requires https://stackoverflow.com/questions/8648428/an-error-occurred-while-validating-hresult-8000000a/45580775#45580775
echo building 64-bit installers
devenv Helios.sln /Build "JustInstallers|x64"
if %errorlevel% neq 0 (
	echo build of "JustInstallers|x64" failed.
	REM defer exit until we restore installers
	set HELIOS_BUILD_FAILED=true
)
echo building 32-bit installers
devenv Helios.sln /Build "JustInstallers|AnyCPU32"
if %errorlevel% neq 0 (
	echo build of "JustInstallers|AnyCPU32" failed.
	REM defer exit until we restore installers
	set HELIOS_BUILD_FAILED=true
)

REM restore installer projects
echo restoring "Helios Installer\Helios Installer.vdproj" from "Helios Installer\Helios Installer.vdproj.bak" 
move "Helios Installer\Helios Installer.vdproj.bak" "Helios Installer\Helios Installer.vdproj"
echo restoring "Helios Installer\Helios32bit Installer.vdproj" from "Helios Installer\Helios32bit Installer.vdproj.bak" 
move "Helios Installer\Helios32bit Installer.vdproj.bak" "Helios Installer\Helios32bit Installer.vdproj"
echo restoring "Keypress Receiver Installer\Keypress Receiver Installer.vdproj" from "Keypress Receiver Installer\Keypress Receiver Installer.vdproj.bak" 
move "Keypress Receiver Installer\Keypress Receiver Installer.vdproj.bak" "Keypress Receiver Installer\Keypress Receiver Installer.vdproj"
echo restoring "Tools Installer\Tools Installer.vdproj" from "Tools Installer\Tools Installer.vdproj.bak" 
move "Tools Installer\Tools Installer.vdproj.bak" "Tools Installer\Tools Installer.vdproj"

if "%HELIOS_BUILD_FAILED%" == "true" (
	goto Failed
)

REM fix up setup loaders
echo renaming setup executables
move "Helios Installer\Release\setup.exe" "Helios Installer\Release\Helios.%HELIOS_BUILT_VERSION%.Setup.exe"
move "Helios Installer\Release32\setup.exe" "Helios Installer\Release32\Helios32bit.%HELIOS_BUILT_VERSION%.Setup.exe"
move "Keypress Receiver Installer\Release\setup.exe" "Keypress Receiver Installer\Release\Helios Keypress Receiver.%HELIOS_BUILT_VERSION%.Setup.exe"

REM fix up installers
pushd "Helios Installer\Release"
cscript //nologo ..\HeliosInstallAdjustments.vbs "Helios.%HELIOS_BUILT_VERSION%.msi" %HELIOS_BUILT_VERSION%
if %errorlevel% neq 0 (
	echo Installer fixup script failed to fix up 64-bit Helios installer.  Already built installers will be deleted.
	popd
	goto Failed
)
popd
pushd "Helios Installer\Release32"
cscript //nologo ..\HeliosInstallAdjustments.vbs "Helios32bit.%HELIOS_BUILT_VERSION%.msi" %HELIOS_BUILT_VERSION%
if %errorlevel% neq 0 (
	echo Installer fixup script failed to fix up 32-bit Helios installer.  Already built installers will be deleted.
	popd
	goto Failed
)
popd
pushd "Keypress Receiver Installer\Release"
cscript //nologo "..\..\Helios Installer\HeliosInstallAdjustments.vbs" "Helios Keypress Receiver.%HELIOS_BUILT_VERSION%.msi" %HELIOS_BUILT_VERSION%
if %errorlevel% neq 0 (
	echo Installer fixup script failed to fix up 64-bit Keypress Receiver installer.  Already built installers will be deleted.
	popd
	goto Failed
)
popd
pushd "Tools Installer\Release"
cscript //nologo "..\Helios Installer\HeliosInstallAdjustments.vbs" "Helios Developer Tools.%HELIOS_BUILT_VERSION%.msi" %HELIOS_BUILT_VERSION%
if %errorlevel% neq 0 (
	echo Installer fixup script failed to fix up 64-bit Developer Tools installer.  Already built installers will be deleted.
	popd
	goto Failed
)
popd

exit /b 0

:Failed
echo renaming installers from failed build to .failed
pushd "Helios Installer\Release"
ren *.msi *.msi.failed
popd
pushd "Helios Installer\Release32"
ren *.msi *.msi.failed
popd
pushd "Keypress Receiver Installer\Release"
ren *.msi *.msi.failed
popd
pushd "Tools Installer\Release"
ren *.msi *.msi.failed
popd
exit /b 3
