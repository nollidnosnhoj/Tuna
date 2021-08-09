import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import { ErrorResponse } from "~/lib/types";
import { getIdAndSlugFromSlug } from "~/utils";
import { getAudioRequest } from "../api";
import { AudioId, AudioView } from "../api/types";

export const GET_AUDIO_QUERY_KEY = (id: AudioId): QueryKey => ["audios", id];

export function useGetAudio(
  idSlug: string,
  options: UseQueryOptions<AudioView, ErrorResponse> = {}
): UseQueryResult<AudioView, ErrorResponse> {
  const [id] = getIdAndSlugFromSlug(idSlug);
  return useQuery<AudioView, ErrorResponse>(
    GET_AUDIO_QUERY_KEY(id),
    () => getAudioRequest(idSlug),
    options
  );
}
