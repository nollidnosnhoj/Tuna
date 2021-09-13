import { useMutation, UseMutationResult } from "react-query";
import request from "~/lib/http";
import { ID } from "~/lib/types";

type UseAddAudiosToPlaylistInputs = {
  playlistId: ID;
  audioIds: ID[];
};

export function useAddAudiosToPlaylist(): UseMutationResult<
  void,
  unknown,
  UseAddAudiosToPlaylistInputs,
  unknown
> {
  const handler = async (
    inputs: UseAddAudiosToPlaylistInputs
  ): Promise<void> => {
    const { playlistId, audioIds } = inputs;
    await request({
      method: "put",
      url: `playlists/${playlistId}/audios`,
      data: {
        audioIds,
      },
    });
  };

  return useMutation(handler);
}
