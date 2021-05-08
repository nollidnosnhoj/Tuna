import api, { FetchAudioOptions } from "~/lib/api";
import { AudioDetail } from "~/features/audio/types";

export async function fetchAudioById(
  id: string,
  options: FetchAudioOptions = {}
): Promise<AudioDetail> {
  const { data } = await api.get<AudioDetail>(`audios/${id}`, undefined, {
    accessToken: options.accessToken,
  });
  return data;
}
