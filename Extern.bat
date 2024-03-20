@echo off
set pw=12345

if "%1" == "dl" (
	echo y | pscp -scp -pw %pw% root@%2:/etc/majestic.yaml .
	echo y | pscp -scp -pw %pw% root@%2:/etc/wfb.conf .
	echo y | pscp -scp -pw %pw% root@%2:/etc/telemetry.conf .
)

if "%1" == "ul" (
	echo y | pscp -scp -pw %pw% majestic.yaml root@%2:/etc
	echo y | pscp -scp -pw %pw% wfb.conf root@%2:/etc
	echo y | pscp -scp -pw %pw% telemetry.conf root@%2:/etc
	plink -ssh root@%2 -pw %pw% dos2unix /etc/wfb.conf /etc/telemetry.conf /etc/majestic.yaml
)

if "%1" == "dlvrx" (
	echo y | pscp -scp -pw %pw% root@%2:/etc/vdec.conf .
	echo y | pscp -scp -pw %pw% root@%2:/etc/wfb.conf .
	echo y | pscp -scp -pw %pw% root@%2:/etc/telemetry.conf .
)

if "%1" == "ulvrx" (
	echo y | pscp -scp -pw %pw% vdec.conf root@%2:/etc
	echo y | pscp -scp -pw %pw% wfb.conf root@%2:/etc
	echo y | pscp -scp -pw %pw% telemetry.conf root@%2:/etc
	plink -ssh root@%2 -pw %pw% dos2unix /etc/wfb.conf /etc/telemetry.conf /etc/vdec.conf
)

if "%1" == "rb" (
	plink -ssh root@%2 -pw %pw% reboot
)

if "%1" == "sysup" (
	plink -ssh root@%2 -pw %pw% sysupgrade -k -r -n
)

if "%1" == "keysdl" (
	echo y | pscp -scp -pw %pw% root@%2:/root/drone.key .
)

if "%1" == "keysul" (
	echo y | pscp -scp -pw %pw% drone.key root@%2:/etc
)

if "%1" == "keysgen" (
	plink -ssh root@%2 -pw %pw% wfb_keygen
	plink -ssh root@%2 -pw %pw% cp /root/gs.key /etc/
)

if "%1" == "UART2" (
	plink -ssh root@%2 -pw %pw% sed -i '13 i devmem 0x1F207890 16 0x8' /etc/init.d/S95majestic
	plink -ssh root@%2 -pw %pw% sed -i '14 i stty -F /dev/ttyS2 115200 raw -echo -onlcr' /etc/init.d/S95majestic
)

if "%1" == "UART0" (
	plink -ssh root@%2 -pw %pw% sed -i '/devmem 0x1F207890 16 0x8/d' /etc/init.d/S95majestic
	plink -ssh root@%2 -pw %pw% sed -i '/stty -F \/dev\/ttyS2 115200 raw -echo -onlcr/d' /etc/init.d/S95majestic
)

:end
echo.
pause
