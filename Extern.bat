@echo off

if "%1" == "dl" (
	echo y | pscp -scp -pw %3 root@%2:/etc/majestic.yaml .
	echo y | pscp -scp -pw %3 root@%2:/etc/wfb.conf .
	echo y | pscp -scp -pw %3 root@%2:/etc/telemetry.conf .
)

if "%1" == "dlyaml" (
	echo y | pscp -scp -pw %3 root@%2:/etc/majestic.yaml .
	echo y | pscp -scp -pw %3 root@%2:/etc/wfb.yaml .
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
	plink -ssh root@%2 -pw %3 dos2unix /etc/wfb.conf /etc/telemetry.conf /etc/majestic.yaml "&&" reboot
)

if "%1" == "ulyamlr" (
	echo y | pscp -scp -pw %3 majestic.yaml root@%2:/etc
	echo y | pscp -scp -pw %3 wfb.yaml root@%2:/etc
	plink -ssh root@%2 -pw %3 dos2unix /etc/wfb.yaml /etc/majestic.yaml "&&" reboot
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
	plink -ssh root@%2 -pw %3 dos2unix /etc/wfb.conf /etc/telemetry.conf /etc/vdec.conf "&&" reboot
)

if "%1" == "dlwfbng" (
	echo y | pscp -scp -pw %3 root@%2:/etc/wifibroadcast.cfg .
	echo y | pscp -scp -pw %3 root@%2:/etc/modprobe.d/wfb.conf .
	echo y | pscp -scp -pw %3 root@%2:/config/scripts/screen-mode .
	echo y | pscp -scp -pw %3 root@%2:/etc/default/wifibroadcast .
)

if "%1" == "ulwfbng" (
	echo y | pscp -scp -pw %3 wifibroadcast.cfg root@%2:/etc
	echo y | pscp -scp -pw %3 wfb.conf root@%2:/etc/modprobe.d/
	echo y | pscp -scp -pw %3 screen-mode root@%2:/config/scripts/
	plink -ssh root@%2 -pw %3 dos2unix /etc/wifibroadcast.cfg /etc/modprobe.d/wfb.conf /config/scripts/screen-mode
)

if "%1" == "ulwfbngr" (
	echo y | pscp -scp -pw %3 wifibroadcast.cfg root@%2:/etc
	echo y | pscp -scp -pw %3 wfb.conf root@%2:/etc/modprobe.d/
	echo y | pscp -scp -pw %3 screen-mode root@%2:/config/scripts/
	plink -ssh root@%2 -pw %3 dos2unix /etc/wifibroadcast.cfg /etc/modprobe.d/wfb.conf /config/scripts/screen-mode "&&" reboot
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
	plink -ssh root@%2 -pw %3 wfb_keygen "&&" cp /root/gs.key /etc/
)

if "%1" == "UART0on" (
	plink -ssh root@%2 -pw %3 sed -i 's/console::respawn:\/sbin\/getty -L console 0 vt100/#console::respawn:\/sbin\/getty -L console 0 vt100/' /etc/inittab "&&" reboot
)

if "%1" == "UART0off" (
	plink -ssh root@%2 -pw %3 sed -i 's/#console::respawn:\/sbin\/getty -L console 0 vt100/console::respawn:\/sbin\/getty -L console 0 vt100/' /etc/inittab "&&" reboot
)

if "%1" == "mspextra" (
	echo y | pscp -scp -pw %3 reset/wifibroadcast root@%2:/usr/bin/
        echo y | pscp -scp -pw %3 vtxmenu.ini root@%2:/etc/
	plink -ssh root@%2 -pw %3 dos2unix /usr/bin/wifibroadcast /etc/vtxmenu.ini "&&" wifibroadcast reset "&&" reboot
)

if "%1" == "mspgsextra" (
	echo y | pscp -scp -pw %3 reset/wifibroadcast_gs root@%2:/usr/bin/
        echo y | pscp -scp -pw %3 vtxmenu.ini root@%2:/etc/
	plink -ssh root@%2 -pw %3 dos2unix /usr/bin/wifibroadcast_gs /etc/vtxmenu.ini "&&" mv /usr/bin/wifibroadcast_gs /usr/bin/wifibroadcast "&&" chmod +x /usr/bin/wifibroadcast "&&" wifibroadcast reset "&&" reboot
)

