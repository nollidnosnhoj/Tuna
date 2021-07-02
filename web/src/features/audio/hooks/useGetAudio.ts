import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import { ErrorResponse } from "~/lib/types";
import { fetchAudioHandler } from "../api";
import { AudioDetailData, AudioId } from "../types";

export const GET_AUDIO_QUERY_KEY = (id: AudioId): QueryKey => ["audios", id];

export function useGetAudio(
  id: AudioId,
  secret?: string,
  options: UseQueryOptions<AudioDetailData, ErrorResponse> = {}
): UseQueryResult<AudioDetailData, ErrorResponse> {
  return useQuery<AudioDetailData, ErrorResponse>(
    GET_AUDIO_QUERY_KEY(id),
    () => fetchAudioHandler(id, secret),
    options
  );
}
