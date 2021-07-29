import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { createPlaylistRequest } from "../api";
import { CreatePlaylistRequest, Playlist } from "../types";
import { GET_PLAYLIST_KEY } from "./useGetPlaylist";

export function useCreatePlaylist(): UseMutationResult<
  Playlist,
  unknown,
  CreatePlaylistRequest,
  unknown
> {
  const qc = useQueryClient();
  return useMutation(createPlaylistRequest, {
    onSuccess(data) {
      qc.setQueryData<Playlist>(GET_PLAYLIST_KEY(data.id), data);
    },
  });
}
