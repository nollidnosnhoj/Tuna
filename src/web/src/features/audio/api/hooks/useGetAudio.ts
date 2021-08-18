import { useCallback } from "react";
import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import request from "~/lib/http";
import { ErrorResponse, IdSlug } from "~/lib/types";
import { getIdAndSlugFromSlug } from "~/utils";
import { AudioId, AudioView } from "../types";

export const GET_AUDIO_QUERY_KEY = (id: AudioId): QueryKey => ["audios", id];

type UseGetAudioQueryOptions = UseQueryOptions<AudioView, ErrorResponse> & {
  secret?: string;
};

export function useGetAudio(
  idSlug: IdSlug,
  options: UseGetAudioQueryOptions = {}
): UseQueryResult<AudioView, ErrorResponse> {
  const [id] = getIdAndSlugFromSlug(idSlug);
  const { secret, ...queryOptions } = options;

  const fetcher = useCallback(async () => {
    const { data } = await request<AudioView>({
      method: "get",
      url: `audios/${idSlug}`,
      params: {
        secret: secret || undefined,
      },
    });
    return data;
  }, [idSlug, secret]);

  return useQuery<AudioView, ErrorResponse>(
    GET_AUDIO_QUERY_KEY(id),
    fetcher,
    queryOptions
  );
}
