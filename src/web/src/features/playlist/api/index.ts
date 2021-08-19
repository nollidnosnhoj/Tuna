import { AudioId } from "~/features/audio/api/types";
import request from "~/lib/http";

export async function checkDuplicatedAudiosRequest(
  id: number,
  audioIds: AudioId[]
): Promise<AudioId[]> {
  const { data } = await request<AudioId[]>({
    method: "post",
    url: `playlists/${id}/audios/duplicate`,
    data: {
      audioIds,
    },
  });
  return data;
}
