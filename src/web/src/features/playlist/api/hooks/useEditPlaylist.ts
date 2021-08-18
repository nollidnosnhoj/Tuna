import { useCallback } from "react";
import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import request from "~/lib/http";
import { Playlist, PlaylistId, PlaylistRequest } from "../types";
import { GET_PLAYLIST_KEY } from "./useGetPlaylist";

export function useEditPlaylist(id: PlaylistId): UseMutationResult<Playlist> {
  const qc = useQueryClient();

  const mutate = useCallback(
    async (input: PlaylistRequest) => {
      const { data } = await request({
        method: "put",
        url: `playlists/${id}`,
        data: input,
      });
      return data;
    },
    [id]
  );

  return useMutation(mutate, {
    onSuccess: (data) => {
      qc.setQueryData<Playlist>(GET_PLAYLIST_KEY(id), data);
    },
  });
}
