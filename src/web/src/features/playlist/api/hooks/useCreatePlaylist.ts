import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import request from "~/lib/http";
import { CreatePlaylistRequest, Playlist } from "../types";
import { GET_PLAYLIST_KEY } from "./useGetPlaylist";

export function useCreatePlaylist(): UseMutationResult<
  Playlist,
  unknown,
  CreatePlaylistRequest,
  unknown
> {
  const qc = useQueryClient();
  async function mutate(inputs: CreatePlaylistRequest): Promise<Playlist> {
    const { data } = await request<Playlist>({
      method: "post",
      url: "playlists",
      data: inputs,
    });
    return data;
  }
  return useMutation(mutate, {
    onSuccess(data) {
      qc.setQueryData<Playlist>(GET_PLAYLIST_KEY(data.id), data);
    },
  });
}
