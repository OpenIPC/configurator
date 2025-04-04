#!/bin/bash

DVR_PATH=/media
SCREEN_MODE=$(grep "^mode = " /config/scripts/screen-mode | cut -d'=' -f2 | tr -d ' ')
REC_FPS=$(grep "^fps = " /config/scripts/rec-fps | cut -d'=' -f2 | tr -d ' ')
OSD=$(grep "^render = " /config/scripts/osd | cut -d'=' -f2 | tr -d ' ')
PID=0
AP_MODE=0
DVR_RECORDING=0
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

    # Check if connection already exists
    if nmcli connection show | grep -q "Hostspot"; then
        echo "Hostspot connection exists. Starting it..."
        nmcli con up Hostspot
    else
        echo "Creating new Hostspot connection..."
        nmcli con add type wifi ifname wlan0 con-name Hostspot autoconnect no ssid RadxaGroundstation
        nmcli con modify Hostspot 802-11-wireless.mode ap 802-11-wireless.band bg ipv4.method shared
        nmcli con modify Hostspot wifi-sec.key-mgmt wpa-psk
        nmcli con modify Hostspot wifi-sec.psk "radxaopenipc"
        nmcli con modify Hostspot ipv4.addresses 192.168.4.1/24
        echo "Starting Hostspot..."
        nmcli con up Hostspot
    fi

    AP_MODE=1
    echo "AP mode on. Connect to 'RadxaGroundstation'." > /run/pixelpilot.msg
    echo "AP mode started. Connect to 'RadxaGroundstation' network to access files."
    return 0
}

# Function to stop AP mode and restore wifibroadcast
stop_ap_mode() {
    echo "Stopping AP mode..." > /run/pixelpilot.msg
    echo "Stopping AP mode..."

    nmcli con down Hostspot

    AP_MODE=0
    echo "AP mode off." > /run/pixelpilot.msg
    echo "AP mode off."
}

