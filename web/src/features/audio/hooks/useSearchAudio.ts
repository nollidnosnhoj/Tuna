import {
  usePagination,
  UsePaginationOptions,
  UsePaginationResultType,
} from "~/lib/hooks";
import { AudioData } from "../types";

type UseSearchAudioParams = {
  q?: string;
  tags?: string;
  size?: number;
};

export function useSearchAudio(
  page: number,
  params: UseSearchAudioParams = {},
  options: UsePaginationOptions<AudioData> = {}
): UsePaginationResultType<AudioData> {
  return usePagination<AudioData>("search/audios", params, page, options);
}
