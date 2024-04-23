#/bin/bash
export DISPLAY=:0
xset s off -dpms

#set your desired screen resolution here
MODE=1920x1080
RATE=60

if [[ $(xrandr | awk '/HDMI-1/ {print $2}') == "connected" ]]; then
        xrandr --output HDMI-1 --mode $MODE --rate $RATE
fi

exit 0
