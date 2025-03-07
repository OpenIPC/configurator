@echo off

if "%1" == "preset" (
	plink -ssh root@%2 -pw %3 cli -s .video0.fps 120
	plink -ssh root@%2 -pw %3 cli -s .isp.exposure 8
	plink -ssh root@%2 -pw %3 cli -s .video0.size 1280x720
	plink -ssh root@%2 -pw %3 cli -s .video0.bitrate 15360
	plink -ssh root@%2 -pw %3 sed -i '/driver_txpower_override=/c\driver_txpower_override=35' /etc/wfb.conf
	plink -ssh root@%2 -pw %3 sed -i '/stbc=/c\stbc=1' /etc/wfb.conf
	plink -ssh root@%2 -pw %3 sed -i '/ldpc=/c\ldpc=1' /etc/wfb.conf
	plink -ssh root@%2 -pw %3 sed -i '/mcs_index=/c\mcs_index=2' /etc/wfb.conf
	plink -ssh root@%2 -pw %3 sed -i '/bandwidth=/c\bandwidth=40' /etc/wfb.conf
	plink -ssh root@%2 -pw %3 sed -i '/fec_k=/c\fec_k=8' /etc/wfb.conf
	plink -ssh root@%2 -pw %3 sed -i '/fec_n=/c\fec_n=12' /etc/wfb.conf
	plink -ssh root@%2 -pw %3 sed -i '/serial=/c\serial=/dev/ttyS0' /etc/telemetry.conf
	plink -ssh root@%2 -pw %3 sed -i '/router=/c\router=2' /etc/telemetry.conf
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
