import _ from "lodash";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import {
  Box,
  Flex,
  Slider,
  SliderFilledTrack,
  SliderThumb,
  SliderTrack,
} from "@chakra-ui/react";
import { formatDuration } from "~/utils/format";
import { useAudioPlayer } from "~/lib/stores";

const EMPTY_TIME_FORMAT = "--:--";

export default function ProgressBar() {
  const playIndex = useAudioPlayer((state) => state.playIndex);
  const audioRef = useAudioPlayer((state) => state.audioRef);
  const [currentTime, setCurrentTime] = useAudioPlayer((state) => [
    state.currentTime,
    state.setCurrentTime,
  ]);
  const currentAudio = useAudioPlayer((state) => state.current);
  const { duration } = currentAudio || { duration: 0 };
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
        if (audioRef) audioRef.currentTime = value;
        setCurrentTime(value);
      }
      setIsDraggingProgress(false);
    },
    [audioRef, isDraggingProgress]
  );

  const updateSeek = useCallback(
    _.throttle(() => {
      const time = audioRef?.currentTime ?? (currentTime || 0);
      setCurrentTime(time);
      if (!isDraggingProgress) {
        setSliderValue(time);
      }
    }, 20),
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
          colorScheme="primary"
          value={sliderValue}
          min={0}
          max={audioRef?.duration || duration || 100}
          step={1}
          onChangeStart={handleSliderChangeStart}
          onChangeEnd={handleSliderChangeEnd}
          onChange={handleSliderChange}
          focusThumbOnChange={false}
          isDisabled={playIndex === undefined}
        >
          <SliderTrack>
            <SliderFilledTrack />
          </SliderTrack>
          <SliderThumb />
        </Slider>
      </Box>
      <Box fontSize="sm">{formattedDuration}</Box>
    </Flex>
  );
}
