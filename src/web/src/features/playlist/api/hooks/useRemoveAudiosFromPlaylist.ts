import { useCallback } from "react";
import { useMutation, UseMutationResult } from "react-query";
import request from "~/lib/http";
import { ID } from "~/lib/types";

export function useRemoveAudiosFromPlaylist(
  playlistId: ID
): UseMutationResult<void, unknown, ID[]> {
  const handler = useCallback(
    async (audioIds: ID[]) => {
      await request({
        method: "delete",
        url: `playlists/${playlistId}/audios`,
        data: {
          playlistAudioIds: audioIds,
        },
      });
    },
    [playlistId]
  );

  return useMutation<void, unknown, ID[]>(handler);
}
