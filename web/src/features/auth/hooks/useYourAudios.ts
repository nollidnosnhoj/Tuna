import { AudioData } from "~/features/audio/types";
import {
  useInfinitePagination,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { getCurrentUserAudiosRequest } from "../api";

export const GET_YOUR_AUDIOS_KEY = "your_audios";

export function useYourAudios(): UseInfinitePaginationReturnType<AudioData> {
  return useInfinitePagination(GET_YOUR_AUDIOS_KEY, (page) =>
    getCurrentUserAudiosRequest(page)
  );
}
