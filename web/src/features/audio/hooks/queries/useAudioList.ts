import { UseInfiniteQueryOptions } from "react-query";
import { CursorPagedList } from "~/lib/types";
import { Audio } from "~/features/audio/types";
import { fetch } from "~/lib/api";
import useInfiniteCursorPagination, {
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks/useInfiniteCursorPagination";
import { useAuth } from "~/lib/hooks/useAuth";

export default function useGetAudioListInfinite(
  params: Record<string, unknown> = {},
  size = 15,
  options?: UseInfiniteQueryOptions<CursorPagedList<Audio>>
): UseInfiniteCursorPaginationReturnType<Audio> {
  const { accessToken } = useAuth();
  Object.assign(params, { size });
  return useInfiniteCursorPagination<Audio>(
    "audios",
    (cursor) => {
      if (!cursor) cursor = undefined;
      return fetch("audios", { ...params, cursor: cursor }, { accessToken });
    },
    params,
    options
  );
}