get_latest_video() {
    find "$DVR_PATH" -name "*.mp4" -type f -printf '%T@ %p\n' | sort -n | tail -1 | cut -f2- -d" "
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


sudo systemctl is-active --quiet wifibroadcast || sudo systemctl restart wifibroadcast
sudo systemctl is-active --quiet wifibroadcast@gs || sudo systemctl restart wifibroadcast@gs
#Start PixelPilot
pixelpilot --osd --osd-elements 0 --osd-custom-message --osd-refresh 100 --osd-config /config/scripts/osd.json --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr-sequenced-files --dvr-template $DVR_PATH/record_%Y-%m-%d_%H-%M-%S.mp4 &
PID=$!

#Start MSPOSD on gs-side
if [[ "$OSD" == "ground" ]]; then
    msposd_rockchip --osd --ahi 0 --matrix 11 -v -r 5 --master 0.0.0.0:14551 &
fi

# Variables for button press timing
mhz_press_start=0

# Function to get the current index of the channel
get_current_index() {
    local channel=$1
    local list=("${!2}")
    for i in "${!list[@]}"; do
        if [[ "${list[$i]}" == "$channel" ]]; then
            echo $i
            return
        fi
    done
    echo -1
}
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
        else
            # Regular short press handling for MHZ button
            if [ "$mhz_press_start" -ne 0 ] && [ "$AP_MODE" -eq 0 ]; then
                echo "toggling 40MHz bandwidth"
                if [[ -f "$WFB_CFG" ]]; then
                    bandwidth=$(grep '^bandwidth =' $WFB_CFG | cut -d'=' -f2 | sed 's/^ //')
                else
                    echo "File $WFB_CFG not found."
                fi

                if [[ $bandwidth -eq 20 ]]; then
                    echo "setting to 40MHz" > /run/pixelpilot.msg
                    sudo sed -i "/^bandwidth =/ s/=.*/= 40/" $WFB_CFG
                    systemctl restart wifibroadcast
                elif [[ $bandwidth -eq 40 ]]; then
                    echo "setting to 20MHz" > /run/pixelpilot.msg
                    sudo sed -i "/^bandwidth =/ s/=.*/= 20/" $WFB_CFG
                    systemctl restart wifibroadcast
                fi
            fi
            mhz_press_start=0
        fi

        # Regular button handling (only when not in AP mode)
        if [ "$AP_MODE" -eq 0 ]; then

            # Get the current channel from the config file
            current_channel=$(grep '^wifi_channel =' $WFB_CFG | cut -d'=' -f2 | sed 's/^ //')
            bandwidth=$(grep '^bandwidth =' $WFB_CFG | cut -d'=' -f2 | sed 's/^ //')

            if [ "$DVR_BUTTON_STATE" -eq 1 ]; then
                if [ "$DVR_RECORDING" -eq 0 ]; then
                echo "toggle DVR for $PID"
                kill -SIGUSR1 $PID
                DVR_RECORDING=1
                sleep 1
                elif [ "$DVR_RECORDING" -eq 1 ];then
                    echo "toggle DVR for $PID"
                    kill -SIGUSR1 $PID
                    DVR_RECORDING=0
                    sleep 2
                    latest_video=$(get_latest_video)
                    if [ -n "$latest_video" ]; then
                    # Extract filename without path and extension
                    filename=$(basename "$latest_video" .mp4)
                    # Generate thumbnail
                    sudo ffmpegthumbnailer -i "$latest_video" -o "/etc/webUI/static/thumbnails/${filename}.jpg" -s 320
                    fi
                fi
            elif [ "$UP_BUTTON_STATE" -eq 1 ]; then
            if [ "$bandwidth" -eq 20 ]; then
                current_index=$(get_current_index "$current_channel" full_chan_list[@])
                i=$((current_index+1))
                if [[ $i -ge ${#full_chan_list[@]} ]]; then
                i=0
            fi
                Freq=${full_freq_list[$i]}
                Chan=${full_chan_list[$i]}
                sudo sed -i "s/wifi_channel = .*/wifi_channel = $Chan/" /etc/wifibroadcast.cfg
                echo "Channel: $Chan - $Freq" > /run/pixelpilot.msg
                systemctl restart wifibroadcast
            elif [ "$bandwidth" -eq 40 ]; then
                current_index=$(get_current_index "$current_channel" wide_chan_list[@])
                i=$((current_index+1))
                if [[ $i -ge ${#wide_chan_list[@]} ]]; then
                    i=0
                fi
                Freq=${wide_freq_list[$i]}
                Chan=${wide_chan_list[$i]}
                sudo sed -i "s/wifi_channel = .*/wifi_channel = $Chan/" /etc/wifibroadcast.cfg
            echo "Channel: $Chan - $Freq" > /run/pixelpilot.msg
                systemctl restart wifibroadcast
            fi
            elif [ "$DOWN_BUTTON_STATE" -eq 1 ]; then
            if [ "$bandwidth" -eq 20 ]; then
                current_index=$(get_current_index "$current_channel" full_chan_list[@])
                i=$((current_index-1))
                if [[ $i -lt 0 ]]; then
                i=$((${#full_chan_list[@]}-1))
            fi
                Freq=${full_freq_list[$i]}
                Chan=${full_chan_list[$i]}
                sudo sed -i "s/wifi_channel = .*/wifi_channel = $Chan/" /etc/wifibroadcast.cfg
                echo "Channel: $Chan - $Freq" > /run/pixelpilot.msg
                systemctl restart wifibroadcast
            elif [ "$bandwidth" -eq 40 ]; then
                current_index=$(get_current_index "$current_channel" wide_chan_list[@])
                i=$((current_index-1))
                if [[ $i -lt 0 ]]; then
                i=$((${#wide_chan_list[@]}-1))
                fi
                Freq=${wide_freq_list[$i]}
                Chan=${wide_chan_list[$i]}
                sudo sed -i "s/wifi_channel = .*/wifi_channel = $Chan/" /etc/wifibroadcast.cfg
                echo "Channel: $Chan - $Freq" > /run/pixelpilot.msg
                systemctl restart wifibroadcast
            fi
        fi
    fi
sleep 0.1
done