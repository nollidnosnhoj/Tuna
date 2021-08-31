import { useCallback } from "react";
import {
  QueryKey,
  useQuery,
  useQueryClient,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import request from "~/lib/http";
import { ErrorResponse } from "~/lib/types";
import { AudioView } from "../types";

export const GET_AUDIO_QUERY_BYSLUG_KEY = (slug: string): QueryKey => [
  "audios",
  slug,
];
export const GET_AUDIO_QUERY_KEY = (id: number): QueryKey => ["audios", id];

type UseGetAudioQueryOptions = UseQueryOptions<AudioView, ErrorResponse>;

export function useGetAudio(
  slug: string,
  options: UseGetAudioQueryOptions = {}
): UseQueryResult<AudioView, ErrorResponse> {
  const queryClient = useQueryClient();
  const fetcher = useCallback(async () => {
    const { data } = await request<AudioView>({
      method: "get",
      url: `audios/${slug}`,
    });
    return data;
  }, [slug]);

  return useQuery<AudioView, ErrorResponse>(
    GET_AUDIO_QUERY_BYSLUG_KEY(slug),
    fetcher,
    {
      ...options,
      onSuccess(data) {
        queryClient.setQueryData<AudioView>(GET_AUDIO_QUERY_KEY(data.id), data);
      },
    }
  );
}
