import { Box, chakra, Flex, Icon } from "@chakra-ui/react";
import React, { useCallback } from "react";
import {
  MdPause,
  MdPlayArrow,
  MdSkipNext,
  MdSkipPrevious,
} from "react-icons/md";

interface PlayerControlsProps {
  isPlaying: boolean;
  onTogglePlay: (e: React.SyntheticEvent) => void;
  onPrevious: () => void;
  onNext: () => void;
}

export default function PlayerControls(props: PlayerControlsProps) {
  const { isPlaying, onTogglePlay, onPrevious, onNext } = props;

  return (
    <Flex fontSize="40px">
      <Box>
        <chakra.button
          onClick={onPrevious}
          aria-label="Previous"
          title="Previous"
        >
          <Icon as={MdSkipPrevious} />
        </chakra.button>
      </Box>
      <Box>
        <chakra.button onClick={onTogglePlay}>
          <Icon
            as={isPlaying ? MdPause : MdPlayArrow}
            aria-label={isPlaying ? "Pause" : "Play"}
            title={isPlaying ? "Pause" : "Play"}
          />
        </chakra.button>
      </Box>
      <Box>
        <chakra.button onClick={onNext}>
          <Icon as={MdSkipNext} aria-label="Next" title="Next" />
        </chakra.button>
      </Box>
    </Flex>
  );
}
