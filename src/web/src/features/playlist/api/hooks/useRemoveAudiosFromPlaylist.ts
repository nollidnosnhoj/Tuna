import { useMutation, UseMutationResult } from "react-query";
import { removeAudiosFromPlaylistRequests } from "..";
import { PlaylistAudioId, PlaylistId } from "../types";

export function useRemoveAudiosFromPlaylist(id: PlaylistId): UseMutationResult {
  const handler = async (audioIds: PlaylistAudioId[]): Promise<void> => {
    return await removeAudiosFromPlaylistRequests(id, audioIds);
  };

  return useMutation(handler);
}
