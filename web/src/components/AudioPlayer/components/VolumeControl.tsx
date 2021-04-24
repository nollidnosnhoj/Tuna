import {
  Slider,
  SliderTrack,
  SliderFilledTrack,
  SliderThumb,
  Flex,
  Box,
  chakra,
  Icon,
} from "@chakra-ui/react";
import React, { useEffect, useMemo, useRef } from "react";
import { MdVolumeDown, MdVolumeMute, MdVolumeUp } from "react-icons/md";
import { useAudioPlayer } from "~/contexts/AudioPlayerContext";

export default function VolumeControl() {
  const { state, dispatch } = useAudioPlayer();
  const { audioRef, volume } = state;
  const lastVolumeRef = useRef<number>();

  const handleVolumeChange = (value: number) => {
    dispatch({ type: "SET_VOLUME", payload: value });
  };

  const volumeIcon = useMemo(() => {
    if (volume === 0) {
      return <Icon as={MdVolumeMute} />;
    } else if (volume > 60) {
      return <Icon as={MdVolumeUp} />;
    } else {
      return <Icon as={MdVolumeDown} />;
    }
  }, [volume]);

  const handleVolumeButtonClick = () => {
    if (volume > 0) {
      lastVolumeRef.current = volume;
      handleVolumeChange(0);
    } else {
      handleVolumeChange(lastVolumeRef.current ?? 0);
    }
  };

  useEffect(() => {
    if (audioRef) {
      audioRef.volume = volume / 100;
    }
  }, [audioRef, volume]);

  return (
    <Flex>
      <Box marginRight={2}>
        <chakra.button
          onClick={handleVolumeButtonClick}
          aria-label={`Volume: ${volume}%`}
          title={`Volume: ${volume}%`}
        >
          {volumeIcon}
        </chakra.button>
      </Box>
      <Box flex="0 1 100%">
        <Slider
          value={volume}
          min={0}
          max={100}
          step={5}
          onChange={handleVolumeChange}
          colorScheme="primary"
          focusThumbOnChange={false}
        >
          <SliderTrack>
            <SliderFilledTrack />
          </SliderTrack>
          <SliderThumb />
        </Slider>
      </Box>
    </Flex>
  );
}
