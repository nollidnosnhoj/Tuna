import { HStack, IconButton } from "@chakra-ui/react";
import React, { useCallback, useMemo } from "react";
import {
  MdPause,
  MdPlayArrow,
  MdSkipNext,
  MdSkipPrevious,
} from "react-icons/md";
import { useAudioPlayer } from "~/lib/contexts/AudioPlayerContext";

interface PlayerControlsProps {
  size?: "desktop";
}

export default function PlayerControls(props: PlayerControlsProps) {
  const { size = "desktop" } = props;
  const { state, dispatch } = useAudioPlayer();
  const { isPlaying, playIndex, queue } = state;

  const buttonSize = useMemo(() => {
    switch (size) {
      default:
        return "md";
    }
  }, [size]);

  const handleTogglePlay = useCallback(() => {
    if (playIndex !== undefined) {
      dispatch({ type: "TOGGLE_PLAYING" });
    }
  }, [playIndex]);

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
        size={buttonSize}
        fontSize="25px"
      />
      <IconButton
        icon={isPlaying ? <MdPause /> : <MdPlayArrow />}
        onClick={handleTogglePlay}
        aria-label={isPlaying ? "Pause" : "Play"}
        title={isPlaying ? "Pause" : "Play"}
        isRound
        size={buttonSize}
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
        size={buttonSize}
        fontSize="25px"
      />
    </HStack>
  );
}
