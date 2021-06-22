import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import { ErrorResponse } from "~/lib/types";
import { fetchAudioHandler } from "../api";
import { AudioDetailData } from "../types";

export const GET_AUDIO_QUERY_KEY = (id: number): QueryKey => ["audios", id];

export function useGetAudio(
  id: number,
  options: UseQueryOptions<AudioDetailData, ErrorResponse> = {}
): UseQueryResult<AudioDetailData, ErrorResponse> {
  return useQuery<AudioDetailData, ErrorResponse>(
    GET_AUDIO_QUERY_KEY(id),
    () => fetchAudioHandler(id),
    options
  );
}
