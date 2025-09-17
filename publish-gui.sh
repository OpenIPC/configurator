#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
GUI_PROJECT="${ROOT_DIR}/OpenIPCConfigurator.Avalonia/OpenIPCConfigurator.Avalonia.csproj"
OUTPUT_DIR="${ROOT_DIR}/publish/gui"

echo "Building OpenIPC Configurator GUI for Linux..."

# Clean previous builds
rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

# Build self-contained GUI application
dotnet publish "$GUI_PROJECT" \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -o "$OUTPUT_DIR" \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true

echo "Published GUI to $OUTPUT_DIR"
echo ""
echo "Run with: $OUTPUT_DIR/OpenIPCConfigurator.Avalonia"
