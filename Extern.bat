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
	echo y | pscp -scp -pw %3 root@%2:/config/scripts/screen-mode .
	echo y | pscp -scp -pw %3 root@%2:/etc/default/wifibroadcast .
)

if "%1" == "ulwfbng" (
	plink -ssh root@%2 -pw %3 apt install dos2unix
	echo y | pscp -scp -pw %3 wifibroadcast.cfg root@%2:/etc
	echo y | pscp -scp -pw %3 wfb.conf root@%2:/etc/modprobe.d/
	echo y | pscp -scp -pw %3 screen-mode root@%2:/config/scripts/
	plink -ssh root@%2 -pw %3 dos2unix /etc/wifibroadcast.cfg /etc/modprobe.d/wfb.conf /config/scripts/screen-mode
)

if "%1" == "ulwfbngr" (
	plink -ssh root@%2 -pw %3 apt install dos2unix
	echo y | pscp -scp -pw %3 wifibroadcast.cfg root@%2:/etc
	echo y | pscp -scp -pw %3 wfb.conf root@%2:/etc/modprobe.d/
	echo y | pscp -scp -pw %3 screen-mode root@%2:/config/scripts/
	plink -ssh root@%2 -pw %3 dos2unix /etc/wifibroadcast.cfg /etc/modprobe.d/wfb.conf /config/scripts/screen-mode
	plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "rb" (
	plink -ssh root@%2 -pw %3 reboot
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

if "%1" == "mspextra" (
	echo y | pscp -scp -pw %3 reset/telemetry root@%2:/usr/bin/
	plink -ssh root@%2 -pw %3 dos2unix /usr/bin/telemetry
        plink -ssh root@%2 -pw %3 reboot	
)

if "%1" == "mspgsextra" (
	echo y | pscp -scp -pw %3 reset/telemetry_gs root@%2:/usr/bin/
	plink -ssh root@%2 -pw %3 dos2unix /usr/bin/telemetry_gs
        plink -ssh root@%2 -pw %3 mv /usr/bin/telemetry_gs /usr/bin/telemetry
        plink -ssh root@%2 -pw %3 chmod +x /usr/bin/telemetry
        plink -ssh root@%2 -pw %3 reboot	
)

if "%1" == "remmspextra" (
	plink -ssh root@%2 -pw %3 sed -i 's/sleep 5/#sleep 5/' /usr/bin/telemetry
	plink -ssh root@%2 -pw %3 reboot	
)

if "%1" == "msposdair" (
	plink -ssh root@%2 -pw %3 sed -i 's/ground/air/' /config/scripts/osd
	plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "msposdgs" (
	plink -ssh root@%2 -pw %3 sed -i 's/air/ground/' /config/scripts/osd
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
	echo y | pscp -scp -pw %3 sensors/%4 root@%2:/etc/sensors/
	plink -ssh root@%2 -pw %3 yaml-cli -s .isp.sensorConfig /etc/sensors/%4
        plink -ssh root@%2 -pw %3 reboot
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

if "%1" == "offlinefw" (
	echo y | pscp -scp -pw %3 %4.tgz root@%2:/tmp
        plink -ssh root@%2 -pw %3 gzip -d /tmp/%4.tgz
        plink -ssh root@%2 -pw %3 tar -xvf /tmp/%4.tar -C /tmp
        plink -ssh root@%2 -pw %3 sysupgrade --kernel=/tmp/uImage.%5 --rootfs=/tmp/rootfs.squashfs.%5 -n
)

if "%1" == "msp" (
        echo y | pscp -scp -pw %3 vtxmenu.ini root@%2:/etc/
	plink -ssh root@%2 -pw %3 dos2unix /etc/vtxmenu.ini
        plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "40mhz" (
        echo y | pscp -scp -pw %3 reset/wifibroadcast root@%2:/usr/bin/
	plink -ssh root@%2 -pw %3 dos2unix /usr/bin/wifibroadcast
        plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "mspgs" (
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4/c\pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 "&"' /config/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --screen-mode $SCREEN_MODE/c\pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE "&"' /config/scripts/stream.sh
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
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 "&"/c\pixelpilot --osd --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 --osd-telem-lvl 2 "&"' /config/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE "&"/c\pixelpilot --osd --screen-mode $SCREEN_MODE --osd-telem-lvl 2 "&"' /config/scripts/stream.sh
        plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "mavgs2" (
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 "&"/c\pixelpilot --osd --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 --osd-telem-lvl 1 "&"' /config/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE "&"/c\pixelpilot --osd --screen-mode $SCREEN_MODE --osd-telem-lvl 1 "&"' /config/scripts/stream.sh
        plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "resetradxa" (
	plink -ssh root@%2 -pw %3 apt install dos2unix
        echo y | pscp -scp -pw %3 reset/wifibroadcast.cfg root@%2:/etc
	echo y | pscp -scp -pw %3 reset/wfb.conf root@%2:/etc/modprobe.d/
	echo y | pscp -scp -pw %3 reset/screen-mode root@%2:/home/radxa/scripts/
	echo y | pscp -scp -pw %3 reset/wifibroadcast root@%2:/etc/default/
	echo y | pscp -scp -pw %3 reset/screen-mode root@%2:/config/scripts/
	plink -ssh root@%2 -pw %3 dos2unix /etc/wifibroadcast.cfg /etc/modprobe.d/wfb.conf /etc/default/wifibroadcast /config/scripts/screen-mode
        plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "resetcam" (
	plink -ssh root@%2 -pw %3 firstboot
)

if "%1" == "addbuttons" (
	plink -ssh root@%2 -pw %3 apt install dos2unix
	echo y | pscp -scp -pw %3 stream.sh root@%2:/config/scripts/
        plink -ssh root@%2 -pw %3 dos2unix /config/scripts/stream.sh
)

:end
echo.
pause
