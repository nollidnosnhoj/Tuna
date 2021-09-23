import { useCallback } from "react";
import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import request from "~/lib/http";
import { ID } from "~/lib/types";
import { Playlist, PlaylistRequest } from "../../types";
import { GET_PLAYLIST_KEY } from "../queries/useGetPlaylist";

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
