import { QueryKey } from "react-query";
import { AudioView } from "~/features/audio/api/types";
import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { getUserFavoriteAudiosRequest } from "..";

type UseGetUserFavoriteAudiosParams = {
  size?: number;
};

export const GET_USER_FAVORITE_AUDIOS_QUERY_KEY = (
  username: string
): QueryKey => ["userFavoriteAudios", username];

export function useGetUserFavoriteAudios(
  username: string,
  params: UseGetUserFavoriteAudiosParams = {},
  options: UseInfinitePaginationOptions<AudioView> = {}
): UseInfinitePaginationReturnType<AudioView> {
  return useInfinitePagination(
    GET_USER_FAVORITE_AUDIOS_QUERY_KEY(username),
    (page) => getUserFavoriteAudiosRequest(username, page, params),
    {
      ...options,
      enabled: !!username && (options.enabled ?? true),
    }
  );
}
