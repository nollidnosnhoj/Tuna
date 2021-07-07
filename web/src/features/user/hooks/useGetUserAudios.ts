import { QueryKey } from "react-query";
import { AudioData } from "~/features/audio/types";
import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { fetchUserAudios } from "../api";

type UseGetUserAudiosParams = {
  size?: number;
};

export const GET_USER_AUDIOS_QUERY_KEY = (username: string): QueryKey => [
  "userAudios",
  username,
];

export function useGetUserAudios(
  username: string,
  params: UseGetUserAudiosParams = {},
  options: UseInfinitePaginationOptions<AudioData> = {}
): UseInfinitePaginationReturnType<AudioData> {
  return useInfinitePagination(
    GET_USER_AUDIOS_QUERY_KEY(username),
    (page) => fetchUserAudios(username, page, params),
    {
      ...options,
      enabled: !!username && (options.enabled ?? true),
    }
  );
}
