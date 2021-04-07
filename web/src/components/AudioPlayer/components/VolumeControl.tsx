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

const VOLUME_KEY = "audiochan_volume";

interface VolumeControlProps {
  audioNode: HTMLAudioElement | null;
  volume: number;
  onChange: (value: number) => void;
}

export default function VolumeControl(props: VolumeControlProps) {
  const { audioNode, volume, onChange } = props;
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
    const localData = localStorage.getItem(VOLUME_KEY);
    if (localData) {
      let parsedVolume = Math.floor(parseFloat(localData));
      onChange(parsedVolume);
    }
  }, []);

  useEffect(() => {
    if (audioNode) {
      audioNode.volume = volume / 100;
    }

    var saveTimer = setTimeout(() => {
      localStorage.setItem(VOLUME_KEY, volume.toFixed(2));
    }, 200);

    return () => {
      clearTimeout(saveTimer);
    };
  }, [audioNode, volume]);

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
