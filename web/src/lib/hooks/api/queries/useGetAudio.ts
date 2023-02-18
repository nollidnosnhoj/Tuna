import { useCallback } from "react";
import {
  useQuery,
  useQueryClient,
  UseQueryOptions,
  UseQueryResult,
} from "@tanstack/react-query";
import request from "~/lib/http";
import { Audio, ErrorResponse } from "~/lib/types";
import {
  GET_AUDIO_QUERY_BY_SLUG_KEY,
  GET_AUDIO_QUERY_KEY,
} from "~/lib/hooks/api/keys";

type UseGetAudioQueryOptions = UseQueryOptions<Audio, ErrorResponse>;

export function useGetAudio(
  audioSlug: string,
  options: UseGetAudioQueryOptions = {}
): UseQueryResult<Audio, ErrorResponse> {
  const queryClient = useQueryClient();
  const fetcher = useCallback(async () => {
    const { data } = await request<Audio>({
      method: "get",
      url: `audios/${audioSlug}`,
    });
    return data;
  }, [audioSlug]);

  return useQuery<Audio, ErrorResponse>(
    GET_AUDIO_QUERY_BY_SLUG_KEY(audioSlug),
    fetcher,
    {
      ...options,
      onSuccess(data) {
        queryClient.setQueryData<Audio>(GET_AUDIO_QUERY_KEY(data.id), data);
      },
    }
  );
}
