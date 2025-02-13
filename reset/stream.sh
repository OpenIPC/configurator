#!/bin/bash

DVR_PATH=/media
SCREEN_MODE=$(grep "^mode = " /config/scripts/screen-mode | cut -d'=' -f2 | tr -d ' ')
REC_FPS=$(grep "^fps = " /config/scripts/rec-fps | cut -d'=' -f2 | tr -d ' ')
OSD=$(grep "^render = " /config/scripts/osd | cut -d'=' -f2 | tr -d ' ')
PID=0
AP_MODE=0
LONG_PRESS_DURATION=4  # Duration in seconds for long press

# Button GPIO assignments
DVR_BUTTON=`gpiofind PIN_32`
UP_BUTTON=`gpiofind PIN_16`
DOWN_BUTTON=`gpiofind PIN_18`
LEFT_BUTTON=`gpiofind PIN_13`
RIGHT_BUTTON=`gpiofind PIN_11`
MHZ_BUTTON=`gpiofind PIN_38`

# Function to start AP mode
start_ap_mode() {
    echo "Starting AP mode..." > /run/pixelpilot.msg

    # Stop any existing wireless services that might interfere
    sudo systemctl stop hostapd 2>/dev/null
    sudo systemctl stop dnsmasq 2>/dev/null
    sleep 1

    # Ensure wlan0 is in a clean state
    sudo ip link set wlan0 down
    sleep 1
    sudo ip addr flush dev wlan0
    sleep 1

    # Configure network and verify
    sudo ip addr add 192.168.4.1/24 dev wlan0
    sleep 1
    
    # Verify IP assignment
    if ! ip addr show wlan0 | grep -q "192.168.4.1/24"; then
        echo "Failed to assign IP to wlan0" > /run/pixelpilot.msg
        return 1
    fi

    # Start hostapd and verify
    sudo systemctl start hostapd
    sleep 2
    if ! systemctl is-active --quiet hostapd; then
        echo "Failed to start hostapd" > /run/pixelpilot.msg
        return 1
    fi

    # Bring up interface
    sudo ip link set wlan0 up
    sleep 1
    if [[ $(ip link show wlan0 | grep -c "UP") -eq 0 ]]; then
        echo "Failed to bring up wlan0" > /run/pixelpilot.msg
        return 1
    fi

    # Start web UI
    cd /config/webUI
    sudo python3 /config/webUI/app.py &
    sleep 2
    
    # Start DHCP server and verify
    sudo systemctl start dnsmasq
    sleep 2
    if ! systemctl is-active --quiet dnsmasq; then
        echo "Failed to start dnsmasq" > /run/pixelpilot.msg
        return 1
    fi
    
    AP_MODE=1
    echo "AP mode on." > /run/pixelpilot.msg
    echo "AP mode started. Connect to 'RadxaGroundstation' network to access files."
    return 0
}

# Function to stop AP mode and restore wifibroadcast
stop_ap_mode() {
    echo "Stopping AP mode..." > /run/pixelpilot.msg
    echo "Stopping AP mode..."
    sudo pkill python3 /config/webUI/app.py
    sudo systemctl stop hostapd
    sudo systemctl stop dnsmasq
    
    sudo ip link set wlan0 down
    sudo ip addr flush dev wlan0
    
    AP_MODE=0
    echo "AP mode off." > /run/pixelpilot.msg
    echo "Wifibroadcast mode restored."
}

# Rest of your existing variables and initialization code...
i=0

full_freq_list=("5180" "5200" "5220" "5240" "5260" "5280" "5300" "5320" "5500" "5520" "5540" "5560" "5580" "5600" "5620" "5640" "5660" "5680" "5700" "5720" "5745" "5765" "5785" "5805" "5825")
full_chan_list=("36" "40" "44" "48" "52" "56" "60" "64" "100" "104" "108" "112" "116" "120" "124" "128" "132" "136" "140" "144" "149" "153" "157" "161" "165")
wide_freq_list=("5180" "5220" "5260" "5300" "5500" "5540" "5580" "5620" "5660" "5700" "5745" "5785" "5825")
wide_chan_list=("36" "44" "52" "60" "100" "108" "116" "124" "132" "140" "149" "157" "165")
FILE="/etc/default/wifibroadcast"
WFB_CFG="/etc/wifibroadcast.cfg"

#Ensure WFB-ng is setup and NICs are available
if [[ -f "$FILE" ]]; then
    NIC_NAMES=$(grep -oP '^WFB_NICS="\K[^"]+' "$FILE")
    if [[ -n "$NIC_NAMES" ]]; then
        NICS=($NIC_NAMES)
    else
        echo "No NIC names found in WFB_NICS variable. Exiting."
        exit 1
    fi
else
    echo "File $FILE not found. Exiting."
    exit 1
fi

