import { useRouter } from "next/router";
import { UseInfiniteQueryOptions } from "react-query";
import { Audio } from "~/features/audio/types";
import { fetchPages } from "~/lib/api";
import { useAuth } from "~/lib/hooks/useAuth";
import useInfinitePagination, {
  UseInfinitePaginationReturnType,
} from "~/lib/hooks/useInfinitePagination";
import { PagedList } from "~/lib/types";

export function useAudioFeedQuery(
  size?: number,
  queryOptions?: UseInfiniteQueryOptions<PagedList<Audio>>
): UseInfinitePaginationReturnType<Audio> {
  const { accessToken } = useAuth();
  const { query } = useRouter();
  const params = { ...query };
  if (typeof size !== "undefined" && size > 0) {
    Object.assign(params, { size });
  }
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const { page: _, ...queryParam } = params;
  return useInfinitePagination<Audio>(
    "me/feed",
    (page) => fetchPages("me/feed", queryParam, page, { accessToken }),
    queryParam,
    queryOptions
  );
}
