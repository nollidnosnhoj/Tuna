import { useQuery, UseQueryOptions, UseQueryResult } from "react-query";
import { useAuth } from "~/features/auth/hooks/useAuth";
import { ErrorResponse } from "~/lib/types";
import { fetchAudioById } from "../services/mutations";
import { AudioDetailData } from "../types";

export function useGetAudio(
  id: string,
  options: UseQueryOptions<AudioDetailData, ErrorResponse> = {}
): UseQueryResult<AudioDetailData, ErrorResponse> {
  const { accessToken } = useAuth();
  return useQuery<AudioDetailData, ErrorResponse>(
    ["audios", id],
    () => fetchAudioById(id, { accessToken }),
    options
  );
}
