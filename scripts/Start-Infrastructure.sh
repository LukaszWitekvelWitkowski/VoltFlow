#!/bin/bash

# 1. Setup paths
# Pobieramy folder, w ktorym znajduje sie skrypt i wychodzimy poziom wyzej
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"
ENV_PATH="$ROOT_DIR/.env"

cd "$ROOT_DIR" || exit

# Kolory dla lepszej czytelnosci
CYAN='\033[0;36m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
GREEN='\033[0;32m'
MAGENTA='\033[0;35m'
NC='\033[0m' # No Color

echo -e "${CYAN}--- VoltFlow Infrastructure Manager (Linux) ---${NC}"
echo -e "Project Root: $ROOT_DIR"

# --- CONFIGURATION ---
CONTAINER_NAME="portfolio_db"
PORT=5432

# 2. Check Docker Status
if ! docker ps >/dev/null 2>&1; then
    echo -e "${RED}ERROR: Docker is not running or no permissions! Try: sudo systemctl start docker${NC}"
    exit 1
fi

# 3. Handle Port 5432
# Sprawdzamy czy port jest zajety (wymaga lsof lub netstat)
if lsof -Pi :$PORT -sTCP:LISTEN -t >/dev/null ; then
    PID=$(lsof -Pi :$PORT -sTCP:LISTEN -t)
    echo -e "${YELLOW}Port $PORT is occupied by PID: $PID. Cleaning up...${NC}"
    kill -9 "$PID"
    sleep 1
fi

# 4. Environment Configuration (.env)
if [ ! -f "$ENV_PATH" ]; then
    echo -e "${CYAN}Creating new .env configuration...${NC}"
    echo "Select storage type: (1) Local Folder ./pgdata, (2) Docker Volume"
    read -r choice
    
    if [ "$choice" == "1" ]; then
        STORAGE_PATH="./pgdata"
    else
        STORAGE_PATH="voltflow_db_volume"
    fi

    cat <<EOF > "$ENV_PATH"
DB_ADMIN_USER=db_admin
DB_ADMIN_PASSWORD=admin_password
DB_APP_USER=portfolio_app
DB_APP_PASSWORD=app_password
DB_NAME=voltflow_db
DB_STORAGE_PATH=$STORAGE_PATH
EOF
    echo -e "${GREEN}.env file created.${NC}"
fi

# 5. Infrastructure Lifecycle
if [ "$(docker ps -a -q -f name=^/${CONTAINER_NAME}$)" ]; then
    echo -e "${YELLOW}Container exists. (R)estart or (C)lean & Start fresh? [r/c]${NC}"
    read -r action
    if [[ "$action" =~ ^[Cc]$ ]]; then
        echo -e "${MAGENTA}Performing deep clean...${NC}"
        docker compose down -v
        [ -d "./pgdata" ] && rm -rf "./pgdata"
    fi
fi

echo -e "${YELLOW}Starting Docker Compose...${NC}"
docker compose up -d

# 6. Final Health Check
echo -e "Waiting for database to be ready..."
sleep 5

STATUS=$(docker inspect --format='{{.State.Status}}' "$CONTAINER_NAME" 2>/dev/null)

if [ "$STATUS" == "running" ]; then
    echo -e "${GREEN}SUCCESS: Infrastructure is up and running!${NC}"
    echo -e "Database: PostgreSQL 16"
    echo -e "Container: $CONTAINER_NAME"
else
    echo -e "${RED}FAILURE: Container status is $STATUS${NC}"
    docker logs "$CONTAINER_NAME"
fi