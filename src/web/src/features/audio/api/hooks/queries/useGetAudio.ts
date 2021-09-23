import { useCallback } from "react";
import {
  QueryKey,
  useQuery,
  useQueryClient,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import request from "~/lib/http";
import { ErrorResponse, ID } from "~/lib/types";
import { AudioView } from "../../types";

export const GET_AUDIO_QUERY_BYSLUG_KEY = (audioSlug: string): QueryKey => [
  "audios",
  audioSlug,
];

export const GET_AUDIO_QUERY_KEY = (audioId: ID): QueryKey => [
  "audios",
  audioId,
];

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
    GET_AUDIO_QUERY_BYSLUG_KEY(audioSlug),
    fetcher,
    {
      ...options,
      onSuccess(data) {
        queryClient.setQueryData<AudioView>(GET_AUDIO_QUERY_KEY(data.id), data);
      },
    }
  );
}
