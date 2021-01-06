import {
  Box,
  IconButton,
  Popover,
  PopoverBody,
  PopoverContent,
  PopoverTrigger,
  Slider,
  SliderFilledTrack,
  SliderThumb,
  SliderTrack,
} from "@chakra-ui/react";
import React, { useMemo, useRef } from "react";
import { FaVolumeDown, FaVolumeMute, FaVolumeUp } from "react-icons/fa";
import { useAudioPlayer } from "~/lib/contexts/audio_player_context";

export default function VolumeSliderHeader() {
  const { volume, handleVolume } = useAudioPlayer();
  const volumeRef = useRef();

  const volumeIcon = useMemo(() => {
    if (volume <= 0) {
      return <FaVolumeMute />;
    }

    if (volume >= 0.5) {
      return <FaVolumeUp />;
    }

    return <FaVolumeDown />;
  }, [volume]);

  return (
    <Box>
      <Popover initialFocusRef={volumeRef} placement="bottom">
        <PopoverTrigger>
          <IconButton
            aria-label="Set volume"
            icon={volumeIcon}
            variant="ghost"
          />
        </PopoverTrigger>
        <PopoverContent>
          <PopoverBody>
            <Slider
              ref={volumeRef}
              value={volume}
              min={0}
              max={1}
              step={0.1}
              onChange={(v) => handleVolume(v)}
            >
              <SliderTrack>
                <SliderFilledTrack />
              </SliderTrack>
              <SliderThumb />
            </Slider>
          </PopoverBody>
        </PopoverContent>
      </Popover>
    </Box>
  );
}
