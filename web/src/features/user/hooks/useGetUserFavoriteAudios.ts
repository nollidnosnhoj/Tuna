import { QueryKey } from "react-query";
import { AudioData } from "~/features/audio/types";
import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { fetchUserFavoriteAudios } from "../api";

type UseGetUserFavoriteAudiosParams = {
  size?: number;
};

export const GET_USER_FAVORITE_AUDIOS_QUERY_KEY = (
  username: string
): QueryKey => ["userFavoriteAudios", username];

export function useGetUserFavoriteAudios(
  username: string,
  params: UseGetUserFavoriteAudiosParams = {},
  options: UseInfinitePaginationOptions<AudioData> = {}
): UseInfinitePaginationReturnType<AudioData> {
  return useInfinitePagination(
    GET_USER_FAVORITE_AUDIOS_QUERY_KEY(username),
    (page) => fetchUserFavoriteAudios(username, page, params),
    options
  );
}
