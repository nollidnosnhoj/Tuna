import { useMutation, UseMutationResult } from "react-query";
import { addAudiosToPlaylistRequest } from "../api";

export function useAddAudiosToPlaylist(id: string): UseMutationResult {
  const handler = async (audioIds: string[]): Promise<void> => {
    return await addAudiosToPlaylistRequest(id, audioIds);
  };

  return useMutation(handler);
}
