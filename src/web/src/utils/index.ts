import { ID } from "~/lib/types";
import request from "~/lib/http";

export * from "./file";
export * from "./format";
export * from "./http";
export * from "./string";
export * from "./time";
export * from "./toast";

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
