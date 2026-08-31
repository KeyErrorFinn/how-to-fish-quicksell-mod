@echo off
setlocal EnableExtensions EnableDelayedExpansion

REM Build a Thunderstore release from the latest source code.
REM This script lives in dist, so its parent directory is the project root.
set "DIST_DIR=%~dp0"
for %%I in ("%DIST_DIR%..") do set "PROJECT_ROOT=%%~fI"

REM The Thunderstore metadata must already be present beside this script.
for %%F in (manifest.json README.md CHANGELOG.md icon.png) do (
    if not exist "%DIST_DIR%%%F" (
        echo ERROR: Missing release file "%DIST_DIR%%%F".
        exit /b 1
    )
)

REM Read the package name and version from manifest.json so the ZIP name follows it.
for /f "tokens=2 delims=:," %%A in ('findstr /i /c:"\"name\"" "%DIST_DIR%manifest.json"') do set "PACKAGE_NAME=%%~A"
for /f "tokens=2 delims=:," %%A in ('findstr /i /c:"version_number" "%DIST_DIR%manifest.json"') do set "VERSION=%%~A"
set "PACKAGE_NAME=!PACKAGE_NAME: =!"
set "PACKAGE_NAME=!PACKAGE_NAME:"=!"
set "VERSION=!VERSION: =!"
set "VERSION=!VERSION:"=!"
if not defined PACKAGE_NAME set "PACKAGE_NAME=How_to_QuickSell"
if not defined VERSION (
    echo ERROR: Could not read version_number from manifest.json.
    exit /b 1
)

set "DLL_NAME=KeyErrorFinn.QuickSell.dll"
set "BUILD_DLL=%PROJECT_ROOT%\bin\Release\net472\%DLL_NAME%"
set "DIST_DLL=%DIST_DIR%%DLL_NAME%"
set "ZIP_PATH=%DIST_DIR%!PACKAGE_NAME!-!VERSION!.zip"

REM Compile the current project with optimizations for distribution.
dotnet build "%PROJECT_ROOT%\QuickSell.csproj" -c Release --nologo -p:DeployToProfile=false
if errorlevel 1 (
    echo ERROR: The Release build failed.
    exit /b 1
)

REM Refresh the generated DLL. Metadata stays editable in dist.
if exist "%DIST_DLL%" del /q "%DIST_DLL%"
copy /y "%BUILD_DLL%" "%DIST_DLL%" >nul
if errorlevel 1 (
    echo ERROR: Could not refresh the release DLL.
    exit /b 1
)

REM Windows includes tar.exe; ZIP only explicit package files, never this build script.
where tar >nul 2>nul
if errorlevel 1 (
    echo ERROR: Windows tar.exe was not found. Install a current Windows 10/11 build or create the ZIP with 7-Zip.
    exit /b 1
)
if exist "%ZIP_PATH%" del /q "%ZIP_PATH%"
pushd "%DIST_DIR%"
tar -a -c -f "%ZIP_PATH%" manifest.json README.md CHANGELOG.md icon.png "%DLL_NAME%"
if errorlevel 1 (
    popd
    echo ERROR: ZIP creation failed.
    exit /b 1
)

REM Verify every expected package entry is present at the ZIP root.
for %%F in (manifest.json README.md CHANGELOG.md icon.png "%DLL_NAME%") do (
    tar -tf "%ZIP_PATH%" | findstr /x /c:"%%~F" >nul
    if errorlevel 1 (
        popd
        echo ERROR: Package is missing "%%~F".
        exit /b 1
    )
)
popd

echo Refreshed release files in "%DIST_DIR%"
echo Created "%ZIP_PATH%"
exit /b 0
