#!/usr/bin/env pwsh

# Sample script: Run ModelCheck on the jama-devops model and report suspected traceability links

$exePath        = Join-Path $PSScriptRoot "src\LemonTree.Pipeline.Tools.ModelCheck\bin\Release\net8.0\win-x64\LemonTree.Pipeline.Tools.ModelCheck.exe"
$modelPath      = Join-Path $PSScriptRoot "src\Models\copy_lemontree-jama-devops.qeax"
$checksConfig   = Join-Path $PSScriptRoot "src\LemonTree.Pipeline.Tools.ModelCheck\checks-config.json"
$outputMd       = Join-Path $PSScriptRoot "output.md"
$outputDetails  = Join-Path $PSScriptRoot "details.json"

Write-Host "Running ModelCheck..."
Write-Host "  Model  : $modelPath"
Write-Host "  Config : $checksConfig"
Write-Host ""

& $exePath ModelCheck `
    --model $modelPath `
    --out $outputMd `
    --details $outputDetails `
    --ChecksConfig $checksConfig `
    --NoCompact

$exitCode = $LASTEXITCODE
Write-Host ""
Write-Host "Exit code: $exitCode"

# Read the JSON details and report suspected traceability link elements
if (Test-Path $outputDetails) {
    $details = Get-Content $outputDetails -Raw | ConvertFrom-Json

    $suspectedCheck = $details.checks | Where-Object { $_.id -eq "Suspected Tracebility Links" }

    if ($null -eq $suspectedCheck) {
        Write-Host "Check 'Suspected Tracebility Links' not found in details.json."
    } elseif ($suspectedCheck.level -eq "Passed") {
        Write-Host "✅ No suspected traceability links found."
    } else {
        $elements = $suspectedCheck.affectedElements
        if ($elements.Count -eq 0) {
            Write-Host "⚠️  Check failed but no affected elements were returned."
        } else {
            Write-Host "❌ Suspected traceability links found — affected elements:"
            Write-Host ""
            $elements | Format-Table -AutoSize
        }
    }
} else {
    Write-Warning "details.json was not produced."
}

exit $exitCode
