import request from "~/lib/http";
import { ID } from "~/lib/types";

export async function checkDuplicatedAudiosRequest(
  id: ID,
  audioIds: ID[]
): Promise<ID[]> {
  const { data } = await request<ID[]>({
    method: "post",
    url: `playlists/${id}/audios/duplicate`,
    data: {
      audioIds,
    },
  });
  return data;
}
