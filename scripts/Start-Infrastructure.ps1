# 1. Setup paths
$scriptPath = $PSScriptRoot
$rootPath   = Split-Path -Parent $scriptPath
$envPath    = Join-Path $rootPath ".env"

Set-Location $rootPath

Write-Host "`n--- VoltFlow Infrastructure Manager ---" -ForegroundColor Cyan
Write-Host "Project Root: $rootPath" -ForegroundColor Gray

# --- CONFIGURATION ---
$CONTAINER_NAME = "portfolio_db"

# 2. Check Docker Status
try {
    & docker ps *> $null
} catch {
    Write-Host "ERROR: Docker is not running! Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

# 3. Handle Port 5432
$portCheck = Get-NetTCPConnection -LocalPort 5432 -ErrorAction SilentlyContinue
if ($portCheck) {
    $processId = $portCheck[0].OwningProcess
    Write-Host "Port 5432 is occupied. Cleaning up (PID: $processId)..." -ForegroundColor Yellow
    Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
}

# 4. Environment Configuration (.env)
Write-Host "Checking for .env file at: $envPath" -ForegroundColor Gray

if (Test-Path -Path $envPath -PathType Leaf) {
    Write-Host "Found existing .env file. Skipping creation." -ForegroundColor Green
} else {
    Write-Host ".env file NOT found. Starting configuration wizard..." -ForegroundColor Cyan

    $storageChoice = Read-Host "Select storage type: (1) Local Folder ./pgdata, (2) Docker Volume"
    $storagePath = if ($storageChoice -eq "1") { "./pgdata" } else { "voltflow_db_volume" }

@"
DB_ADMIN_USER=db_admin
DB_ADMIN_PASSWORD=admin_password
DB_APP_USER=portfolio_app
DB_APP_PASSWORD=app_password
DB_NAME=voltflow_db
DB_STORAGE_PATH=$storagePath
"@ | Out-File -FilePath $envPath -Encoding UTF8 -NoNewline

    Write-Host "Successfully created new .env file." -ForegroundColor Green
}

# 5. Infrastructure Lifecycle
$existing = docker ps -a --format '{{.Names}}' | Select-String "^${CONTAINER_NAME}$"

if ($existing) {
    $action = Read-Host "Container exists. (R)estart or (C)lean & Start fresh? [R/C]"
    if ($action -match '^[cC]$') {
        Write-Host "Performing deep clean..." -ForegroundColor Magenta
        docker compose down -v

        if (Test-Path "./pgdata") {
            Remove-Item -Recurse -Force "./pgdata"
        }

        Write-Host "Data cleared, but .env config preserved." -ForegroundColor Gray
    }
}

# 6. Start Infrastructure
Write-Host "Starting Docker Compose..." -ForegroundColor Yellow
docker compose up -d

# 7. Final Health Check
Write-Host "Waiting for database to be ready..." -ForegroundColor Gray
Start-Sleep -Seconds 5

$status = docker inspect --format='{{.State.Status}}' $CONTAINER_NAME 2>$null

if ($status -eq "running") {
    Write-Host "SUCCESS: Infrastructure is up and running!" -ForegroundColor Green
} else {
    Write-Host "FAILURE: Container status is $status" -ForegroundColor Red
    docker logs $CONTAINER_NAME
}