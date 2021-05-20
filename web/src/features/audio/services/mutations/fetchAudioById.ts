import api, { FetchRequestOptions } from "~/lib/api";
import { AudioDetailData } from "~/features/audio/types";

export async function fetchAudioById(
  id: string,
  options: FetchRequestOptions = {}
): Promise<AudioDetailData> {
  const { data } = await api.get<AudioDetailData>(`audios/${id}`, undefined, {
    accessToken: options.accessToken,
  });
  return data;
}
