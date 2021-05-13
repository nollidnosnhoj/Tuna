import { useRouter } from "next/router";
import { Audio } from "~/features/audio/types";
import { fetchPages } from "~/lib/api";
import { useAuth } from "~/lib/hooks/useAuth";
import useInfinitePagination from "~/lib/hooks/useInfinitePagination";

export function useAudioFeedQuery() {
  const { accessToken } = useAuth();
  const { query } = useRouter();
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const { page: _, ...queryParam } = query;
  return useInfinitePagination<Audio>(
    "me/feed",
    (page) => fetchPages("me/feed", queryParam, page, { accessToken }),
    queryParam
  );
}
