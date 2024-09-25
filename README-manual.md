# Installation Guide for msposd

This guide provides step-by-step instructions to install and configure msposd on your device to display telemetry data.

## Prerequisites

- Access to the device's file system and terminal.
- The following files from the root of this repository:
  - msposd
  - font.png
  - font_hd.png

## Installation Steps

### 1. Copy Files to the Device

Copy the following files from the root of this repository to the `/usr/bin/` directory on your device:

- msposd
- font.png
- font_hd.png

### 2. Configure the Device

Execute the following commands on your device:

#### Update the S98datalink Service

    sed -i '/echo "Starting wifibroadcast service..."/c\msposd --master /dev/ttyS2 --baudrate 115200 --channels 8 --out 127.0.0.1:14555 -osd -r 20 --ahi 0 --wait 5 --persist 50 -v &' /etc/init.d/S98datalink
    sed -i '/killall -q mavfwd/c\killall -q msposd' /etc/init.d/S98datalink

#### Disable Default Telemetry

    sed -i '/telemetry=true/c\telemetry=false' /etc/datalink.conf

#### Terminate Any Running Instances of msposd

    killall -q msposd

#### Set Execution Permissions

    chmod +x /usr/bin/msposd

### 3. Reboot the Device

Reboot your device to apply the changes:

    reboot

## Post-Installation

After the device reboots, you should see telemetry data displayed on the screen.

## Troubleshooting and Useful Links

If you encounter any issues, refer to the following resources for debugging:

- OpenIPC FAQ: https://github.com/OpenIPC/wiki/blob/88146b4788786bc291ce63e5d2eba85f6c36ec45/en/faq.md
- Diagnosing msposd: https://github.com/OpenIPC/msposd#diagnosing
- Example ipctool Usage: https://github.com/OpenIPC/wiki/blob/88146b4788786bc291ce63e5d2eba85f6c36ec45/en/example-ipctool.md
