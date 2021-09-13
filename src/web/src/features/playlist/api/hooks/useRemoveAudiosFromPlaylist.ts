import { useCallback } from "react";
import { useMutation, UseMutationResult } from "react-query";
import request from "~/lib/http";
import { PlaylistAudioId, PlaylistId } from "../types";

export function useRemoveAudiosFromPlaylist(
  id: PlaylistId
): UseMutationResult<void, unknown, PlaylistAudioId[]> {
  const handler = useCallback(
    async (audioIds: PlaylistAudioId[]) => {
      await request({
        method: "delete",
        url: `playlists/${id}/audios`,
        data: {
          playlistAudioIds: audioIds,
        },
      });
    },
    [id]
  );

  return useMutation<void, unknown, PlaylistAudioId[]>(handler);
}
