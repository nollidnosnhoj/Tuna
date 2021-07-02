import { GetServerSidePropsContext } from "next";
import request from "~/lib/http";
import { CursorPagedList, PagedList } from "~/lib/types";
import {
  AudioData,
  AudioDetailData,
  AudioRequest,
  CreateAudioRequest,
} from "./types";

export async function fetchAudiosHandler(
  cursor?: number,
  params: Record<string, string | boolean | number> = {}
): Promise<CursorPagedList<AudioData>> {
  const { data } = await request<CursorPagedList<AudioData>>({
    method: "get",
    url: "audios",
    params: { ...params, cursor: cursor },
  });
  return data;
}

export async function fetchAudioHandler(
  id: number,
  secret?: string,
  ctx?: GetServerSidePropsContext
): Promise<AudioDetailData> {
  const { res, req } = ctx ?? {};
  const { data } = await request<AudioDetailData>({
    method: "get",
    url: `audios/${id}`,
    params: {
      ...(secret && { secret: secret }),
    },
    req,
    res,
  });
  return data;
}

export async function fetchAudioFeedHandler(
  pageNumber: number
): Promise<PagedList<AudioData>> {
  const { data } = await request<PagedList<AudioData>>({
    method: "get",
    url: "me/feed",
    params: { page: pageNumber },
  });
  return data;
}

export type SearchAudioParams = {
  tags?: string;
  size?: number;
};

export async function searchAudiosHandler(
  searchTerm: string,
  pageNumber: number,
  params?: SearchAudioParams
): Promise<PagedList<AudioData>> {
  const { data } = await request<PagedList<AudioData>>({
    method: "get",
    url: "search/audios",
    params: {
      ...params,
      q: searchTerm,
      page: pageNumber,
    },
  });
  return data;
}

export async function createAudioHandler(
  input: CreateAudioRequest
): Promise<AudioDetailData> {
  const { data } = await request<AudioDetailData>({
    url: "audios",
    method: "post",
    data: input,
  });
  return data;
}

export async function editAudioHandler(
  audioId: number,
  input: AudioRequest
): Promise<AudioDetailData> {
  const { data } = await request<AudioDetailData>({
    url: `audios/${audioId}`,
    method: "put",
    data: input,
  });
  return data;
}

export async function removeAudioHandler(audioId: number): Promise<void> {
  await request({
    method: "delete",
    url: `audios/${audioId}`,
  });
}

export async function uploadAudioPictureHandler(
  audioId: number,
  imageData: string
): Promise<AudioDetailData> {
  const { data } = await request<AudioDetailData>({
    method: "patch",
    url: `audios/${audioId}/picture`,
    data: {
      data: imageData,
    },
  });
  return data;
}

export async function isFavoriteHandler(audioId: number): Promise<boolean> {
  try {
    const res = await request({
      method: "head",
      url: `me/favorites/audios/${audioId}`,
      validateStatus: (status) => {
        return status === 404 || status < 400;
      },
    });

    return res.status !== 404;
  } catch (err) {
    return false;
  }
}

export async function favoriteAudioHandler(audioId: number): Promise<boolean> {
  await request({
    method: "PUT",
    url: `me/favorites/audios/${audioId}`,
  });
  return true;
}

export async function unFavoriteAudioHandler(
  audioId: number
): Promise<boolean> {
  await request({
    method: "DELETE",
    url: `me/favorites/audios/${audioId}`,
  });
  return true;
}

export async function resetAudioSecret(audioId: number): Promise<string> {
  const { data } = await request<{ secret: string }>({
    method: "PATCH",
    url: `audios/${audioId}/reset-private-key`,
  });
  return data.secret;
}
