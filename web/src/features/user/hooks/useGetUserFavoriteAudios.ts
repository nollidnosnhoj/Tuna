import { QueryKey } from "react-query";
import { AudioData } from "~/features/audio/types";
import { useAuth } from "~/features/auth/hooks";
import { fetchPages } from "~/lib/api";
import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { PagedList } from "~/lib/types";

type UseGetUserFavoriteAudiosParams = {
  size?: number;
};

export const GET_USER_FAVORITE_AUDIOS_QUERY_KEY = (
  username: string
): QueryKey => ["userFavoriteAudios", username];

export const fetchUserFavoriteAudios = async (
  username: string,
  page?: number,
  params?: Record<string, string | number | boolean>,
  accessToken?: string
): Promise<PagedList<AudioData>> => {
  return fetchPages<AudioData>(
    `users/${username}/favorite/audios`,
    params,
    page,
    {
      accessToken,
    }
  );
};

export function useGetUserFavoriteAudios(
  username: string,
  params: UseGetUserFavoriteAudiosParams = {},
  options: UseInfinitePaginationOptions<AudioData>
): UseInfinitePaginationReturnType<AudioData> {
  const { accessToken } = useAuth();
  return useInfinitePagination(
    GET_USER_FAVORITE_AUDIOS_QUERY_KEY(username),
    (page) => fetchUserFavoriteAudios(username, page, params, accessToken),
    options
  );
}
