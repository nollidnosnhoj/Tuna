import { QueryKey } from "react-query";
import { AudioView } from "~/features/audio/api/types";
import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { getUserAudiosRequest } from "..";

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
  options: UseInfinitePaginationOptions<AudioView> = {}
): UseInfinitePaginationReturnType<AudioView> {
  return useInfinitePagination(
    GET_USER_AUDIOS_QUERY_KEY(username),
    (offset) => getUserAudiosRequest(username, offset, params),
    {
      ...options,
      enabled: !!username && (options.enabled ?? true),
    }
  );
}
