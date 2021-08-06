/* eslint-disable @typescript-eslint/explicit-function-return-type */
/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
import { useMutation } from "react-query";
import { addAudiosToPlaylistRequest } from "../api";

export function useAddAudiosToPlaylist() {
  const handler = async (request: {
    id: string;
    audioIds: string[];
  }): Promise<void> => {
    const { id, audioIds } = request;
    return await addAudiosToPlaylistRequest(id, audioIds);
  };

  return useMutation(handler);
}
