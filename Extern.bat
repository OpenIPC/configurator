@echo off

if "%1" == "dl" (
	echo y | pscp -scp -pw %3 root@%2:/etc/majestic.yaml .
	echo y | pscp -scp -pw %3 root@%2:/etc/wfb.conf .
	echo y | pscp -scp -pw %3 root@%2:/etc/telemetry.conf .
)

if "%1" == "ul" (
	echo y | pscp -scp -pw %3 majestic.yaml root@%2:/etc
	echo y | pscp -scp -pw %3 wfb.conf root@%2:/etc
	echo y | pscp -scp -pw %3 telemetry.conf root@%2:/etc
	plink -ssh root@%2 -pw %3 dos2unix /etc/wfb.conf /etc/telemetry.conf /etc/majestic.yaml
)

if "%1" == "ulr" (
	echo y | pscp -scp -pw %3 majestic.yaml root@%2:/etc
	echo y | pscp -scp -pw %3 wfb.conf root@%2:/etc
	echo y | pscp -scp -pw %3 telemetry.conf root@%2:/etc
	plink -ssh root@%2 -pw %3 dos2unix /etc/wfb.conf /etc/telemetry.conf /etc/majestic.yaml
	plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "dlvrx" (
	echo y | pscp -scp -pw %3 root@%2:/etc/vdec.conf .
	echo y | pscp -scp -pw %3 root@%2:/etc/wfb.conf .
	echo y | pscp -scp -pw %3 root@%2:/etc/telemetry.conf .
)

if "%1" == "ulvrx" (
	echo y | pscp -scp -pw %3 vdec.conf root@%2:/etc
	echo y | pscp -scp -pw %3 wfb.conf root@%2:/etc
	echo y | pscp -scp -pw %3 telemetry.conf root@%2:/etc
	plink -ssh root@%2 -pw %3 dos2unix /etc/wfb.conf /etc/telemetry.conf /etc/vdec.conf
)

if "%1" == "ulvrxr" (
	echo y | pscp -scp -pw %3 vdec.conf root@%2:/etc
	echo y | pscp -scp -pw %3 wfb.conf root@%2:/etc
	echo y | pscp -scp -pw %3 telemetry.conf root@%2:/etc
	plink -ssh root@%2 -pw %3 dos2unix /etc/wfb.conf /etc/telemetry.conf /etc/vdec.conf
	plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "dlwfbng" (
	echo y | pscp -scp -pw %3 root@%2:/etc/wifibroadcast.cfg .
	echo y | pscp -scp -pw %3 root@%2:/etc/modprobe.d/wfb.conf .
	echo y | pscp -scp -pw %3 root@%2:/home/radxa/scripts/screen-mode .
	echo y | pscp -scp -pw %3 root@%2:/config/scripts/screen-mode .
)

if "%1" == "ulwfbng" (
	echo y | pscp -scp -pw %3 wifibroadcast.cfg root@%2:/etc
	echo y | pscp -scp -pw %3 wfb.conf root@%2:/etc/modprobe.d/
	echo y | pscp -scp -pw %3 screen-mode root@%2:/home/radxa/scripts/
	plink -ssh root@%2 -pw %3 dos2unix /etc/wifibroadcast.cfg /etc/modprobe.d/wfb.conf /home/radxa/scripts/screen-mode
	echo y | pscp -scp -pw %3 screen-mode root@%2:/config/scripts/
	plink -ssh root@%2 -pw %3 dos2unix /etc/wifibroadcast.cfg /etc/modprobe.d/wfb.conf /config/scripts/screen-mode
)

if "%1" == "ulwfbngr" (
	echo y | pscp -scp -pw %3 wifibroadcast.cfg root@%2:/etc
	echo y | pscp -scp -pw %3 wfb.conf root@%2:/etc/modprobe.d/
	echo y | pscp -scp -pw %3 screen-mode root@%2:/home/radxa/scripts/
	plink -ssh root@%2 -pw %3 dos2unix /etc/wifibroadcast.cfg /etc/modprobe.d/wfb.conf /home/radxa/scripts/screen-mode
	echo y | pscp -scp -pw %3 screen-mode root@%2:/config/scripts/
	plink -ssh root@%2 -pw %3 dos2unix /etc/wifibroadcast.cfg /etc/modprobe.d/wfb.conf /config/scripts/screen-mode
	plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "rb" (
	plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "sysup" (
	plink -ssh root@%2 -pw %3 sysupgrade -k -r -n --force_ver
)

if "%1" == "keysdlgs" (
	echo y | pscp -scp -pw %3 root@%2:/root/drone.key .
)

if "%1" == "keysdlcam" (
	echo y | pscp -scp -pw %3 root@%2:/etc/drone.key .
)

if "%1" == "keysulgs" (
	echo y | pscp -scp -pw %3 drone.key root@%2:/etc
        plink -ssh root@%2 -pw %3 cp /etc/drone.key /etc/gs.key
)

if "%1" == "keysulcam" (
	echo y | pscp -scp -pw %3 drone.key root@%2:/etc
)

if "%1" == "keysgen" (
	plink -ssh root@%2 -pw %3 wfb_keygen
	plink -ssh root@%2 -pw %3 cp /root/gs.key /etc/
)

if "%1" == "UART0on" (
	plink -ssh root@%2 -pw %3 sed -i 's/console::respawn:\/sbin\/getty -L console 0 vt100/#console::respawn:\/sbin\/getty -L console 0 vt100/' /etc/inittab
	plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "UART0off" (
	plink -ssh root@%2 -pw %3 sed -i 's/#console::respawn:\/sbin\/getty -L console 0 vt100/console::respawn:\/sbin\/getty -L console 0 vt100/' /etc/inittab
	plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "extra" (
	plink -ssh root@%2 -pw %3 sed -i 's/mavfwd --channels \"$channels\" --master \"$serial\" --baudrate \"$baud\" -p 100 -t -a \"$aggregate\" \\/mavfwd --channels \"$channels\" --master \"$serial\" --baudrate \"$baud\" -a \"$aggregate\" --wait 5 --persist 50 -t \\/' /usr/bin/telemetry
	plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "rswfb" (
	plink -ssh root@%2 -pw %3 wifibroadcast stop
	plink -ssh root@%2 -pw %3 sleep 3
	plink -ssh root@%2 -pw %3 wifibroadcast start
)

if "%1" == "rsmaj" (
	plink -ssh root@%2 -pw %3 killall -1 majestic
)

if "%1" == "binup" (
	echo y | pscp -scp -pw %3 %4 root@%2:/etc/sensors/
)

if "%1" == "koup" (
	echo y | pscp -scp -pw %3 %4 root@%2:/lib/modules/4.9.84/sigmastar/
)

if "%1" == "shup" (
	echo y | pscp -scp -pw %3 *.sh root@%2:/root/
	echo y | pscp -scp -pw %3 channels.sh root@%2:/usr/bin/
	plink -ssh root@%2 -pw %3 rm /root/channels.sh
	plink -ssh root@%2 -pw %3 chmod +x /root/*.sh
	plink -ssh root@%2 -pw %3 chmod +x /usr/bin/channels.sh
)

if "%1" == "bindl" (
	echo y | mkdir backup
	echo y | pscp -scp -pw %3 root@%2:/etc/sensors/%4 ./backup/
)

if "%1" == "kodl" (
	echo y | mkdir backup
	echo y | pscp -scp -pw %3 root@%2:/lib/modules/4.9.84/sigmastar/%4 ./backup/
)

if "%1" == "shdl" (
	echo y | pscp -scp -pw %3 root@%2:/usr/bin/channels.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/816.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/1080.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/1080b.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/1264.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/3K.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/4K.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/1184p100.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/1304p80.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/1440p60.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/1920p30.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/1080p60.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/720p120.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/720p100.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/720p90.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/720p60.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/1080p120.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/1248p90.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/1304p80.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/1416p70.sh .
	echo y | pscp -scp -pw %3 root@%2:/root/kill.sh .
)

if "%1" == "temp" (
	plink -ssh root@%2 -pw %3 cat /sys/devices/virtual/mstar/msys/TEMP_R
)

if "%1" == "rubyfw" (
	plink -ssh root@%2 -pw %3 sed -i 's/BUILD_OPTION=fpv/BUILD_OPTION=rubyfpv/' /etc/os-release
	plink -ssh root@%2 -pw %3 fw_setenv upgrade https://github.com/OpenIPC/firmware/releases/download/latest/openipc.ssc338q-nor-rubyfpv.tgz
)

if "%1" == "wfbfw" (
	plink -ssh root@%2 -pw %3 sed -i 's/BUILD_OPTION=rubyfpv/BUILD_OPTION=fpv/' /etc/os-release
	plink -ssh root@%2 -pw %3 fw_setenv upgrade https://github.com/OpenIPC/firmware/releases/download/latest/ssc338q_fpv_openipc-urllc-aio-nor.tgz
)

if "%1" == "offlinefw" (
	echo y | pscp -scp -pw %3 uImage.%4 root@%2:/tmp
	echo y | pscp -scp -pw %3 rootfs.squashfs.%4 root@%2:/tmp
	plink -ssh root@%2 -pw %3 sysupgrade --kernel=/tmp/uImage.%4 --rootfs=/tmp/rootfs.squashfs.%4 -n
)

if "%1" == "msp0" (
	plink -ssh root@%2 -pw %3 sed -i '/echo \"Starting wifibroadcast service...\"/c\msposd --master /dev/ttyS0 --baudrate 115200 --channels 8 --out 127.0.0.1:14555 -osd -r 20 --ahi 0 --wait 5 --persist 50 -v "&" echo \"&L70 &F35 CPU:&C &B Temp:&T\" ">"/tmp/MSPOSD.msg "&"' /etc/init.d/S98datalink
	plink -ssh root@%2 -pw %3 sed -i '/killall -q mavfwd/c\killall -q msposd' /etc/init.d/S98datalink
        plink -ssh root@%2 -pw %3 sed -i '/telemetry=true/c\telemetry=false' /etc/datalink.conf
        plink -ssh root@%2 -pw %3 killall -q msposd
        echo y | pscp -scp -pw %3 msposd root@%2:/usr/bin/
        plink -ssh root@%2 -pw %3 chmod +x /usr/bin/msposd
        plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "msp2" (
	plink -ssh root@%2 -pw %3 sed -i '/echo \"Starting wifibroadcast service...\"/c\msposd --master /dev/ttyS2 --baudrate 115200 --channels 8 --out 127.0.0.1:14555 -osd -r 20 --ahi 0 --wait 5 --persist 50 -v "&" echo \"&L70 &F35 CPU:&C &B Temp:&T\" ">"/tmp/MSPOSD.msg "&"' /etc/init.d/S98datalink
	plink -ssh root@%2 -pw %3 sed -i '/killall -q mavfwd/c\killall -q msposd' /etc/init.d/S98datalink
        plink -ssh root@%2 -pw %3 sed -i '/telemetry=true/c\telemetry=false' /etc/datalink.conf
        plink -ssh root@%2 -pw %3 killall -q msposd
        echo y | pscp -scp -pw %3 msposd root@%2:/usr/bin/
        plink -ssh root@%2 -pw %3 chmod +x /usr/bin/msposd
        plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "mspgs" (
	plink -ssh root@%2 -pw %3 sed -i '/fpvue --osd --screen-mode $SCREEN_MODE --dvr-framerate 60 --dvr-fmp4 --dvr record_${current_date}.mp4/c\fpvue --osd --osd-elements wfbng,video --screen-mode $SCREEN_MODE --dvr-framerate 60 --dvr-fmp4 --dvr record_${current_date}.mp4 "&"' /home/radxa/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/fpvue --osd --screen-mode $SCREEN_MODE/c\fpvue --osd --osd-elements wfbng,video --screen-mode $SCREEN_MODE "&"' /home/radxa/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --screen-mode $SCREEN_MODE --dvr-framerate 60 --dvr-fmp4 --dvr record_${current_date}.mp4/c\pixelpilot --osd --osd-elements wfbng,video --screen-mode $SCREEN_MODE --dvr-framerate 60 --dvr-fmp4 --dvr record_${current_date}.mp4 "&"' /home/radxa/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --screen-mode $SCREEN_MODE/c\pixelpilot --osd --osd-elements wfbng,video --screen-mode $SCREEN_MODE "&"' /home/radxa/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4/c\pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 "&"' /config/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --screen-mode $SCREEN_MODE/c\pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE "&"' /config/scripts/stream.sh
        plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "mav" (
	plink -ssh root@%2 -pw %3 sed -i '/msposd --master/c\echo \"Starting wifibroadcast service...\"' /etc/init.d/S98datalink
	plink -ssh root@%2 -pw %3 sed -i '/killall -q msposd/c\killall -q mavfwd' /etc/init.d/S98datalink
        plink -ssh root@%2 -pw %3 sed -i '/telemetry=false/c\telemetry=true' /etc/datalink.conf
        plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "dualosd" (
        plink -ssh root@%2 -pw %3 sed -i '/telemetry=false/c\telemetry=true' /etc/datalink.conf
	plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "onboardrecon" (
        plink -ssh root@%2 -pw %3 yaml-cli -s .records.enabled true
	plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "onboardrecoff" (
        plink -ssh root@%2 -pw %3 yaml-cli -s .records.enabled false
	plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "mavgs" (
	plink -ssh root@%2 -pw %3 sed -i '/fpvue --osd --osd-elements wfbng,video --screen-mode $SCREEN_MODE --dvr-framerate 60 --dvr-fmp4 --dvr record_${current_date}.mp4 "&"/c\fpvue --osd --screen-mode $SCREEN_MODE --dvr-framerate 60 --dvr-fmp4 --dvr record_${current_date}.mp4 --osd-telem-lvl 2 "&"' /home/radxa/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/fpvue --osd --osd-elements wfbng,video --screen-mode $SCREEN_MODE "&"/c\fpvue --osd --screen-mode $SCREEN_MODE --osd-telem-lvl 2 "&"' /home/radxa/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --osd-elements wfbng,video --screen-mode $SCREEN_MODE --dvr-framerate 60 --dvr-fmp4 --dvr record_${current_date}.mp4 "&"/c\pixelpilot --osd --screen-mode $SCREEN_MODE --dvr-framerate 60 --dvr-fmp4 --dvr record_${current_date}.mp4 --osd-telem-lvl 2 "&"' /home/radxa/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --osd-elements wfbng,video --screen-mode $SCREEN_MODE "&"/c\pixelpilot --osd --screen-mode $SCREEN_MODE --osd-telem-lvl 2 "&"' /home/radxa/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 "&"/c\pixelpilot --osd --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 --osd-telem-lvl 2 "&"' /config/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE "&"/c\pixelpilot --osd --screen-mode $SCREEN_MODE --osd-telem-lvl 2 "&"' /config/scripts/stream.sh
        plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "mavgs2" (
	plink -ssh root@%2 -pw %3 sed -i '/fpvue --osd --osd-elements wfbng,video --screen-mode $SCREEN_MODE --dvr-framerate 60 --dvr-fmp4 --dvr record_${current_date}.mp4 "&"/c\fpvue --osd --screen-mode $SCREEN_MODE --dvr-framerate 60 --dvr-fmp4 --dvr record_${current_date}.mp4 --osd-telem-lvl 1 "&"' /home/radxa/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/fpvue --osd --osd-elements wfbng,video --screen-mode $SCREEN_MODE "&"/c\fpvue --osd --screen-mode $SCREEN_MODE --osd-telem-lvl 1 "&"' /home/radxa/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 "&"/c\pixelpilot --osd --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 --osd-telem-lvl 1 "&"' /config/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE "&"/c\pixelpilot --osd --screen-mode $SCREEN_MODE --osd-telem-lvl 1 "&"' /config/scripts/stream.sh
        plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "fonts" (
        echo y | pscp -scp -pw %3 bf/font.png root@%2:/usr/bin/
        echo y | pscp -scp -pw %3 bf/font_hd.png root@%2:/usr/bin/
)

if "%1" == "fontsINAV" (
        echo y | pscp -scp -pw %3 inav/font.png root@%2:/usr/bin/
        echo y | pscp -scp -pw %3 inav/font_hd.png root@%2:/usr/bin/
)

:end
echo.
pause
