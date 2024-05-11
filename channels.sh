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

exit 1