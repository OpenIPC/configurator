#!/bin/bash

SCREEN_MODE=$(</config/scripts/screen-mode)
REC_FPS=$(</config/scripts/rec-fps)

RUNNING=0
STREAMING=0
i=0
j=0
gpio_chip="gpiochip4"
gpio_offset="10"
gpio_chip2="gpiochip3"
gpio_offset2="1"
gpio_offset3="2"
gpio_offset4="9"
gpio_offset5="10"
freq_list=("5180" "5200" "5220" "5240" "5260" "5280" "5300" "5320" "5500" "5520" "5540" "5560" "5580" "5600" "5620" "5640" "5660" "5680" "5700" "5720" "5745" "5765" "5785" "5805" "5825")
chan_list=("36" "40" "44" "48" "52" "56" "60" "64" "100" "104" "108" "112" "116" "120" "124" "128" "132" "136" "140" "144" "149" "153" "157" "161" "165")
FILE="/etc/default/wifibroadcast"
WFB_CFG="/etc/wifibroadcast.cfg"

if [[ -f "$FILE" ]]; then
    NIC_NAMES=$(grep -oP '^WFB_NICS="\K[^"]+' "$FILE")
    if [[ -n "$NIC_NAMES" ]]; then
        IFS=' ' read -r -a NICS <<< "$NIC_NAMES"
    else
        echo "No NIC names found in WFB_NICS variable. Exiting."
        exit 1
    fi
else
    echo "File $FILE not found. Exiting."
    exit 1
fi

cd /media

while true; do
    if [ $(gpioget $gpio_chip $gpio_offset) -eq 0 ]; then

        if [ $RUNNING -eq 0 ]; then
            kill -15 $STREAMING
            sleep 0.1
            current_date=$(date +'%m-%d-%Y_%H-%M-%S')

                pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 &

                RUNNING=$!
        else
                kill -15 $RUNNING
                RUNNING=0
                STREAMING=0
        fi
        sleep 0.1
    elif [ $STREAMING -eq 0 ]; then

        pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE &

        STREAMING=$!

    fi
    if [ $(gpioget $gpio_chip2 $gpio_offset4) -eq 1 ]; then
        Freq=${freq_list[$i]}
        Chan=${chan_list[$i]}
		for NIC in "${NICS[@]}"; do
            sudo iw "$NIC" set freq $Freq
        done
        sed -i "s/wifi_channel = .*/wifi_channel = $Chan/" /etc/wifibroadcast.cfg
        echo "$Freq"
        i=$((i+1))
        if [[ $i -gt 24 ]]
        then
           i=0
        fi
    fi
    if [ $(gpioget $gpio_chip2 $gpio_offset5) -eq 1 ]; then
        i=$((i-1))
        if [[ $i -lt 0 ]]
        then
           i=24
        fi
        Freq=${freq_list[$i]}
        Chan=${chan_list[$i]}
		for NIC in "${NICS[@]}"; do
            sudo iw "$NIC" set freq $Freq
        done
        sed -i "s/wifi_channel = .*/wifi_channel = $Chan/" /etc/wifibroadcast.cfg
        echo "$Freq"
    fi
    if [ $(gpioget $gpio_chip2 $gpio_offset2) -eq 1 ]; then
        if [[ -f "$WFB_CFG" ]]; then
            CHANNEL=$(grep 'wifi_channel = ' $WFB_CFG | sed 's/^.*= //')
        else
            echo "File $WFB_CFG not found. Exiting."
            exit 1
        fi

        for NIC in "${NICS[@]}"; do
            sudo iw dev "$NIC" set channel "$CHANNEL" HT40+
        done
    fi
    sleep 0.2
done