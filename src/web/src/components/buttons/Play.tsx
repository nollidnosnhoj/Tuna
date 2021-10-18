import { ButtonProps, IconButton } from "@chakra-ui/react";
import React, { useCallback, useMemo } from "react";
import { FaPause, FaPlay } from "react-icons/fa";
import { useAudioPlayer } from "~/lib/stores";
import { AudioView } from "~/lib/types";

interface AudioPlayButtonProps extends ButtonProps {
  audio: AudioView;
}

export default function AudioPlayButton({
  audio,
  ...buttonProps
}: AudioPlayButtonProps) {
  const {
    isPlaying,
    setIsPlaying,
    setNewQueue,
    current: currentAudio,
  } = useAudioPlayer();

  const isAudioPlaying = useMemo(() => {
    return isPlaying && currentAudio?.audioId === audio.id;
  }, [isPlaying, currentAudio?.audioId, audio.id]);

  const clickPlayButton = useCallback(() => {
    if (isAudioPlaying) {
      setIsPlaying(!isPlaying);
    } else {
      setNewQueue("custom", [audio], 0);
    }
  }, [isAudioPlaying, audio.id, isPlaying]);

  return (
    <IconButton
      isRound
      colorScheme="pink"
      icon={isAudioPlaying ? <FaPause /> : <FaPlay />}
      aria-label="Play"
      alt="Play"
      onClick={clickPlayButton}
      {...buttonProps}
    />
  );
}