import { HStack, IconButton } from "@chakra-ui/react";
import React from "react";
import {
  MdPause,
  MdPlayArrow,
  MdSkipNext,
  MdSkipPrevious,
} from "react-icons/md";
import { useAudioPlayer } from "~/contexts/AudioPlayerContext";

interface PlayerControlsProps {
  onTogglePlay: (e: React.SyntheticEvent) => void;
}

export default function PlayerControls(props: PlayerControlsProps) {
  const { onTogglePlay } = props;

  const { state, dispatch } = useAudioPlayer();
  const { isPlaying, playIndex, queue } = state;

  return (
    <HStack>
      <IconButton
        icon={<MdSkipPrevious />}
        aria-label="Previous"
        title="Previous"
        onClick={() => dispatch({ type: "PLAY_PREVIOUS" })}
        disabled={playIndex === 0}
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
        onClick={() => dispatch({ type: "PLAY_NEXT" })}
        disabled={playIndex === queue.length - 1}
        isRound
        variant="ghost"
        size="lg"
        fontSize="25px"
      />
    </HStack>
  );
}
