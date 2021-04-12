import { Box, chakra, Flex, HStack, Icon, IconButton } from "@chakra-ui/react";
import React, { useCallback } from "react";
import {
  MdPause,
  MdPlayArrow,
  MdSkipNext,
  MdSkipPrevious,
} from "react-icons/md";

interface PlayerControlsProps {
  isPlaying: boolean;
  hasNoPrevious?: boolean;
  hasNoNext?: boolean;
  onTogglePlay: (e: React.SyntheticEvent) => void;
  onPrevious?: () => void;
  onNext?: () => void;
}

export default function PlayerControls(props: PlayerControlsProps) {
  const {
    isPlaying,
    onTogglePlay,
    onPrevious,
    onNext,
    hasNoNext = true,
    hasNoPrevious = false,
  } = props;

  return (
    <HStack>
      <IconButton
        icon={<MdSkipPrevious />}
        aria-label="Previous"
        title="Previous"
        onClick={onPrevious}
        disabled={hasNoPrevious}
        isRound
        variant="ghost"
        size="lg"
        fontSize="25px"
      />
      <IconButton
        icon={isPlaying ? <MdPause /> : <MdPlayArrow />}
        onClick={onTogglePlay}
        aria-label={isPlaying ? "Pause" : "Play"}
        title={isPlaying ? "Pause" : "Play"}
        isRound
        size="lg"
        colorScheme="primary"
        fontSize="25px"
      />
      <IconButton
        icon={<MdSkipNext />}
        aria-label="Next"
        title="Next"
        onClick={onNext}
        disabled={hasNoNext}
        isRound
        variant="ghost"
        size="lg"
        fontSize="25px"
      />
    </HStack>
  );
}
