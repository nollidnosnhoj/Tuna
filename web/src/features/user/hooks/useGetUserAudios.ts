import { AudioData } from "~/features/audio/types";
import {
  useInfiniteCursorPagination,
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks";

type UseGetUserAudiosParams = {
  size?: number;
};

export function useGetUserAudios(
  username: string,
  params: UseGetUserAudiosParams = {},
  options: UseInfiniteCursorPaginationOptions<AudioData>
): UseInfiniteCursorPaginationReturnType<AudioData> {
  return useInfiniteCursorPagination(
    `users/${username}/audios`,
    params,
    options
  );
}
