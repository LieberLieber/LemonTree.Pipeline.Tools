@echo off
REM Build script for LemonTree.Pipeline.Tools solution

echo Building LemonTree.Pipeline.Tools solution...
dotnet build src\LemonTree.Pipeline.Tools.sln --configuration Release
REM dotnet publish src\LemonTree.Pipeline.Tools.ModelCheck\LemonTree.Pipeline.Tools.ModelCheck.csproj --configuration Release --runtime win-x86 --self-contained

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Build completed successfully!
) else (
    echo.
    echo Build failed with error code %ERRORLEVEL%
    exit /b %ERRORLEVEL%
)
