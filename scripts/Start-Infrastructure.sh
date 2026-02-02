#!/bin/bash

# ========================================================
# VoltFlow Infrastructure Manager
# Automation for Docker-based PostgreSQL environments
# ========================================================

# --- COLORS ---
CYAN='\033[0;36m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
GREEN='\033[0;32m'
GRAY='\033[0;90m'
MAGENTA='\033[0;35m'
NC='\033[0m' # No Color

# --- FUNCTIONS ---

create_sql_init_file() {
    local path=$1
    local content=$2
    local label=$3
    local should_create=true

    if [ -f "$path" ]; then
        echo -e "\n${YELLOW}[!] File $path already exists ($label).${NC}"
        read -p "Do you want to (O)verwrite it or (S)kip? [O/S]: " choice
        if [[ ! "$choice" =~ ^[oO]$ ]]; then
            echo -e "${GRAY}Skipping $label file creation.${NC}"
            should_create=false
        fi
    fi

    if [ "$should_create" = true ]; then
        echo -e "${CYAN}Generating $label script at: $path${NC}"
        echo "$content" > "$path"
        echo -e "${GREEN}$label file generated successfully.${NC}"
    fi
}

execute_styled_sql() {
    local file_path=$1
    local description=$2

    if [ ! -f "$file_path" ]; then
        echo -e "${RED}ERROR: File not found: $file_path${NC}"
        return
    fi

    echo -e "${CYAN}Executing: $description...${NC}"
    
    # Pipe the file content into the container's psql client
    cat "$file_path" | docker exec -i "$CONTAINER_NAME" psql -v ON_ERROR_STOP=1 -U "$DB_ADMIN_USER" -d postgres
    
    if [ $? -ne 0 ]; then
        echo -e "${RED}FAILED: $description${NC}"
        exit 1
    fi
    echo -e "${GREEN}SUCCESS: $description completed.${NC}"
}

# --- 1. SETUP PATHS ---
SCRIPT_PATH="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_PATH="$(dirname "$SCRIPT_PATH")"
ENV_PATH="$ROOT_PATH/.env"

cd "$ROOT_PATH" || exit

echo -e "\n${CYAN}--- VoltFlow Infrastructure Manager ---${NC}"
echo -e "${GRAY}Project Root: $ROOT_PATH${NC}"

# --- 2. CONFIGURATION ---
CONTAINER_NAME="portfolio_db"
SQL_SCRIPT="$ROOT_PATH/init_role.sql"
SQL_SCRIPT_USER="$ROOT_PATH/init_user.sql"

# --- 3. DOCKER STATUS CHECK ---
if ! docker ps > /dev/null 2>&1; then
    echo -e "${RED}ERROR: Docker is not running! Please start Docker service.${NC}"
    exit 1
fi

# --- 4. PORT MANAGEMENT ---
# Using lsof or netstat to check for port 5432
PORT_PID=$(lsof -t -i:5432)
if [ -not -z "$PORT_PID" ]; then
    echo -e "${YELLOW}Port 5432 is occupied. Terminating process (PID: $PORT_PID)...${NC}"
    kill -9 "$PORT_PID" 2>/dev/null
fi

# --- 5. ENVIRONMENT CONFIGURATION (.env) ---
if [ ! -f "$ENV_PATH" ]; then
    echo -e "${CYAN}.env file NOT found. Starting configuration wizard...${NC}"

    echo "Select storage type: (1) Local Folder ./pgdata, (2) Docker Volume"
    read -p "Choice: " storage_choice
    
    if [ "$storage_choice" == "1" ]; then
        storage_path="./pgdata"
    else
        storage_path="voltflow_db_volume"
    fi

cat <<EOF > "$ENV_PATH"
DB_ADMIN_USER=db_admin
DB_ADMIN_PASSWORD=admin_password
DB_APP_USER=portfolio_app
DB_APP_PASSWORD=app_password
DB_NAME=voltflow_db
DB_STORAGE_PATH=$storage_path
EOF
    echo -e "${GREEN}Successfully created new .env file.${NC}"
fi

# Load environment variables
export $(grep -v '^#' "$ENV_PATH" | xargs)

# --- 6. SQL FILES PREPARATION ---
ROLE_SQL="GRANT ALL PRIVILEGES ON DATABASE $DB_NAME TO $DB_ADMIN_USER;"
create_sql_init_file "$SQL_SCRIPT" "$ROLE_SQL" "Role Privileges"

USER_SQL="CREATE USER $DB_APP_USER WITH PASSWORD '$DB_APP_PASSWORD';"
create_sql_init_file "$SQL_SCRIPT_USER" "$USER_SQL" "App User Creation"

# --- 7. INFRASTRUCTURE LIFECYCLE ---
if [ "$(docker ps -aq -f name=^/${CONTAINER_NAME}$)" ]; then
    read -p "Container exists. (R)estart or (C)lean & Start fresh? [R/C]: " action
    if [[ "$action" =~ ^[cC]$ ]]; then
        echo -e "${MAGENTA}Performing deep clean...${NC}"
        docker compose down -v
        if [ -d "./pgdata" ]; then rm -rf "./pgdata"; fi
    fi
fi

# --- 8. START INFRASTRUCTURE ---
echo -e "${YELLOW}Starting Docker containers...${NC}"
docker compose up -d

# --- 9. HEALTH CHECK & READINESS ---
echo -e "${GRAY}Waiting for Postgres engine to be fully operational...${NC}"
retry_count=0
is_fully_ready=false

until [ "$is_fully_ready" = true ]; do
    sleep 3
    # Check 1: Is pg_isready?
    READY_STATUS=$(docker exec "$CONTAINER_NAME" pg_isready -U "$DB_ADMIN_USER" -d postgres 2>/dev/null)
    
    # Check 2: Can we run a query?
    if [[ "$READY_STATUS" == *"accepting connections"* ]]; then
        TEST_QUERY=$(docker exec "$CONTAINER_NAME" psql -U "$DB_ADMIN_USER" -d postgres -tAc "SELECT 1;" 2>/dev/null)
        if [ "$TEST_QUERY" == "1" ]; then
            is_fully_ready=true
        fi
    fi

    retry_count=$((retry_count + 1))
    if [ $retry_count -gt 30 ]; then
        echo -e "${RED}Timeout waiting for Postgres engine.${NC}"
        exit 1
    fi
    echo -n "."
done

echo -e "\n${GREEN}Postgres is fully ready.${NC}"

# --- 10. DATABASE INITIALIZATION ---
sleep 2

# Check if database exists
DB_EXISTS=$(docker exec "$CONTAINER_NAME" psql -U "$DB_ADMIN_USER" -d postgres -tAc "SELECT 1 FROM pg_database WHERE datname = lower('$DB_NAME');")

if [ "$DB_EXISTS" != "1" ]; then
    echo -e "${YELLOW}Database '$DB_NAME' not found. Creating...${NC}"
    docker exec "$CONTAINER_NAME" createdb -U "$DB_ADMIN_USER" -O "$DB_ADMIN_USER" "$DB_NAME"
else
    echo -e "${GREEN}Database '$DB_NAME' already exists.${NC}"
fi

execute_styled_sql "$SQL_SCRIPT" "Role Bootstrap"
execute_styled_sql "$SQL_SCRIPT_USER" "Application User Provisioning"

echo -e "\n${GREEN}SUCCESS: Infrastructure is up and running!${NC}"