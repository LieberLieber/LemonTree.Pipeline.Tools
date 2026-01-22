#!/usr/bin/env pwsh

# Test script for LemonTree.Pipeline.Tools.ModelCheck

$exePath = Join-Path $PSScriptRoot "src\LemonTree.Pipeline.Tools.ModelCheck\bin\Release\net8.0\win-x86\LemonTree.Pipeline.Tools.ModelCheck.exe"
$modelPath = Join-Path $PSScriptRoot "src\Models\Model.qeax"
$checksConfigPath = Join-Path $PSScriptRoot "src\LemonTree.Pipeline.Tools.ModelCheck\checks-config.json"
$outputPath = Join-Path $PSScriptRoot "output.md"

Write-Host "Running ModelCheck..."
Write-Host "Model: $modelPath"
Write-Host "Checks Config: $checksConfigPath"
Write-Host "Output: $outputPath"

& $exePath --model $modelPath --out $outputPath --ChecksConfig $checksConfigPath --NoCompact

$exitCode = $LASTEXITCODE
Write-Host "Exit Code: $exitCode"

exit $exitCode
