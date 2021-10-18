import { useCallback } from "react";
import {
  useQuery,
  useQueryClient,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import request from "~/lib/http";
import { AudioView, ErrorResponse } from "~/lib/types";
import {
  GET_AUDIO_QUERY_BY_SLUG_KEY,
  GET_AUDIO_QUERY_KEY,
} from "~/lib/hooks/api/keys";

type UseGetAudioQueryOptions = UseQueryOptions<AudioView, ErrorResponse>;

export function useGetAudio(
  audioSlug: string,
  options: UseGetAudioQueryOptions = {}
): UseQueryResult<AudioView, ErrorResponse> {
  const queryClient = useQueryClient();
  const fetcher = useCallback(async () => {
    const { data } = await request<AudioView>({
      method: "get",
      url: `audios/${audioSlug}`,
    });
    return data;
  }, [audioSlug]);

  return useQuery<AudioView, ErrorResponse>(
    GET_AUDIO_QUERY_BY_SLUG_KEY(audioSlug),
    fetcher,
    {
      ...options,
      onSuccess(data) {
        queryClient.setQueryData<AudioView>(GET_AUDIO_QUERY_KEY(data.id), data);
      },
    }
  );
}
