import { AudioData } from "~/features/audio/types";
import {
  useInfinitePagination,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { getAuthenticatedUserFavoriteAudios } from "../api";

export const GET_YOUR_FAV_AUDIOS_KEY = "your_fav_audios";

export function useYourFavoriteAudios(): UseInfinitePaginationReturnType<AudioData> {
  return useInfinitePagination(GET_YOUR_FAV_AUDIOS_KEY, (page) =>
    getAuthenticatedUserFavoriteAudios(page)
  );
}
