/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { useCallback } from "react";
import { QueryKey } from "react-query";
import {
  useInfiniteCursorPagination,
  UseInfiniteCursorPaginationOptions,
  UseInfiniteCursorPaginationReturnType,
} from "~/lib/hooks";
import request from "~/lib/http";
import { CursorPagedList, ID, PlaylistAudio } from "~/lib/types";

export const GET_PLAYLIST_AUDIOS_KEY = (
  playlistId: ID | undefined
): QueryKey => ["playlist_audios", playlistId];

export function useGetPlaylistAudios(
  playlistId: ID | undefined,
  options: UseInfiniteCursorPaginationOptions<PlaylistAudio> = {}
): UseInfiniteCursorPaginationReturnType<PlaylistAudio> {
  const fetcher = useCallback(
    async function getPlaylistAudiosRequest(
      offset = 0
    ): Promise<CursorPagedList<PlaylistAudio>> {
      const { data } = await request<CursorPagedList<PlaylistAudio>>({
        method: "GET",
        url: `playlists/${playlistId}/audios`,
        params: {
          offset,
        },
      });
      return data;
    },
    [playlistId]
  );

  return useInfiniteCursorPagination<PlaylistAudio>(
    GET_PLAYLIST_AUDIOS_KEY(playlistId),
    fetcher,
    {
      enabled: !!playlistId,
      ...options,
    }
  );
}
