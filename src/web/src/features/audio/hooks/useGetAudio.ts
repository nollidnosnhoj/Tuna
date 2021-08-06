import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import { ErrorResponse } from "~/lib/types";
import { getAudioRequest } from "../api";
import { AudioData, AudioId } from "../types";

export const GET_AUDIO_QUERY_KEY = (id: AudioId): QueryKey => ["audios", id];

export function useGetAudio(
  id: AudioId,
  options: UseQueryOptions<AudioData, ErrorResponse> = {}
): UseQueryResult<AudioData, ErrorResponse> {
  return useQuery<AudioData, ErrorResponse>(
    GET_AUDIO_QUERY_KEY(id),
    () => getAudioRequest(id),
    options
  );
}
