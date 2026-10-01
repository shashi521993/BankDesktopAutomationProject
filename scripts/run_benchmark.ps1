<#
PowerShell wrapper to run visual benchmark locally:
- Creates Python venv and installs requirements
- Starts docker-compose model stubs if Docker available
- Waits for model endpoints to respond
- Runs benchmark_runner.py and metrics.py
- Collects artifacts (zips results)
- Tears down docker-compose if started

Run from repository root:
  powershell -ExecutionPolicy Bypass -File .\scripts\run_benchmark.ps1
#>

param(
	[int]$MaxRetries = 5,
	[int]$RequestTimeout = 20,
	[double]$RetryBackoff = 1.5,
	[switch]$SkipDocker
)

$ErrorActionPreference = 'Stop'

function Log { param($msg) Write-Host "[" (Get-Date -Format o) "] $msg" }

$RepoRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition
Push-Location $RepoRoot

$venvPath = Join-Path $RepoRoot '.venv'
$requirements = Join-Path $RepoRoot 'visual_models\requirements.txt'
$dockerCompose = Join-Path $RepoRoot 'visual_models\docker-compose.yml'
$modelsJson = Join-Path $RepoRoot 'visual_models\models.json'
$resultsDir = Join-Path $RepoRoot 'visual\benchmark\results'
$summaryFile = Join-Path $RepoRoot 'visual\benchmark\summary.json'

$dockerStarted = $false

try {
	# 1) Setup Python venv
	if (-not (Get-Command python -ErrorAction SilentlyContinue)) {
		Log 'Python not found in PATH. Please install Python 3.10+ and re-run.'; exit 1
	}

	if (-not (Test-Path $requirements)) { Log "Requirements file not found: $requirements"; exit 1 }

	if (-not (Test-Path $venvPath)) {
		Log 'Creating Python virtual environment...'
		python -m venv $venvPath
	} else {
		Log 'Using existing virtual environment'
	}

	# Activate venv in this script
	$activate = Join-Path $venvPath 'Scripts\Activate.ps1'
	if (-not (Test-Path $activate)) { Log "Activate script missing: $activate"; exit 1 }
	Log 'Activating virtual environment...'
	. $activate

	Log 'Upgrading pip and installing requirements...'
	python -m pip install --upgrade pip
	python -m pip install -r $requirements

	# 2) Start docker-compose if available and not skipped
	if (-not $SkipDocker) {
		$dockerAvailable = (Get-Command docker -ErrorAction SilentlyContinue) -or (Get-Command docker-compose -ErrorAction SilentlyContinue)
		if ($dockerAvailable -and (Test-Path $dockerCompose)) {
			Log 'Starting docker-compose model stubs...'
			if (Get-Command docker-compose -ErrorAction SilentlyContinue) {
				docker-compose -f $dockerCompose up -d --build
			} else {
				docker compose -f $dockerCompose up -d --build
			}
			$dockerStarted = $true
		} else {
			Log 'Docker or docker-compose not available or docker-compose.yml missing; skipping container start.'
		}
	} else {
		Log 'Skipping docker-compose startup (user requested).'
	}

	# 3) Wait for model endpoints
	if (-not (Test-Path $modelsJson)) { Log "models.json not found: $modelsJson"; }
	else {
		$models = Get-Content $modelsJson | ConvertFrom-Json
		foreach ($m in $models) {
			$url = $m.url
			if (-not $url) { continue }
			Log "Waiting for $($m.name) at $url ..."
			$ok = $false
			for ($i=1; $i -le ($MaxRetries * 6); $i++) {
				try {
					$resp = Invoke-WebRequest -Uri $url -Method Get -UseBasicParsing -TimeoutSec 3 -ErrorAction Stop
					if ($resp.StatusCode -in 200,400,405) { $ok = $true; break }
				} catch {
					# ignore
				}
				Start-Sleep -Seconds 2
			}
			if (-not $ok) { Log "Warning: $url did not respond within wait window" }
			else { Log "$url is responding" }
		}
	}

	# 4) Export env overrides for benchmark runner
	$env:VM_MAX_RETRIES = $MaxRetries.ToString()
	$env:VM_REQUEST_TIMEOUT = $RequestTimeout.ToString()
	$env:VM_RETRY_BACKOFF = $RetryBackoff.ToString()

	# 5) Run benchmark and metrics
	Log 'Running benchmark_runner.py...'
	python visual_models/benchmark_runner.py

	Log 'Running metrics.py...'
	python visual_models/metrics.py

	# 6) Collect artifacts
	if (-not (Test-Path $resultsDir)) { Log "No results directory found at $resultsDir" }
	else {
		$ts = Get-Date -Format "yyyyMMdd-HHmmss"
		$artifact = Join-Path $RepoRoot "visual\benchmark\artifacts-$ts.zip"
		Log "Archiving results to $artifact"
		if (Test-Path $artifact) { Remove-Item $artifact -Force }
		Compress-Archive -Path (Join-Path $resultsDir '*') -DestinationPath $artifact -Force
		Log "Archived results to: $artifact"
		if (Test-Path $summaryFile) { Log "Summary available: $summaryFile" }
	}

	Log 'Benchmark run finished successfully.'
} catch {
	Log "ERROR: $($_.Exception.Message)"
	Write-Error $_
	exit 1
} finally {
	# 7) Tear down docker-compose if started
	if ($dockerStarted) {
		try {
			Log 'Tearing down docker-compose stubs...'
			if (Get-Command docker-compose -ErrorAction SilentlyContinue) {
				docker-compose -f $dockerCompose down
			} else {
				docker compose -f $dockerCompose down
			}
		} catch {
			Log "Warning: failed to stop docker-compose: $($_.Exception.Message)"
		}
	}
	Pop-Location
}
