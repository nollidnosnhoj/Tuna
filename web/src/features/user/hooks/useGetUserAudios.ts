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

type UseGetUserAudiosParams = {
  size?: number;
};

export const GET_USER_AUDIOS_QUERY_KEY = (username: string): QueryKey => [
  "userAudios",
  username,
];

export const fetchUserAudios = async (
  username: string,
  page: number,
  params?: Record<string, string | number | boolean>,
  accessToken?: string
): Promise<PagedList<AudioData>> => {
  return fetchPages<AudioData>(`users/${username}/audios`, params, page, {
    accessToken,
  });
};

export function useGetUserAudios(
  username: string,
  params: UseGetUserAudiosParams = {},
  options: UseInfinitePaginationOptions<AudioData>
): UseInfinitePaginationReturnType<AudioData> {
  const { accessToken } = useAuth();
  return useInfinitePagination(
    GET_USER_AUDIOS_QUERY_KEY(username),
    (page) => fetchUserAudios(username, page, params, accessToken),
    options
  );
}
