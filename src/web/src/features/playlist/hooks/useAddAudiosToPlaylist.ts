/* eslint-disable @typescript-eslint/explicit-function-return-type */
/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
import { useMutation } from "react-query";
import { AudioId } from "~/features/audio/api/types";
import { addAudiosToPlaylistRequest } from "../api";
import { PlaylistId } from "../api/types";

export function useAddAudiosToPlaylist() {
  const handler = async (request: {
    id: PlaylistId;
    audioIds: AudioId[];
  }): Promise<void> => {
    const { id, audioIds } = request;
    return await addAudiosToPlaylistRequest(id, audioIds);
  };

  return useMutation(handler);
}
