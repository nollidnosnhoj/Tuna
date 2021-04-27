import _ from "lodash";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import {
  Box,
  Flex,
  toCSSObject,
  useColorModeValue,
  useMultiStyleConfig,
  useToken,
} from "@chakra-ui/react";
import Slider from "rc-slider";
import { formatDuration } from "~/utils/format";
import { useAudioPlayer } from "~/contexts/AudioPlayerContext";

const EMPTY_TIME_FORMAT = "--:--";

export default function ProgressBar() {
  /**
   * This is a dog water way of trying to incorporate chakra's slider colors into rc-slider
   */
  const styles = useMultiStyleConfig("Slider", {});
  const filledTrackBgColorToken = styles.filledTrack.backgroundColor;
  const trackBgColorToken = styles.track.backgroundColor;
  const filledTrackColor = useToken(
    "colors",
    filledTrackBgColorToken as string
  );
  const trackColor = useToken("colors", trackBgColorToken as string);

  const { state, dispatch } = useAudioPlayer();
  const { audioRef, currentTime, currentAudio: currentPlaying } = state;
  const { duration } = currentPlaying || { duration: 0 };
  const [sliderValue, setSliderValue] = useState(0);
  const [isDraggingProgress, setIsDraggingProgress] = useState(false);

  const formattedCurrentTime = useMemo(() => {
    if (currentTime === undefined) return EMPTY_TIME_FORMAT;
    return formatDuration(currentTime);
  }, [currentTime]);

  const formattedDuration = useMemo(() => {
    if (duration === undefined) return EMPTY_TIME_FORMAT;
    return formatDuration(duration);
  }, [duration]);

  const handleSliderChange = useCallback(
    (value: number) => {
      if (isDraggingProgress) {
        setSliderValue(value);
      }
    },
    [isDraggingProgress]
  );

  const handleSliderChangeStart = () => {
    setIsDraggingProgress(true);
  };

  const handleSliderChangeEnd = useCallback(
    (value: number) => {
      if (isDraggingProgress) {
        const time = value;
        if (audioRef) audioRef.currentTime = time;
        setSliderValue(time);
        dispatch({ type: "SET_CURRENT_TIME", payload: time });
      }
      setIsDraggingProgress(false);
    },
    [isDraggingProgress]
  );

  const updateSeek = useCallback(
    _.throttle(() => {
      const time = audioRef?.currentTime ?? 0;
      dispatch({ type: "SET_CURRENT_TIME", payload: time });
      if (!isDraggingProgress) {
        setSliderValue(time);
      }
    }, 200),
    [audioRef, isDraggingProgress]
  );

  useEffect(() => {
    audioRef?.addEventListener("timeupdate", updateSeek);

    return () => {
      audioRef?.removeEventListener("timeupdate", updateSeek);
    };
  }, [audioRef, updateSeek]);

  return (
    <Flex alignItems="center" width="100%">
      <Box fontSize="sm">{formattedCurrentTime}</Box>
      <Box flex="1" marginX={4}>
        <Slider
          min={0}
          max={audioRef?.duration || duration}
          step={0.1}
          value={sliderValue}
          onBeforeChange={handleSliderChangeStart}
          onChange={handleSliderChange}
          onAfterChange={handleSliderChangeEnd}
          disabled={!currentPlaying}
          handleStyle={{
            boxShadow: "var(--chakra-shadows-base)",
            border: 0,
          }}
          trackStyle={{
            backgroundColor: trackColor,
          }}
          railStyle={{
            backgroundColor: filledTrackColor,
          }}
        />
      </Box>
      <Box fontSize="sm">{formattedDuration}</Box>
    </Flex>
  );
}
