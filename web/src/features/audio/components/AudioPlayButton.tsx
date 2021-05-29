import { IconButton } from "@chakra-ui/react";
import React, { useCallback, useMemo } from "react";
import { FaPause, FaPlay } from "react-icons/fa";
import { useAudioPlayer, useAudioQueue } from "~/lib/stores";
import { mapAudioForAudioQueue } from "~/utils";
import { AudioData } from "../types";

interface AudioPlayButtonProps {
  audio: AudioData;
}

export default function AudioPlayButton({ audio }: AudioPlayButtonProps) {
  const [isPlaying, setIsPlaying] = useAudioPlayer((state) => [
    state.isPlaying,
    state.setIsPlaying,
  ]);
  const setNewQueue = useAudioQueue((state) => state.setNewQueue);
  const currentAudio = useAudioQueue((state) => state.current);

  const isAudioPlaying = useMemo(() => {
    return isPlaying && currentAudio?.audioId === audio.id;
  }, [isPlaying, currentAudio?.audioId, audio.id]);

  const clickPlayButton = useCallback(() => {
    if (isAudioPlaying) {
      setIsPlaying(!isPlaying);
    } else {
      setNewQueue(mapAudioForAudioQueue(audio), 0);
    }
  }, [isAudioPlaying, audio.id, isPlaying]);

  return (
    <IconButton
      isRound
      colorScheme="pink"
      size="lg"
      icon={isAudioPlaying ? <FaPause /> : <FaPlay />}
      aria-label="Play"
      alt="Play"
      onClick={clickPlayButton}
    />
  );
}
