import { useCallback } from "react";
import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { Audio, OffsetPagedList } from "~/lib/types";
import { GET_USER_AUDIOS_QUERY_KEY } from "~/lib/hooks/api/keys";

type UseGetUserAudiosParams = {
  size?: number;
};

export function useGetUserAudios(
  username: string,
  params: UseGetUserAudiosParams = {},
  options: UseInfinitePaginationOptions<Audio> = {}
): UseInfinitePaginationReturnType<Audio> {
  const fetcher = useCallback(
    async (offset: number) => {
      const { data } = await request<OffsetPagedList<Audio>>({
        method: "get",
        url: `users/${username}/audios`,
        params: {
          ...params,
          offset,
        },
      });
      return data;
    },
    [username, params]
  );

  return useInfinitePagination(GET_USER_AUDIOS_QUERY_KEY(username), fetcher, {
    ...options,
    enabled: !!username && (options.enabled ?? true),
  });
}
