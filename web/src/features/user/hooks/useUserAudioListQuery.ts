import { UseInfiniteQueryOptions } from "react-query";
import { Audio } from "~/features/audio/types";
import { fetch } from "~/lib/api";
import { useAuth } from "~/lib/hooks/useAuth";
import useInfiniteCursorPagination, {
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks/useInfiniteCursorPagination";
import { CursorPagedList } from "~/lib/types";

export function useUserAudioListQuery(
  username: string,
  size?: number,
  queryOptions?: UseInfiniteQueryOptions<CursorPagedList<Audio>>
): UseInfiniteCursorPaginationReturnType<Audio> {
  const params = {};
  if (typeof size !== "undefined" && size > 0) {
    Object.assign(params, { size });
  }
  const { accessToken } = useAuth();
  return useInfiniteCursorPagination<Audio>(
    `users/${username}/audios`,
    (cursor) => {
      if (!cursor) cursor = undefined;
      return fetch(
        `users/${username}/audios`,
        { ...params, cursor: cursor },
        { accessToken }
      );
    },
    params,
    queryOptions
  );
}
