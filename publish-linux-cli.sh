#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT="${ROOT_DIR}/OpenIPCConfigurator.Cli/OpenIPCConfigurator.Cli.csproj"
OUTPUT_DIR="${ROOT_DIR}/publish/linux"

dotnet publish "$PROJECT" \
  -c Release \
  -r linux-x64 \
  --self-contained false \
  -o "$OUTPUT_DIR"

echo "Published CLI to $OUTPUT_DIR"
