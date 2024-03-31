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

if "%1" == "dlwfbng" (
	echo y | pscp -scp -pw %3 root@%2:/etc/wifibroadcast.cfg .
	echo y | pscp -scp -pw %3 root@%2:/etc/modprobe.d/wfb.conf .
	echo y | pscp -scp -pw %3 root@%2:/home/radxa/scripts/setdisplay.sh .
)

if "%1" == "ulwfbng" (
	echo y | pscp -scp -pw %3 wifibroadcast.cfg root@%2:/etc
	echo y | pscp -scp -pw %3 wfb.conf root@%2:/etc/modprobe.d/
	echo y | pscp -scp -pw %3 setdisplay.sh root@%2:/home/radxa/scripts/
	plink -ssh root@%2 -pw %3 dos2unix /etc/wifibroadcast.cfg /etc/modprobe.d/wfb.conf /home/radxa/scripts/setdisplay.sh
)

if "%1" == "rb" (
	plink -ssh root@%2 -pw %3 reboot
)

if "%1" == "sysup" (
	plink -ssh root@%2 -pw %3 sysupgrade -k -r -n
)

if "%1" == "keysdl" (
	echo y | pscp -scp -pw %3 root@%2:/root/drone.key .
)

if "%1" == "keysul" (
	echo y | pscp -scp -pw %3 drone.key root@%2:/etc
)

if "%1" == "keysgen" (
	plink -ssh root@%2 -pw %3 wfb_keygen
	plink -ssh root@%2 -pw %3 cp /root/gs.key /etc/
)

if "%1" == "UART2" (
	plink -ssh root@%2 -pw %3 sed -i '13 i devmem 0x1F207890 16 0x8' /etc/init.d/S95majestic
	plink -ssh root@%2 -pw %3 sed -i '14 i stty -F /dev/ttyS2 115200 raw -echo -onlcr' /etc/init.d/S95majestic
)

if "%1" == "UART0" (
	plink -ssh root@%2 -pw %3 sed -i '/devmem 0x1F207890 16 0x8/d' /etc/init.d/S95majestic
	plink -ssh root@%2 -pw %3 sed -i '/stty -F \/dev\/ttyS2 115200 raw -echo -onlcr/d' /etc/init.d/S95majestic
)

if "%1" == "rswfb" (
	plink -ssh root@%2 -pw %3 wifibroadcast stop
	plink -ssh root@%2 -pw %3 wifibroadcast stop
	plink -ssh root@%2 -pw %3 wifibroadcast start
)


if "%1" == "rsmaj" (
	plink -ssh root@%2 -pw %3 killall majestic
	plink -ssh root@%2 -pw %3 majestic
)

:end
echo.
pause
