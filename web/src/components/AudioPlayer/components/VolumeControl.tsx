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

interface VolumeControlProps {
  audioNode: HTMLAudioElement | null;
  volume: number;
  onChange: (value: number) => void;
  saveToLocalStorage?: boolean;
}

export default function VolumeControl(props: VolumeControlProps) {
  const { audioNode, volume, onChange, saveToLocalStorage = false } = props;
  const lastVolumeRef = useRef<number>();

  const handleVolumeChange = (value: number) => {
    onChange(value);
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
      onChange(0);
    } else {
      onChange(lastVolumeRef.current ?? 0);
    }
  };

  useEffect(() => {
    const localData = localStorage.getItem("audiochan_volume");
    if (localData) {
      let parsedVolume = Math.floor(parseFloat(localData));
      onChange(parsedVolume);
    }
  }, [onChange]);

  useEffect(() => {
    if (audioNode) {
      audioNode.volume = volume / 100;
      if (saveToLocalStorage) {
        localStorage.setItem("audiochan_volume", volume.toString());
      }
    }
  }, [audioNode, volume, saveToLocalStorage]);

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
