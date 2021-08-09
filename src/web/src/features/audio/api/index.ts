import { GetServerSidePropsContext } from "next";
import request from "~/lib/http";
import { CursorPagedList, ImageUploadResponse, PagedList } from "~/lib/types";
import { AudioView, AudioId, AudioRequest, CreateAudioRequest } from "./types";

export async function getAudiosRequest(
  cursor?: string,
  params: Record<string, string | boolean | number> = {}
): Promise<CursorPagedList<AudioView>> {
  const { data } = await request<CursorPagedList<AudioView>>({
    method: "get",
    url: "audios",
    params: { ...params, cursor: cursor },
  });
  return data;
}

export async function getAudioRequest(
  idSlug: string,
  ctx?: GetServerSidePropsContext
): Promise<AudioView> {
  const { res, req } = ctx ?? {};
  const { data } = await request<AudioView>({
    method: "get",
    url: `audios/${idSlug}`,
    req,
    res,
  });
  return data;
}

export async function getAudioFeedRequest(
  pageNumber: number
): Promise<PagedList<AudioView>> {
  const { data } = await request<PagedList<AudioView>>({
    method: "get",
    url: "me/audios/feed",
    params: { page: pageNumber },
  });
  return data;
}

export type SearchAudioParams = {
  tags?: string;
  size?: number;
};

export async function searchAudiosRequest(
  searchTerm: string,
  pageNumber: number,
  params?: SearchAudioParams
): Promise<PagedList<AudioView>> {
  const { data } = await request<PagedList<AudioView>>({
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

export async function createAudioRequest(
  input: CreateAudioRequest
): Promise<AudioView> {
  const { data } = await request<AudioView>({
    url: "audios",
    method: "post",
    data: input,
  });
  return data;
}

export async function updateAudioDetailsRequest(
  audioId: AudioId,
  input: AudioRequest
): Promise<AudioView> {
  const { data } = await request<AudioView>({
    url: `audios/${audioId}`,
    method: "put",
    data: input,
  });
  return data;
}

export async function removeAudioRequest(audioId: AudioId): Promise<void> {
  await request({
    method: "delete",
    url: `audios/${audioId}`,
  });
}

export async function uploadAudioPictureRequest(
  audioId: AudioId,
  imageData: string
): Promise<ImageUploadResponse> {
  const { data } = await request<ImageUploadResponse>({
    method: "patch",
    url: `audios/${audioId}/picture`,
    data: {
      data: imageData,
    },
  });
  return data;
}

export async function checkIfUserFavoritedAudioRequest(
  audioId: AudioId
): Promise<boolean> {
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

export async function favoriteAnAudioRequest(
  audioId: AudioId
): Promise<boolean> {
  await request({
    method: "PUT",
    url: `me/favorites/audios/${audioId}`,
  });
  return true;
}

export async function unfavoriteAnAudioRequest(
  audioId: AudioId
): Promise<boolean> {
  await request({
    method: "DELETE",
    url: `me/favorites/audios/${audioId}`,
  });
  return true;
}
