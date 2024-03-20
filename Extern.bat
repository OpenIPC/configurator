@echo off
set pw=12345

if "%1" == "dl" (
	echo y | pscp -scp -pw %pw% root@%2:/etc/majestic.yaml .
	echo y | pscp -scp -pw %pw% root@%2:/etc/wfb.conf .
)

if "%1" == "ul" (
	echo y | pscp -scp -pw %pw% majestic.yaml root@%2:/etc
	echo y | pscp -scp -pw %pw% wfb.conf root@%2:/etc
	plink -ssh root@%2 -pw %pw% dos2unix /etc/wfb.conf /etc/majestic.yaml
)

if "%1" == "rb" (
	plink -ssh root@%2 -pw %pw% reboot
)

:end
echo.
pause
