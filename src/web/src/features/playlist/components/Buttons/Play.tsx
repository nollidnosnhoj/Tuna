import { ButtonProps, IconButton } from "@chakra-ui/react";
import React, { useCallback } from "react";
import { FaPause, FaPlay } from "react-icons/fa";
import { useAudioPlayer } from "~/lib/stores";
import { Playlist } from "../../api/types";

interface PlaylistPlayButtonProps extends ButtonProps {
  playlist: Playlist;
  onPlay: () => Promise<void>;
}

export default function PlaylistPlayButton({
  playlist,
  onPlay,
  ...buttonProps
}: PlaylistPlayButtonProps) {
  const context = useAudioPlayer((state) => state.context);
  const [isPlaying, setIsPlaying] = useAudioPlayer((state) => [
    state.isPlaying,
    state.setIsPlaying,
  ]);
  const isContext = context === `playlist:${playlist.id}`;
  const handleClick = useCallback(async () => {
    if (isContext) {
      setIsPlaying(!isPlaying);
    } else {
      await onPlay();
    }
  }, [isContext, isPlaying]);

  return (
    <IconButton
      isRound
      colorScheme="pink"
      icon={isContext && isPlaying ? <FaPause /> : <FaPlay />}
      aria-label="Play"
      alt="Play"
      onClick={handleClick}
      {...buttonProps}
    />
  );
}
