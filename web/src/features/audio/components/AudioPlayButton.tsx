import { IconButton } from "@chakra-ui/react";
import React, { useCallback, useMemo } from "react";
import { FaPause, FaPlay } from "react-icons/fa";
import { useAudioPlayer } from "~/lib/hooks/useAudioPlayer";
import { mapAudioForAudioQueue } from "~/utils";
import { AudioData } from "../types";

interface AudioPlayButtonProps {
  audio: AudioData;
}

export default function AudioPlayButton({ audio }: AudioPlayButtonProps) {
  const { state, dispatch } = useAudioPlayer();
  const { isPlaying, currentAudio } = state;

  const isAudioPlaying = useMemo(() => {
    return isPlaying && currentAudio?.audioId === audio.id;
  }, [isPlaying, currentAudio?.audioId, audio.id]);

  const clickPlayButton = useCallback(() => {
    if (isAudioPlaying) {
      dispatch({ type: "SET_PLAYING", payload: !isPlaying });
    } else {
      dispatch({
        type: "SET_NEW_QUEUE",
        payload: mapAudioForAudioQueue(audio),
        index: 0,
      });
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
