import { AudioView } from "~/features/audio/api/types";
import {
  useInfinitePagination,
  UseInfinitePaginationReturnType,
} from "~/lib/hooks";
import { getCurrentUserAudiosRequest } from "..";

export const GET_YOUR_AUDIOS_KEY = "your_audios";

export function useYourAudios(): UseInfinitePaginationReturnType<AudioView> {
  return useInfinitePagination(GET_YOUR_AUDIOS_KEY, (page) =>
    getCurrentUserAudiosRequest(page)
  );
}
