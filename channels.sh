#if [ $1 -eq 6 ]; then
#    if [ $2 -gt 1600 ]; then
#      CURRENT_FPS=`yaml-cli -g .video0.fps`
#      if [ $CURRENT_FPS -lt 120 ]; then
#        NEW_FPS="$(($CURRENT_FPS+10))"
#      else
#        NEW_FPS="20"
#        sleep 2
#      fi
#      yaml-cli -s .video0.fps $NEW_FPS
#      if [ $NEW_FPS -lt 40 ]; then
#        yaml-cli -s .isp.sensorConfig /etc/sensors/imx415.bin
#      else
#        yaml-cli -s .isp.sensorConfig /etc/sensors/imx415e.bin
#      fi
#      if [ $NEW_FPS -lt 50 ]; then
#        yaml-cli -s .isp.exposure 10
#      elif [ $NEW_FPS -gt 50 ] && [ $NEW_FPS -lt 90 ]; then
#        yaml-cli -s .isp.exposure 20
#      else
#        yaml-cli -s .isp.exposure 30
#      fi
#      sleep .2
#      /root/kill.sh
#    fi
#    sleep 3
#fi

#if [ $1 -eq 6 ]; then
#    if [ $2 -gt 1600 ]; then
#      CURRENT_ISP=`yaml-cli -g .isp.sensorConfig`
#      if [ $CURRENT_ISP == '/etc/sensors/imx415.bin'  ]; then
#        yaml-cli -s .isp.sensorConfig /etc/sensors/imx415_fpv.bin
#      elif [ $CURRENT_ISP == '/etc/sensors/imx415_fpv.bin'  ]; then
#        yaml-cli -s .isp.sensorConfig /etc/sensors/imx415vin3.bin
#      else
#        yaml-cli -s .isp.sensorConfig /etc/sensors/imx415.bin
#      fi
#      sleep .2
#      /root/kill.sh
#    fi
#    sleep 3
#fi

if [ $1 -eq 7 ]; then
    if [ $2 -gt 1600 ]; then
      CURRENT_BITRATE=`yaml-cli -g .video0.bitrate`
      if [ $CURRENT_BITRATE -lt 7168 ]; then
        NEW_BITRATE="$(($CURRENT_BITRATE+1024))"
      else
        NEW_BITRATE="1024"
        sleep 2
      fi
      yaml-cli -s .video0.bitrate $NEW_BITRATE
      sleep .2
      /root/kill.sh
    fi
    sleep 3
fi

if [ $1 -eq 8 ]; then
    if [ $2 -gt 1600 ]; then
      CURRENT_SIZE=`yaml-cli -g .video0.size`
      CURRENT_FPS=`yaml-cli -g .video0.fps`
      if [ $CURRENT_SIZE == '1456x816' ] && [ $CURRENT_FPS == '120' ] ; then
        /root/1080.sh
        sleep 3
      elif [ $CURRENT_SIZE == '1920x1080' ] && [ $CURRENT_FPS == '90' ] ; then
        /root/1264.sh
        sleep 3
      elif [ $CURRENT_SIZE == '2240x1264' ] && [ $CURRENT_FPS == '60' ] ; then
        /root/3K.sh
        sleep 3
      elif [ $CURRENT_SIZE == '3200x1800' ] ; then
        /root/4K.sh
        sleep 3
      else
        /root/816.sh
        sleep 3
      fi
    fi
fi

exit 1