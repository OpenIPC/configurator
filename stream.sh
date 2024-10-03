#!/bin/bash

SCREEN_MODE=$(</config/scripts/screen-mode)
REC_FPS=$(</config/scripts/rec-fps)


RUNNING=0
STREAMING=0

gpio_chip="gpiochip4"
gpio_offset="10"

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
    sleep 0.2
done
