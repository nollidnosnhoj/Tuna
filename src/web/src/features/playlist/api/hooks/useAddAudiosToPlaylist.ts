import { useMutation, UseMutationResult } from "react-query";
import { AudioId } from "~/features/audio/api/types";
import request from "~/lib/http";
import { PlaylistId } from "../types";

type UseAddAudiosToPlaylistInputs = {
  id: PlaylistId;
  audioIds: AudioId[];
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
    const { id, audioIds } = inputs;
    await request({
      method: "put",
      url: `playlists/${id}/audios`,
      data: {
        audioIds,
      },
    });
  };

  return useMutation(handler);
}
