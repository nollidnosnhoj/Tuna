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
import { useAudioPlayer } from "~/contexts/AudioPlayerContext";

interface ProgressBarProps {
  audioNode: HTMLAudioElement | null;
  duration?: number;
}

const EMPTY_TIME_FORMAT = "--:--";

export default function ProgressBar(props: ProgressBarProps) {
  const { audioNode, duration } = props;
  const { state, dispatch } = useAudioPlayer();
  const { currentTime } = state;
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
        const time = Math.ceil(value);
        if (audioNode) audioNode.currentTime = time;
        setSliderValue(time);
        dispatch({ type: "SET_CURRENT_TIME", payload: time });
      }
      setIsDraggingProgress(false);
    },
    [isDraggingProgress]
  );

  const updateSeek = useCallback(
    _.throttle(() => {
      const time = Math.ceil(audioNode?.currentTime ?? 0);
      dispatch({ type: "SET_CURRENT_TIME", payload: time });
      if (!isDraggingProgress) {
        setSliderValue(time);
      }
    }, 200),
    [audioNode, isDraggingProgress]
  );

  useEffect(() => {
    audioNode?.addEventListener("timeupdate", updateSeek);

    return () => {
      audioNode?.removeEventListener("timeupdate", updateSeek);
    };
  }, [audioNode, updateSeek]);

  return (
    <Flex alignItems="center" width="100%">
      <Box fontSize="sm">{formattedCurrentTime}</Box>
      <Box flex="1" marginX={4}>
        <Slider
          min={0}
          max={duration}
          value={sliderValue}
          onChangeStart={handleSliderChangeStart}
          onChange={handleSliderChange}
          onChangeEnd={handleSliderChangeEnd}
          focusThumbOnChange={false}
          colorScheme="primary"
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
