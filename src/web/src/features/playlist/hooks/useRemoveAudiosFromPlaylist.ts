import { useMutation, UseMutationResult } from "react-query";
import { removeAudiosFromPlaylistRequests } from "../api";

export function useRemoveAudiosFromPlaylist(id: string): UseMutationResult {
  const handler = async (audioIds: string[]): Promise<void> => {
    return await removeAudiosFromPlaylistRequests(id, audioIds);
  };

  return useMutation(handler);
}