if "%1" == "remmspextra" (
	plink -ssh root@%2 -pw %3 sed -i 's/sleep 5/#sleep 5/' /usr/bin/wifibroadcast "&&" reboot
)

if "%1" == "msposdair" (
	plink -ssh root@%2 -pw %3 sed -i 's/render = ground/render = air/' /config/scripts/osd "&&" reboot
)

if "%1" == "msposdgs" (
	plink -ssh root@%2 -pw %3 sed -i 's/render = air/render = ground/' /config/scripts/osd "&&" reboot
)

if "%1" == "rswfb" (
	plink -ssh root@%2 -pw %3 wifibroadcast start
)

if "%1" == "rsmaj" (
	plink -ssh root@%2 -pw %3 killall -1 majestic
)

if "%1" == "binup" (
	echo y | pscp -scp -pw %3 sensors/%4 root@%2:/etc/sensors/
	plink -ssh root@%2 -pw %3 yaml-cli -s .isp.sensorConfig /etc/sensors/%4 "&&" reboot
)

if "%1" == "koup" (
	echo y | pscp -scp -pw %3 %4 root@%2:/lib/modules/4.9.84/sigmastar/
)

if "%1" == "shup" (
	echo y | pscp -scp -pw %3 *.sh root@%2:/root/
	echo y | pscp -scp -pw %3 channels.sh root@%2:/usr/bin/
	plink -ssh root@%2 -pw %3 rm /root/channels.sh "&&" chmod +x /root/*.sh "&&" chmod +x /usr/bin/channels.sh
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
        plink -ssh root@%2 -pw %3 gzip -d /tmp/%4.tgz "&&" tar -xvf /tmp/%4.tar -C /tmp "&&" sysupgrade --kernel=/tmp/uImage.%5 --rootfs=/tmp/rootfs.squashfs.%5 -n
)

if "%1" == "offlinefwf" (
	echo y | pscp -scp -pw %3 %4.tgz root@%2:/tmp
        plink -ssh root@%2 -pw %3 gzip -d /tmp/%4.tgz "&&" tar -xvf /tmp/%4.tar -C /tmp "&&" sysupgrade --kernel=/tmp/uImage.%5 --rootfs=/tmp/rootfs.squashfs.%5 -n -f
)

if "%1" == "mspgs" (
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4/c\pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 "&"' /config/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --screen-mode $SCREEN_MODE/c\pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE "&"' /config/scripts/stream.sh "&&" reboot
)

if "%1" == "onboardrecon" (
        plink -ssh root@%2 -pw %3 yaml-cli -s .records.enabled true "&&" reboot
)

if "%1" == "onboardrecoff" (
        plink -ssh root@%2 -pw %3 yaml-cli -s .records.enabled false "&&" reboot
)

if "%1" == "audioon" (
        plink -ssh root@%2 -pw %3 yaml-cli -s .audio.enabled true "&&" reboot
)

if "%1" == "audiooff" (
        plink -ssh root@%2 -pw %3 yaml-cli -s .audio.enabled false "&&" reboot
)

if "%1" == "mavgs" (
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 "&"/c\pixelpilot --osd --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 --osd-telem-lvl 2 "&"' /config/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE "&"/c\pixelpilot --osd --screen-mode $SCREEN_MODE --osd-telem-lvl 2 "&"' /config/scripts/stream.sh "&&" reboot
)

if "%1" == "mavgs2" (
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 "&"/c\pixelpilot --osd --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 --osd-telem-lvl 1 "&"' /config/scripts/stream.sh
	plink -ssh root@%2 -pw %3 sed -i '/pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE "&"/c\pixelpilot --osd --screen-mode $SCREEN_MODE --osd-telem-lvl 1 "&"' /config/scripts/stream.sh "&&" reboot
)

if "%1" == "resetradxa" (
        echo y | pscp -scp -pw %3 reset/wifibroadcast.cfg root@%2:/etc
	echo y | pscp -scp -pw %3 reset/wfb.conf root@%2:/etc/modprobe.d/
	echo y | pscp -scp -pw %3 reset/wifibroadcast root@%2:/etc/default/
	echo y | pscp -scp -pw %3 reset/stream.sh reset/osd reset/osd.json reset/rec-fps reset/screen-mode root@%2:/config/scripts/
	echo y | pscp -scp -pw %3 reset/alink_gs.conf root@%2:/config/
	plink -ssh root@%2 -pw %3 dos2unix /etc/wifibroadcast.cfg /etc/modprobe.d/wfb.conf /etc/default/wifibroadcast /config/scripts/screen-mode /config/scripts/osd /config/scripts/osd.json /config/scripts/stream.sh /config/scripts/rec-fps /config/alink_gs.conf "&&" reboot
)

if "%1" == "resetcam" (
	plink -ssh root@%2 -pw %3 firstboot
)

if "%1" == "airman" (
	echo y | pscp -scp -pw %3 OpenIPC-air_manager.tar root@%2:/
	plink -ssh root@%2 -pw %3 tar -xvf /OpenIPC-air_manager.tar "&&" cd /root/OpenIPC-air_manager/ "&&" chmod +x install.sh "&&" ./install.sh 10.5.0.10
)

if "%1" == "pixelpilot" (
        tar -xf artifact.zip
        plink -ssh root@%2 -pw %3 systemctl stop openipc
	echo y | pscp -scp -pw %3 pixelpilot gsmenu.sh root@%2:/usr/local/bin
	echo y | pscp -scp -pw %3 pixelpilot root@%2:/usr/local/etc/pixelpilot
	echo y | pscp -scp -pw %3 config_osd.json root@%2:/usr/local/etc/pixelpilot
	plink -ssh root@%2 -pw %3 reboot 
)

if "%1" == "alink" (
        plink -ssh root@%2 -pw %3 killall alink_drone
	echo y | pscp -scp -pw %3 alink_drone root@%2:/usr/bin/
	echo y | pscp -scp -pw %3 yaml-cli-multi root@%2:/usr/bin/
	echo y | pscp -scp -pw %3 wlan_adapters.yaml root@%2:/etc/
	echo y | pscp -scp -pw %3 alink.conf root@%2:/etc/
	echo y | pscp -scp -pw %3 txprofiles/%4.conf root@%2:/etc/
        plink -ssh root@%2 -pw %3 mv /etc/%4.conf /etc/txprofiles.conf "&&" dos2unix /etc/alink.conf /etc/wlan_adapters.yaml /etc/txprofiles.conf "&&" chmod +x /usr/bin/alink_drone "&&" chmod +x /usr/bin/yaml-cli-multi "&&" reboot
)

if "%1" == "box" (
	echo y | pscp -scp -pw %3 reset/rc.local root@%2:/etc/
        plink -ssh root@%2 -pw %3 dos2unix /etc/rc.local
        if "%4" == "1" (
	        plink -ssh root@%2 -pw %3 sed -i '/echo setprecrop/c\echo setprecrop 0 0 904 0 2436 1828 ">" \/proc\/mi_modules\/mi_vpe\/mi_vpe0' /etc/rc.local
        )
        if "%4" == "2" (
	        plink -ssh root@%2 -pw %3 sed -i '/echo setprecrop/c\echo setprecrop 0 0 904 0 2560 1920 ">" \/proc\/mi_modules\/mi_vpe\/mi_vpe0' /etc/rc.local
        )
        if "%4" == "3" (
	        plink -ssh root@%2 -pw %3 sed -i '/echo setprecrop/c\echo setprecrop 0 0 904 0 1440 1080 ">" \/proc\/mi_modules\/mi_vpe\/mi_vpe0' /etc/rc.local
        )
        if "%4" == "4" (
	        plink -ssh root@%2 -pw %3 sed -i '/echo setprecrop/c\echo setprecrop 0 0 240 0 1440 1080 ">" \/proc\/mi_modules\/mi_vpe\/mi_vpe0' /etc/rc.local
        )
        if "%4" == "5" (
	        plink -ssh root@%2 -pw %3 sed -i '/echo setprecrop/c\echo setprecrop 0 0 904 0 1440 1080 ">" \/proc\/mi_modules\/mi_vpe\/mi_vpe0' /etc/rc.local
        )
)

if "%1" == "wide" (
        plink -ssh root@%2 -pw %3 sed -i '/sleep 0.5/d' /etc/rc.local "&&" sed -i '/echo setprecrop*/d' /etc/rc.local
)

if "%1" == "bittest" (
	plink -ssh root@%2 -pw %3 yaml-cli -s .fpv.noiseLevel 0 "&&" wifibroadcast cli -s .wireless.mlink %4 "&&" wifibroadcast start
)

if "%1" == "resfix" (
	plink -ssh root@%2 -pw %3 yaml-cli -s .video0.size 1920x1080
)

:end
echo.
pause
