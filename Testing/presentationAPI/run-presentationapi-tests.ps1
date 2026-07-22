#!/usr/bin/env pwsh
param()

# Run presentationAPI tests and collect diagnostic evidence lines into TestResults
Set-StrictMode -Version Latest

$proj = Join-Path -Path $PSScriptRoot -ChildPath "presentationAPI.csproj"
$outDir = Join-Path -Path $PSScriptRoot -ChildPath "TestResults"
if (-not (Test-Path $outDir)) { New-Item -ItemType Directory -Path $outDir | Out-Null }

$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$logFile = Join-Path $outDir "run_$timestamp.log"
$trxFile = Join-Path $outDir "TestResults_$timestamp.trx"

