/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { QueryKey } from "react-query";
import { AudioView } from "~/features/audio/api/types";
import {
  useInfiniteCursorPagination,
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks";
import { getPlaylistAudiosRequest } from "..";
import { PlaylistId } from "../types";

export const GET_PLAYLIST_AUDIOS_KEY = (
  id: PlaylistId | undefined
): QueryKey => ["playlist_audios", id];

export function useGetPlaylistAudios(
  id: PlaylistId | undefined,
  options: UseInfiniteCursorPaginationOptions<AudioView> = {}
): UseInfiniteCursorPaginationReturnType<AudioView> {
  return useInfiniteCursorPagination<AudioView>(
    GET_PLAYLIST_AUDIOS_KEY(id),
    (cursor) => getPlaylistAudiosRequest(id!, cursor),
    {
      enabled: !!id,
      ...options,
    }
  );
}
