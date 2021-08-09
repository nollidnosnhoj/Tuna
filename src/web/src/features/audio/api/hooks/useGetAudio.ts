import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import { ErrorResponse } from "~/lib/types";
import { getIdAndSlugFromSlug } from "~/utils";
import { getAudioRequest } from "..";
import { AudioId, AudioView } from "../types";

export const GET_AUDIO_QUERY_KEY = (id: AudioId): QueryKey => ["audios", id];

type UseGetAudioQueryOptions = UseQueryOptions<AudioView, ErrorResponse> & {
  secret?: string;
};

export function useGetAudio(
  idSlug: string,
  options: UseGetAudioQueryOptions = {}
): UseQueryResult<AudioView, ErrorResponse> {
  const [id] = getIdAndSlugFromSlug(idSlug);
  const { secret, ...queryOptions } = options;
  return useQuery<AudioView, ErrorResponse>(
    GET_AUDIO_QUERY_KEY(id),
    () => getAudioRequest(idSlug, secret),
    queryOptions
  );
}
