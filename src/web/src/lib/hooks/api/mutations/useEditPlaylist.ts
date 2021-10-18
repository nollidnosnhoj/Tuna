import { useCallback } from "react";
import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import request from "~/lib/http";
import { ID, Playlist, PlaylistRequest } from "~/lib/types";
import { GET_PLAYLIST_KEY } from "~/lib/hooks/api/keys";

export function useEditPlaylist(playlistId: ID): UseMutationResult<Playlist> {
  const qc = useQueryClient();

  const mutate = useCallback(
    async (input: PlaylistRequest) => {
      const { data } = await request({
        method: "put",
        url: `playlists/${playlistId}`,
        data: input,
      });
      return data;
    },
    [playlistId]
  );

  return useMutation(mutate, {
    onSuccess: (data) => {
      qc.setQueryData<Playlist>(GET_PLAYLIST_KEY(playlistId), data);
    },
  });
}
