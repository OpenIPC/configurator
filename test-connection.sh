#!/usr/bin/env bash
set -euo pipefail

IP="192.168.0.234"

echo "Testing different password combinations for OpenIPC device at $IP..."

# Common OpenIPC default passwords
PASSWORDS=(
    "openipc"
    "12345"
    ""
    "root"
    "admin"
    "openipc123"
)

for password in "${PASSWORDS[@]}"; do
    echo "Trying password: '$password'"
    if timeout 5 sshpass -p "$password" ssh -o ConnectTimeout=5 -o StrictHostKeyChecking=no root@$IP "echo 'Connected with password: $password'" 2>/dev/null; then
        echo "✅ SUCCESS! Password is: '$password'"
        
        # Test if it has wfb.conf or wfb.yaml
        echo "Checking configuration format:"
        sshpass -p "$password" ssh -o StrictHostKeyChecking=no root@$IP "ls -la /etc/ | grep -E '(wfb\.conf|wfb\.yaml)'" 2>/dev/null || echo "No wfb files found"
        sshpass -p "$password" ssh -o StrictHostKeyChecking=no root@$IP "ls -la /etc/ | grep majestic" 2>/dev/null || echo "No majestic files found"
        exit 0
    else
        echo "❌ Failed"
    fi
done

echo "All password attempts failed. Try connecting manually with: ssh root@$IP"
