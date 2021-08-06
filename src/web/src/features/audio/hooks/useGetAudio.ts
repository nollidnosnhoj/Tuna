import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import { ErrorResponse } from "~/lib/types";
import { getAudioRequest } from "../api";
import { AudioView, AudioId } from "../api/types";

export const GET_AUDIO_QUERY_KEY = (id: AudioId): QueryKey => ["audios", id];

export function useGetAudio(
  id: AudioId,
  options: UseQueryOptions<AudioView, ErrorResponse> = {}
): UseQueryResult<AudioView, ErrorResponse> {
  return useQuery<AudioView, ErrorResponse>(
    GET_AUDIO_QUERY_KEY(id),
    () => getAudioRequest(id),
    options
  );
}
