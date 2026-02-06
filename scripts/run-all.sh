#!/bin/bash
# Run all VantageView applications using the "http" launch profile.
# Use: ./scripts/run-all.sh
#
# Ports (http profile):
#   API:          5157
#   Auth:         5098
#   Frontend:     5059
#   Admin Portal: 5046

set -e
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SRC_DIR="$(cd "$SCRIPT_DIR/../src" && pwd)"

cd "$SRC_DIR"

echo "Starting VantageView applications (http profile)..."
echo ""
echo "  API:          http://localhost:5157"
echo "  Auth:         http://localhost:5098"
echo "  Frontend:     http://localhost:5059"
echo "  Admin Portal: http://localhost:5046"
echo ""
echo "Press Ctrl+C to stop all."
echo ""

# Trap Ctrl+C to kill all child processes
cleanup() {
  echo ""
  echo "Stopping all applications..."
  kill $(jobs -p) 2>/dev/null || true
  exit 0
}
trap cleanup SIGINT SIGTERM

dotnet run --project VantageView.API --launch-profile http &
dotnet run --project VantageView.Auth --launch-profile http &
dotnet run --project VantageView.Frontend --launch-profile http &
dotnet run --project VantageView.Admin.Portal --launch-profile http &

wait
