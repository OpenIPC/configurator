@echo off

if "%1" == "preset" (
	plink -ssh root@%2 -pw %3 cli -s .video0.fps 60
	plink -ssh root@%2 -pw %3 cli -s .isp.exposure 16
	plink -ssh root@%2 -pw %3 cli -s .video0.size 1920x1080
	plink -ssh root@%2 -pw %3 cli -s .video0.bitrate 8192
	plink -ssh root@%2 -pw %3 wifibroadcast cli -s .wireless.txpower 30 
	plink -ssh root@%2 -pw %3 wifibroadcast cli -s .wireless.width 40
	plink -ssh root@%2 -pw %3 wifibroadcast cli -s .wireless.mlink 3994
	plink -ssh root@%2 -pw %3 wifibroadcast cli -s .broadcast.mcs_index 1
	plink -ssh root@%2 -pw %3 wifibroadcast cli -s .broadcast.fec_k 12
	plink -ssh root@%2 -pw %3 wifibroadcast cli -s .broadcast.fec_n 15
	plink -ssh root@%2 -pw %3 wifibroadcast cli -s .broadcast.stbc 1
	plink -ssh root@%2 -pw %3 wifibroadcast cli -s .broadcast.ldpc 1
	plink -ssh root@%2 -pw %3 wifibroadcast cli -s .telemetry.router msposd
	plink -ssh root@%2 -pw %3 wifibroadcast cli -s .telemetry.serial ttyS0
	plink -ssh root@%2 -pw %3 sed -i 's/console::respawn:\/sbin\/getty -L console 0 vt100/#console::respawn:\/sbin\/getty -L console 0 vt100/' /etc/inittab
	plink -ssh root@%2 -pw %3 cli -s .isp.sensorConfig /etc/sensors/imx415_milos15.bin
	plink -ssh root@%2 -pw %3 cli -s .fpv.enabled true
	plink -ssh root@%2 -pw %3 cli -s .fpv.noiseLevel 0
	plink -ssh root@%2 -pw %3 cli -s .video0.codec h265
	echo y | pscp -scp -pw %3 sensors/imx415_milos15.bin root@%2:/etc/sensors/
	echo y | pscp -scp -pw %3 vtxmenu.ini root@%2:/etc/
	echo y | pscp -scp -pw %3 reset/wifibroadcast root@%2:/usr/bin/
	plink -ssh root@%2 -pw %3 dos2unix /etc/vtxmenu.ini
	plink -ssh root@%2 -pw %3 dos2unix /usr/bin/wifibroadcast
	plink -ssh root@%2 -pw %3 reboot
)

:end
echo.
pause
