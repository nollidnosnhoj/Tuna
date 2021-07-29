import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { useUser } from "~/features/user/hooks";
import { updatePlaylistDetailsRequest } from "../api";
import { Playlist, PlaylistRequest } from "../types";
import { GET_PLAYLIST_KEY } from "./useGetPlaylist";

export function useEditPlaylist(id: string): UseMutationResult<Playlist> {
  const qc = useQueryClient();
  const { user } = useUser();

  const updatePlaylist = async (input: PlaylistRequest): Promise<Playlist> => {
    return await updatePlaylistDetailsRequest(id, input);
  };

  return useMutation(updatePlaylist, {
    onSuccess: (data) => {
      qc.setQueryData<Playlist>(GET_PLAYLIST_KEY(id), data);
    },
  });
}
