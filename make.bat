@echo off
REM Build script for LemonTree.Pipeline.Tools solution

echo Building LemonTree.Pipeline.Tools solution...
dotnet build src\LemonTree.Pipeline.Tools.sln --configuration Release

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Build completed successfully!
) else (
    echo.
    echo Build failed with error code %ERRORLEVEL%
    exit /b %ERRORLEVEL%
)
