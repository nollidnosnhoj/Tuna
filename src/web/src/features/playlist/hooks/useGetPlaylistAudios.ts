import { QueryKey } from "react-query";
import { AudioData } from "~/features/audio/types";
import {
  useInfinitePagination,
  UseInfinitePaginationOptions,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { getPlaylistAudiosRequest } from "../api";

export const GET_PLAYLIST_AUDIOS_KEY = (id: string): QueryKey => [
  "playlist_audios",
  id,
];

export function useGetPlaylistAudios(
  id: string,
  options: UseInfinitePaginationOptions<AudioData> = {}
): UseInfinitePaginationReturnType<AudioData> {
  return useInfinitePagination<AudioData>(
    GET_PLAYLIST_AUDIOS_KEY(id),
    (page) => getPlaylistAudiosRequest(id, page),
    options
  );
}
