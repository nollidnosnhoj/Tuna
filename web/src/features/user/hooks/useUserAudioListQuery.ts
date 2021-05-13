import { useRouter } from "next/router";
import { Audio } from "~/features/audio/types";
import { fetch } from "~/lib/api";
import { useAuth } from "~/lib/hooks/useAuth";
import useInfiniteCursorPagination, {
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks/useInfiniteCursorPagination";

export function useUserAudioListQuery(
  username: string
): UseInfiniteCursorPaginationReturnType<Audio> {
  const { query } = useRouter();
  const { accessToken } = useAuth();
  return useInfiniteCursorPagination<Audio>(
    `users/${username}/audios`,
    (cursor) => {
      if (!cursor) cursor = undefined;
      return fetch(
        `users/${username}/audios`,
        { ...query, cursor: cursor },
        { accessToken }
      );
    },
    query
  );
}
