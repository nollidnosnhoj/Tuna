import { AudioData } from "~/features/audio/types";
import {
  useInfinitePagination,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { getCurrentUserFavoriteAudiosRequest } from "../api";

export const GET_YOUR_FAV_AUDIOS_KEY = "your_fav_audios";

export function useYourFavoriteAudios(): UseInfinitePaginationReturnType<AudioData> {
  return useInfinitePagination(GET_YOUR_FAV_AUDIOS_KEY, (page) =>
    getCurrentUserFavoriteAudiosRequest(page)
  );
}
