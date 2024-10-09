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
        iw wlx04d4c464afea set freq $Freq
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
        iw wlx04d4c464afea set freq $Freq
        sed -i "s/wifi_channel = .*/wifi_channel = $Chan/" /etc/wifibroadcast.cfg
        echo "$Freq"
    fi
    if [ $(gpioget $gpio_chip2 $gpio_offset2) -eq 1 ]; then
        for Freq in ${freq_list[@]}; do
           Chan=${chan_list[$j]}
           if [ $(gpioget $gpio_chip2 $gpio_offset3) -eq 1 ]; then
               echo "exit loop"
               break
           fi
           iw wlx04d4c464afea set freq $Freq
           sed -i "s/wifi_channel = .*/wifi_channel = $Chan/" /etc/wifibroadcast.cfg
           j=$((j+1))
           if [[ $j -gt 24 ]]
           then
              j=0
           fi
           echo "$Freq"
           sleep 3
           if [ $(gpioget $gpio_chip2 $gpio_offset3) -eq 1 ]; then
               echo "exit loop"
               break
           fi
        done
    fi
    sleep 0.2
done
