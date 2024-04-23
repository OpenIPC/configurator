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
      if [ $CURRENT_SIZE == '1920x1080' ] && [ $CURRENT_FPS == '120' ] ; then
        /root/1184p100.sh
        sleep 3
      elif [ $CURRENT_SIZE == '2104x1184' ] && [ $CURRENT_FPS == '100' ] ; then
        /root/1304p80.sh
        sleep 3
      elif [ $CURRENT_SIZE == '2312x1304' ] && [ $CURRENT_FPS == '80' ] ; then
        /root/1440p60.sh
        sleep 3
      elif [ $CURRENT_SIZE == '2560x1440' ] && [ $CURRENT_FPS == '60' ] ; then
        /root/1920p30.sh
        sleep 3
      elif [ $CURRENT_SIZE == '2560x1920' ] && [ $CURRENT_FPS == '30' ] ; then
        /root/1080p60.sh
        sleep 3
      elif [ $CURRENT_SIZE == '1920x1080' ] && [ $CURRENT_FPS == '60' ] ; then
        /root/720p120.sh
        sleep 3
      elif [ $CURRENT_SIZE == '1280x720' ] && [ $CURRENT_FPS == '120' ] ; then
        /root/720p100.sh
        sleep 3
      elif [ $CURRENT_SIZE == '1280x720' ] && [ $CURRENT_FPS == '100' ] ; then
        /root/720p90.sh
        sleep 3
      elif [ $CURRENT_SIZE == '1280x720' ] && [ $CURRENT_FPS == '90' ] ; then
        /root/720p60.sh
        sleep 3
      else
        /root/1080p120.sh
        sleep 3
      fi
    fi
fi

exit 1