#!/usr/bin/env bash
# Restores, builds, and tests the .NET solution and the SQL database project,
# then installs dependencies, builds, and tests the Angular app.
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
API_SLN="$ROOT_DIR/API/Customer_Management_System_API/CustomerManagementSystem.sln"
DB_DIR="$ROOT_DIR/DB/Customer_Management_System_DB"
DB_PROJ="Customer_Management_System_DB.sqlproj"
UI_DIR="$ROOT_DIR/UI/customer_management_system"

echo "==> [1/6] Restoring .NET solution"
dotnet restore "$API_SLN"

echo "==> [2/6] Building .NET solution"
dotnet build "$API_SLN" --no-restore --configuration Debug

echo "==> [3/6] Running .NET tests"
dotnet test "$API_SLN" --no-build --configuration Debug

echo "==> [4/6] Building database project"
(
  cd "$DB_DIR"
  dotnet restore "$DB_PROJ"
  dotnet build "$DB_PROJ" --no-restore --configuration Debug
)

echo "==> [5/6] Building Angular app"
(
  cd "$UI_DIR"
  npm ci
  npm run build
)

echo "==> [6/6] Running Angular tests"
(
  cd "$UI_DIR"
  node_modules/.bin/ng test --watch=false
)

echo "==> Build complete."
