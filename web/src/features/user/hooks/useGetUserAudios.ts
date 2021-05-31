import { QueryKey } from "react-query";
import { AudioData } from "~/features/audio/types";
import { useAuth } from "~/features/auth/hooks";
import {
  fetchCursorList,
  useInfiniteCursorPagination,
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks";
import { CursorPagedList } from "~/lib/types";

type UseGetUserAudiosParams = {
  size?: number;
};

export const GET_USER_AUDIOS_QUERY_KEY = (username: string): QueryKey => [
  "userAudios",
  username,
];

export const fetchUserAudios = async (
  username: string,
  cursor?: string,
  params?: Record<string, string | number | boolean>,
  accessToken?: string
): Promise<CursorPagedList<AudioData>> => {
  return fetchCursorList<AudioData>(
    `users/${username}/audios`,
    cursor,
    params,
    {
      accessToken,
    }
  );
};

export function useGetUserAudios(
  username: string,
  params: UseGetUserAudiosParams = {},
  options: UseInfiniteCursorPaginationOptions<AudioData>
): UseInfiniteCursorPaginationReturnType<AudioData> {
  const { accessToken } = useAuth();
  return useInfiniteCursorPagination(
    GET_USER_AUDIOS_QUERY_KEY(username),
    (cursor) => fetchUserAudios(username, cursor, params, accessToken),
    options
  );
}
