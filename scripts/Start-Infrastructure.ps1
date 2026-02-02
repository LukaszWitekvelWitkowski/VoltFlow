# ========================================================
# VoltFlow Infrastructure Manager
# Automation for Docker-based PostgreSQL environments
# ========================================================

# --- FUNCTIONS ---

function Create-SqlInitFile {
    <#
    .SYNOPSIS
    Generates or overwrites SQL initialization files based on environment variables.
    #>
    param (
        [string]$Path,
        [string]$SqlContent,
        [string]$Label
    )
    
    $shouldCreate = $true

    if (Test-Path $Path) {
        Write-Host "`n[!] File $Path already exists ($Label)." -ForegroundColor Yellow
        $choice = Read-Host "Do you want to (O)verwrite it or (S)kip? [O/S]"
        if ($choice -notmatch '^[oO]$') {
            Write-Host "Skipping $Label file creation." -ForegroundColor Gray
            $shouldCreate = $false
        }
    }

    if ($shouldCreate) {
        Write-Host "Generating $Label script at: $Path" -ForegroundColor Cyan
        # Save the provided SQL content to the file using UTF8 encoding
        $SqlContent | Out-File -FilePath $Path -Encoding UTF8 -Force
        Write-Host "$Label file generated successfully." -ForegroundColor Green
    }
}

function Execute-Styled-Sql {
    <#
    .SYNOPSIS
    Executes a SQL file inside the running Docker container.
    #>
    param (
        [string]$FilePath,
        [string]$Description
    )
    
    if (-not (Test-Path $FilePath)) {
        Write-Host "ERROR: File not found: $FilePath" -ForegroundColor Red
        return
    }

    Write-Host "Executing: $Description..." -ForegroundColor Cyan
    
    # Pipe the file content into the container's psql client
    Get-Content $FilePath -Raw | docker exec -i $CONTAINER_NAME psql -v ON_ERROR_STOP=1 -U $env:DB_ADMIN_USER -d postgres
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "FAILED: $Description" -ForegroundColor Red
        exit 1
    }
    Write-Host "SUCCESS: $Description completed." -ForegroundColor Green
}

# --- 1. SETUP PATHS ---
$scriptPath = $PSScriptRoot
$rootPath   = Split-Path -Parent $scriptPath
$envPath    = Join-Path $rootPath ".env"

Set-Location $rootPath

Write-Host "`n--- VoltFlow Infrastructure Manager ---" -ForegroundColor Cyan
Write-Host "Project Root: $rootPath" -ForegroundColor Gray

# --- 2. CONFIGURATION ---
$CONTAINER_NAME  = "portfolio_db"
$SQL_SCRIPT       = Join-Path $rootPath "init_role.sql"
$SQL_SCRIPT_USER  = Join-Path $rootPath "init_user.sql"

# --- 3. DOCKER STATUS CHECK ---
try {
    & docker ps *> $null
} catch {
    Write-Host "ERROR: Docker is not running! Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

# --- 4. PORT MANAGEMENT ---
$portCheck = Get-NetTCPConnection -LocalPort 5432 -ErrorAction SilentlyContinue
if ($portCheck) {
    $processId = $portCheck[0].OwningProcess
    Write-Host "Port 5432 is occupied. Terminating process (PID: $processId)..." -ForegroundColor Yellow
    Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
}

# --- 5. ENVIRONMENT CONFIGURATION (.env) ---
if (-not (Test-Path -Path $envPath -PathType Leaf)) {
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

# Load environment variables into PowerShell session
Get-Content $envPath | ForEach-Object {
    if ($_ -match "^(.*?)=(.*)$") {
        [System.Environment]::SetEnvironmentVariable($matches[1], $matches[2].Trim())
    }
}

# --- 6. SQL FILES PREPARATION ---
$roleSql = "GRANT ALL PRIVILEGES ON DATABASE $($env:DB_NAME) TO $($env:DB_ADMIN_USER);"
Create-SqlInitFile -Path $SQL_SCRIPT -SqlContent $roleSql -Label "Role Privileges"

$userSql = "CREATE USER $($env:DB_APP_USER) WITH PASSWORD '$($env:DB_APP_PASSWORD)';"
Create-SqlInitFile -Path $SQL_SCRIPT_USER -SqlContent $userSql -Label "App User Creation"

# --- 7. INFRASTRUCTURE LIFECYCLE ---
$existing = docker ps -a --format '{{.Names}}' | Select-String "^${CONTAINER_NAME}$"

if ($existing) {
    $action = Read-Host "Container exists. (R)estart or (C)lean & Start fresh? [R/C]"
    if ($action -match '^[cC]$') {
        Write-Host "Performing deep clean..." -ForegroundColor Magenta
        docker compose down -v
        if (Test-Path "./pgdata") { Remove-Item -Recurse -Force "./pgdata" }
    }
}

# --- 8. START INFRASTRUCTURE ---
Write-Host "Starting Docker containers..." -ForegroundColor Yellow
docker compose up -d

# --- 9. HEALTH CHECK & READINESS ---
Write-Host "Waiting for Postgres engine to be fully operational..." -ForegroundColor Gray
$retryCount = 0
$isFullyReady = $false


do {
    Start-Sleep -Seconds 10
    # Check 1: Is the port open?
    $ready = docker exec $CONTAINER_NAME pg_isready -U $env:DB_ADMIN_USER -d postgres 2>$null
    
    # Check 2: Can we actually run a query? (The final proof)
    if ($ready -match "accepting connections") {
        $testQuery = docker exec $CONTAINER_NAME psql -U $env:DB_ADMIN_USER -d postgres -tAc "SELECT 1;" 2>$null
        if ($testQuery -eq "1") { $isFullyReady = $true }
    }

    $retryCount++
    if ($retryCount -gt 20) { Write-Host "Timeout waiting for Postgres engine." -ForegroundColor Red; exit 1 }
    Write-Host "." -NoNewline -ForegroundColor Gray
} until ($isFullyReady)

Write-Host "`nPostgres is fully ready." -ForegroundColor Green

# --- 10. DATABASE INITIALIZATION ---

# Added a small buffer to prevent OCI runtime errors
Start-Sleep -Seconds 20

# Ensure application database exists - checking case-insensitive
$dbExists = docker exec $CONTAINER_NAME psql -U $env:DB_ADMIN_USER -d postgres -tAc "SELECT 1 FROM pg_database WHERE datname = lower('$($env:DB_NAME)');"

if ($dbExists -ne "1") {
    Write-Host "Database '$($env:DB_NAME)' not found. Creating..." -ForegroundColor Yellow
    docker exec $CONTAINER_NAME createdb -U $env:DB_ADMIN_USER -O $env:DB_ADMIN_USER $env:DB_NAME
    if ($LASTEXITCODE -ne 0) { Write-Host "Warning: Database creation reported an issue, but we will proceed." -ForegroundColor Gray }
} else {
    Write-Host "Database '$($env:DB_NAME)' already exists. Skipping creation." -ForegroundColor Green
}

# Execute generated scripts
Execute-Styled-Sql -FilePath $SQL_SCRIPT -Description "Role Bootstrap"
Execute-Styled-Sql -FilePath $SQL_SCRIPT_USER -Description "Application User Provisioning"

Write-Host "`nSUCCESS: Infrastructure is up and running!" -ForegroundColor Green