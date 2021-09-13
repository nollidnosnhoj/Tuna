/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { useCallback } from "react";
import { QueryKey } from "react-query";
import {
  useInfiniteCursorPagination,
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { CursorPagedList } from "~/lib/types";
import { PlaylistAudio, PlaylistId } from "../types";

export const GET_PLAYLIST_AUDIOS_KEY = (
  id: PlaylistId | undefined
): QueryKey => ["playlist_audios", id];

export function useGetPlaylistAudios(
  id: PlaylistId | undefined,
  options: UseInfiniteCursorPaginationOptions<PlaylistAudio> = {}
): UseInfiniteCursorPaginationReturnType<PlaylistAudio> {
  const fetcher = useCallback(
    async function getPlaylistAudiosRequest(
      offset = 0
    ): Promise<CursorPagedList<PlaylistAudio>> {
      const { data } = await request<CursorPagedList<PlaylistAudio>>({
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

  return useInfiniteCursorPagination<PlaylistAudio>(
    GET_PLAYLIST_AUDIOS_KEY(id),
    fetcher,
    {
      enabled: !!id,
      ...options,
    }
  );
}