#Start PixelPilot
pixelpilot --osd --osd-elements 0 --osd-custom-message --osd-config /config/scripts/osd.json --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr-template $DVR_PATH/record_%Y-%m-%d_%H-%M-%S.mp4 &
PID=$!

#Start MSPOSD on gs-side
if [[ "$OSD" == "ground" ]]; then
    # Wait for IP to become available, with timeout
    max_attempts=60  # 60 attempts = 120 seconds with 2 second sleep
    attempt=1
    
    echo "Waiting for 10.5.0.1 to become available..."
    while ! ping -c 1 -W 1 10.5.0.1 >/dev/null 2>&1; do
        if [ $attempt -ge $max_attempts ]; then
            exit 1
        fi
        sleep 2
        ((attempt++))
    done
    
    echo "IP 10.5.0.1 is available, starting msposd_rockchip"
    msposd_rockchip --osd --ahi 0 --matrix 11 -v -r 5 --master 10.5.0.1:5000 &
fi

# Variables for button press timing
mhz_press_start=0

#Begin monitoring gpio for button presses
echo "Monitoring buttons"

while true; do
        DVR_BUTTON_STATE=$(gpioget $DVR_BUTTON)
        MHZ_BUTTON_STATE=$(gpioget $MHZ_BUTTON)
        UP_BUTTON_STATE=$(gpioget $UP_BUTTON)
        DOWN_BUTTON_STATE=$(gpioget $DOWN_BUTTON)
        
        # Handle MHZ button long press
        if [ "$MHZ_BUTTON_STATE" -eq 1 ]; then
            if [ "$mhz_press_start" -eq 0 ]; then
                mhz_press_start=$(date +%s)
            else
                current_time=$(date +%s)
                elapsed=$((current_time - mhz_press_start))
                
                if [ "$elapsed" -ge "$LONG_PRESS_DURATION" ]; then
                    if [ "$AP_MODE" -eq 0 ]; then
                        start_ap_mode
                    else
                        stop_ap_mode
                    fi
                    mhz_press_start=0
                    sleep 1
                fi
            fi
        fi

        # Regular button handling (only when not in AP mode)
        if [ "$AP_MODE" -eq 0 ]; then
            if [ "$DVR_BUTTON_STATE" -eq 1 ]; then
                echo "toggle DVR for $PID"
                kill -SIGUSR1 $PID
                sleep 1
            elif [ "$UP_BUTTON_STATE" -eq 1 ]; then
                # Your existing UP button handling code
                bandwidth=$(grep '^bandwidth =' $WFB_CFG | cut -d'=' -f2 | sed 's/^ //')
                if [ "$bandwidth" -eq 20 ]; then
                    i=$((i+1))
                    if [[ $i -gt 24 ]]
                    then
                        i=0
                    fi
                    Freq=${full_freq_list[$i]}
                    Chan=${full_chan_list[$i]}
                    sudo sed -i "s/wifi_channel = .*/wifi_channel = $Chan/" /etc/wifibroadcast.cfg
                    echo "$Freq" > /run/pixelpilot.msg
                    sudo systemctl restart wifibroadcast
                elif [ "$bandwidth" -eq 40 ]; then
                    i=$((i+1))
                    if [[ $i -gt 12 ]]
                    then
                        i=0
                    fi
                    Freq=${wide_freq_list[$i]}
                    Chan=${wide_chan_list[$i]}
                    sudo sed -i "s/wifi_channel = .*/wifi_channel = $Chan/" /etc/wifibroadcast.cfg
                    echo "$Freq" > /run/pixelpilot.msg
                    sudo systemctl restart wifibroadcast
                fi
            elif [ "$DOWN_BUTTON_STATE" -eq 1 ]; then
                # Your existing DOWN button handling code
                bandwidth=$(grep '^bandwidth =' $WFB_CFG | cut -d'=' -f2 | sed 's/^ //')
                if [ "$bandwidth" -eq 20 ]; then
                    i=$((i-1))
                    if [[ $i -lt 0 ]]
                    then
                        i=24
                    fi
                    Freq=${full_freq_list[$i]}
                    Chan=${full_chan_list[$i]}
                    sudo sed -i "s/wifi_channel = .*/wifi_channel = $Chan/" /etc/wifibroadcast.cfg
                    echo "$Freq" > /run/pixelpilot.msg
                    sudo systemctl restart wifibroadcast
                elif [ "$bandwidth" -eq 40 ]; then
                    i=$((i-1))
                    if [[ $i -lt 0 ]]
                    then
                        i=12
                    fi
                    Freq=${wide_freq_list[$i]}
                    Chan=${wide_chan_list[$i]}
                    sudo sed -i "s/wifi_channel = .*/wifi_channel = $Chan/" /etc/wifibroadcast.cfg
                    echo "$Freq" > /run/pixelpilot.msg
                    sudo systemctl restart wifibroadcast
                fi
            fi
        fi
        
        sleep 0.1
done
