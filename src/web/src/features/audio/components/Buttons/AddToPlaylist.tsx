import { AddIcon } from "@chakra-ui/icons";
import { IconButton } from "@chakra-ui/react";
import React from "react";
import { useUser } from "~/features/user/hooks";
import { useAddToPlaylist } from "~/lib/stores";
import { AudioView } from "../../api/types";

interface AddToPlaylistButtonProps {
  audio: AudioView;
}

export default function AddToPlaylistButton({
  audio,
}: AddToPlaylistButtonProps) {
  const { user } = useUser();
  const addToPlaylist = useAddToPlaylist((state) => state.openDialog);

  if (audio.user.id !== user?.id) return null;

  return (
    <IconButton
      aria-label="Add To Playlist"
      icon={<AddIcon />}
      onClick={() => addToPlaylist([audio])}
      variant="ghost"
      isRound
    >
      Add To Playlist
    </IconButton>
  );
}
