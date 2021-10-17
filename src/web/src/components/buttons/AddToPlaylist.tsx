import { AddIcon } from "@chakra-ui/icons";
import { IconButton } from "@chakra-ui/react";
import React from "react";
import { useAddToPlaylist } from "~/lib/stores";
import { AudioView } from "~/lib/types";
import { useUser } from "~/components/providers/UserProvider";

interface AddToPlaylistButtonProps {
  audio: AudioView;
}

export default function AddToPlaylistButton({
  audio,
}: AddToPlaylistButtonProps) {
  const { user } = useUser();
  const addToPlaylist = useAddToPlaylist((state) => state.openDialog);

  if (!user) return null;
  // if (audio.user.id !== user?.id) return null;

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
