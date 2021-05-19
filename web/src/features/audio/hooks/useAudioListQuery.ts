import { useRouter } from "next/router";
import { UseInfiniteQueryOptions } from "react-query";
import { Audio } from "~/features/audio/types";
import { fetch } from "~/lib/api";
import { useAuth } from "~/lib/hooks/useAuth";
import useInfiniteCursorPagination, {
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks/useInfiniteCursorPagination";
import { CursorPagedList } from "~/lib/types";

export function useAudioListQuery(
  size?: number,
  queryOptions?: UseInfiniteQueryOptions<CursorPagedList<Audio>>
): UseInfiniteCursorPaginationReturnType<Audio> {
  const { query } = useRouter();
  const params = { ...query };
  if (typeof size !== "undefined" && size > 0) {
    Object.assign(params, { size });
  }
  const { accessToken } = useAuth();
  return useInfiniteCursorPagination<Audio>(
    "audios",
    (cursor) => {
      if (!cursor) cursor = undefined;
      return fetch("audios", { ...params, cursor: cursor }, { accessToken });
    },
    params,
    queryOptions
  );
}
