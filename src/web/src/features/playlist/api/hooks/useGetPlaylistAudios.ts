/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { useCallback } from "react";
import { QueryKey } from "react-query";
import { AudioView } from "~/features/audio/api/types";
import {
  useInfiniteCursorPagination,
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { OffsetPagedList } from "~/lib/types";
import { PlaylistId } from "../types";

export const GET_PLAYLIST_AUDIOS_KEY = (
  id: PlaylistId | undefined
): QueryKey => ["playlist_audios", id];

export function useGetPlaylistAudios(
  id: PlaylistId | undefined,
  options: UseInfiniteCursorPaginationOptions<AudioView> = {}
): UseInfiniteCursorPaginationReturnType<AudioView> {
  const fetcher = useCallback(
    async function getPlaylistAudiosRequest(
      offset = 0
    ): Promise<OffsetPagedList<AudioView>> {
      const { data } = await request<OffsetPagedList<AudioView>>({
        method: "GET",
        url: `playlists/${id}/audios`,
        params: {
          offset,
        },
      });
      return data;
    },
    [id]
  );

  return useInfiniteCursorPagination<AudioView>(
    GET_PLAYLIST_AUDIOS_KEY(id),
    fetcher,
    {
      enabled: !!id,
      ...options,
    }
  );
}
