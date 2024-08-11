#!/bin/sh

	# 6 way switch
	# Decide MCS, FEC, bit-rate and power based on the range [1000,1500] from TX controller
	# IMPORTANT: after copying the binaries wfb_tx and wfb_tx_cmd to /usr/bin, make them executable (chmod +x wfb_tx*) and 
	# add -C 8000 to wfb_tx command in /usr/bin/wifibroadcast under section [wfb_tx_drone]

if [ -e /etc/txprofile ]; then
	. /etc/txprofile
fi

	oldProfile=$vidRadioProfile
	
        if [ $2 -gt 1901 ] ;then
		
		    # Change resolution (some resolutions are only available on OpenIPC Mario AIO)
			CURRENT_SIZE=`yaml-cli -g .video0.size`
      		CURRENT_FPS=`yaml-cli -g .video0.fps`
      		if [ $CURRENT_SIZE == '1920x1080' ] && [ $CURRENT_FPS == '120' ] ; then
        		/root/1184p100.sh
      		elif [ $CURRENT_SIZE == '2104x1184' ] && [ $CURRENT_FPS == '100' ] ; then
        		/root/1248p90.sh
      		elif [ $CURRENT_SIZE == '2208x1248' ] && [ $CURRENT_FPS == '90' ] ; then
        		/root/1304p80.sh
      		elif [ $CURRENT_SIZE == '2312x1304' ] && [ $CURRENT_FPS == '80' ] ; then
        		/root/1416p70.sh
      		elif [ $CURRENT_SIZE == '2512x1416' ] && [ $CURRENT_FPS == '70' ] ; then
        		/root/1440p60.sh
      		elif [ $CURRENT_SIZE == '2560x1440' ] && [ $CURRENT_FPS == '60' ] ; then
        		/root/1920p30.sh
      		elif [ $CURRENT_SIZE == '2560x1920' ] && [ $CURRENT_FPS == '30' ] ; then
        		/root/1080p60.sh
      		elif [ $CURRENT_SIZE == '1920x1080' ] && [ $CURRENT_FPS == '60' ] ; then
        		/root/720p120.sh
      		elif [ $CURRENT_SIZE == '1280x720' ] && [ $CURRENT_FPS == '120' ] ; then
        		/root/720p100.sh
      		elif [ $CURRENT_SIZE == '1280x720' ] && [ $CURRENT_FPS == '100' ] ; then
        		/root/720p90.sh
      		elif [ $CURRENT_SIZE == '1280x720' ] && [ $CURRENT_FPS == '90' ] ; then
        		/root/720p60.sh
      		else
        		/root/1080p120.sh
      		fi
			sleep 1
		
        elif [ $2 -gt 1701 ] && [ $2 -lt 1900 ] ;then
			sleep 1
        elif [ $2 -gt 1501 ] && [ $2 -lt 1700 ] ;then
			sleep 1
		elif [ $2 -gt 1351 ] && [ $2 -lt 1500 ] ;then

			setGI=short
			setMCS=4
			setFecK=8
			setFecN=12
			setBitrate=15360
			setGop=1.0
			wfbPower=45
			newProfile=1
			echo "vidRadioProfile1" >/etc/txprofile
		
		elif [ $2 -gt 1051 ] && [ $2 -lt 1350 ] ;then
	
			setGI=short
			setMCS=2
			setFecK=8
			setFecN=12
			setBitrate=9216
			setGop=1.0
			wfbPower=58
			newProfile=2
			echo "vidRadioProfile=2" >/etc/txprofile
	
		elif [ $2 -lt 1050 ] ;then

			setGI=short
			setMCS=0
			setFecK=8
			setFecN=12
			setBitrate=3072
			setGop=1.0
			wfbPower=58
			newProfile=3
			echo "vidRadioProfile=3" >/etc/txprofile
						
		fi	

	#calculate driver power
	setPower=$((wfbPower * 50))

	if [ $newProfile -gt $oldProfile ] ;then
		
		# Lower power first
		iw dev wlan0 set txpower fixed $setPower
	
		# Set gopSize
		curl localhost/api/v1/set?video0.gopSize=$setGop
	
		# Raise MCS
		wfb_tx_cmd 8000 set_radio -B 20 -G $setGI -S 1 -L 1 -M $setMCS
		wfb_tx_cmd 8000 set_fec -k $setFecK -n $setFecN
	
		# Increase bit-rate
		curl -s "http://localhost/api/v1/set?video0.bitrate=$setBitrate"
        sleep 0.1
        curl "localhost:9000/api/osd/2?font=UbuntuMono-Regular&size=32.0&color=green&text=Bitrate:$setBitrate%20MCS:$setMCS%20FEC:$setFecK%2F$setFecN%20wfbPower:$wfbPower"
		
	elif [ $newProfile -lt $oldProfile ] ;then

		# Decrease bit-rate first
		curl -s "http://localhost/api/v1/set?video0.bitrate=$setBitrate"
	
		# Set gopSize
		curl localhost/api/v1/set?video0.gopSize=$setGop
	
		# Lower MCS
		sleep 0.1
		wfb_tx_cmd 8000 set_radio -B 20 -G $setGI -S 1 -L 1 -M $setMCS
		wfb_tx_cmd 8000 set_fec -k $setFecK -n $setFecN
	
		# Increase power
		iw dev wlan0 set txpower fixed $setPower
		sleep 0.1
		curl "localhost:9000/api/osd/2?font=UbuntuMono-Regular&size=32.0&color=green&text=Bitrate:$setBitrate%20MCS:$setMCS%20FEC:$setFecK%2F$setFecN%20wfbPower:$wfbPower"
    fi

exit 1
