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

type UseGetAudioQueryOptions = UseQueryOptions<AudioView, ErrorResponse> & {
  secret?: string;
};

export function useGetAudio(
  slug: string,
  options: UseGetAudioQueryOptions = {}
): UseQueryResult<AudioView, ErrorResponse> {
  const { secret, ...queryOptions } = options;
  const queryClient = useQueryClient();
  const fetcher = useCallback(async () => {
    const { data } = await request<AudioView>({
      method: "get",
      url: `audios/${slug}`,
      params: {
        secret: secret || undefined,
      },
    });
    return data;
  }, [slug, secret]);

  return useQuery<AudioView, ErrorResponse>(
    GET_AUDIO_QUERY_BYSLUG_KEY(slug),
    fetcher,
    {
      ...queryOptions,
      onSuccess(data) {
        queryClient.setQueryData<AudioView>(GET_AUDIO_QUERY_KEY(data.id), data);
      },
    }
  );
}
