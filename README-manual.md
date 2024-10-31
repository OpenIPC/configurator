# Installation Guide for msposd

This guide provides step-by-step instructions to install and configure msposd on your device to display telemetry data.

## Prerequisites

- Access to the device's file system and terminal.
- The following file from the root of this repository:
  - msposd
  - font.png
  - font_hd.png
  - vtxmenu.ini

## Installation Steps

### 1. Copy Files to the Device

Copy the following files from the root of this repository to the `/usr/bin/` directory on your device:

- msposd

Copy the following files from the root of this repository to the `/etc/` directory on your device:

- vtxmenu.ini

Copy the following files from the BF or INAV folders of this repository to the `/usr/share/fonts/` directory on your device:
- font.png
- font_hd.png

### 2. Configure the Device

Execute the following commands on your device:


#### Change the router type

    vi /etc/telemetry.conf

In the file change the router from whatever it is to router=2

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
