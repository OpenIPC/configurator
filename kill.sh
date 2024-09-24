killall -q majestic
killall -q msposd
sleep 3
majestic &
msposd  --master /dev/ttyS0 --baudrate 115200 --channels 8 --out 127.0.0.1:14555 -osd -r 20 --ahi 0 --wait 5 --persist 50 -v &
echo "&L70 &F35 CPU:&C &B Temp:&T" >/tmp/MSPOSD.msg &
