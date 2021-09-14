import { DeleteIcon } from "@chakra-ui/icons";
import { IconButton } from "@chakra-ui/react";
import React, { useCallback } from "react";
import { useRemoveAudiosFromPlaylist } from "~/features/playlist/api/hooks";
import { Playlist } from "~/features/playlist/api/types";
import { useUser } from "~/features/user/hooks";
import { ID } from "~/lib/types";

interface RemoveFromPlaylistButtonProps {
  playlist: Playlist;
  playlistAudioId: ID;
}

export default function RemoveFromPlaylistButton({
  playlist,
  playlistAudioId,
}: RemoveFromPlaylistButtonProps) {
  const { user } = useUser();
  const { mutateAsync: removeFromPlaylistAsync } = useRemoveAudiosFromPlaylist(
    playlist.id
  );
  const handleRemove = useCallback(async () => {
    if (!confirm("Are you sure you want to remove audio from playlist?"))
      return;
    await removeFromPlaylistAsync([playlistAudioId]);
  }, [playlistAudioId]);

  if (playlist.user.id !== user?.id) return null;

  return (
    <IconButton
      aria-label="Remove From Playlist"
      icon={<DeleteIcon />}
      onClick={handleRemove}
      variant="ghost"
      isRound
    >
      Remove From Playlist
    </IconButton>
  );
}
