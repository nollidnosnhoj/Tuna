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
import { useAudioPlayer } from "~/lib/hooks";

export default function VolumeControl() {
  const { audioRef, volume, setVolume } = useAudioPlayer();
  const lastVolumeRef = useRef<number>();

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
      setVolume(0);
    } else {
      setVolume(lastVolumeRef.current ?? 0);
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
          onChange={(v) => setVolume(v)}
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
