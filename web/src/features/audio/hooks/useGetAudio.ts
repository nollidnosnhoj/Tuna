import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import { useAuth } from "~/features/auth/hooks/useAuth";
import api, { FetchRequestOptions } from "~/lib/api";
import { ErrorResponse } from "~/lib/types";
import { AudioDetailData } from "../types";

export const GET_AUDIO_QUERY_KEY = (id: string): QueryKey => ["audios", id];

export async function fetchAudioById(
  id: string,
  options: FetchRequestOptions = {}
): Promise<AudioDetailData> {
  const { data } = await api.get<AudioDetailData>(`audios/${id}`, undefined, {
    accessToken: options.accessToken,
  });
  return data;
}

export function useGetAudio(
  id: string,
  options: UseQueryOptions<AudioDetailData, ErrorResponse> = {}
): UseQueryResult<AudioDetailData, ErrorResponse> {
  const { accessToken } = useAuth();
  return useQuery<AudioDetailData, ErrorResponse>(
    GET_AUDIO_QUERY_KEY(id),
    () => fetchAudioById(id, { accessToken }),
    options
  );
}
