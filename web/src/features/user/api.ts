import { GetServerSidePropsContext } from "next";
import request from "~/lib/http";
import { ImageUploadResponse, PagedList } from "~/lib/types";
import { AudioData } from "../audio/types";
import { CurrentUser, Profile } from "./types";

export async function getCurrentUser(): Promise<CurrentUser> {
  const response = await request<CurrentUser>({
    method: "get",
    url: "me",
  });
  return response.data;
}

export async function fetchProfile(
  username: string,
  ctx?: GetServerSidePropsContext
): Promise<Profile> {
  const { req, res } = ctx ?? {};
  const { data } = await request<Profile>({
    method: "get",
    url: `users/${username}`,
    req,
    res,
  });

  return data;
}

export async function fetchUserAudios(
  username: string,
  page: number,
  params?: Record<string, string | number | boolean>
): Promise<PagedList<AudioData>> {
  const { data } = await request<PagedList<AudioData>>({
    method: "get",
    url: `users/${username}/audios`,
    params: {
      ...params,
      page,
    },
  });
  return data;
}

export async function fetchUserFavoriteAudios(
  username: string,
  page?: number,
  params?: Record<string, string | number | boolean>
): Promise<PagedList<AudioData>> {
  const { data } = await request<PagedList<AudioData>>({
    method: "get",
    url: `users/${username}/favorite/audios`,
    params: {
      ...params,
      page,
    },
  });
  return data;
}

export async function uploadUserPictureHandler(
  imageData: string
): Promise<ImageUploadResponse> {
  const { data } = await request<ImageUploadResponse>({
    method: "patch",
    url: "me/picture",
    data: {
      data: imageData,
    },
  });
  return data;
}

export async function isFollowingHandler(userId: string): Promise<boolean> {
  try {
    const res = await request({
      method: "head",
      url: `me/following/${userId}`,
      validateStatus: (status) => status === 404 || status < 400,
    });
    return res.status !== 404;
  } catch (err) {
    return false;
  }
}

export async function followUserHandler(userId: string): Promise<boolean> {
  await request({
    method: "PUT",
    url: `me/followings/${userId}`,
  });
  return true;
}

export async function unFollowUserHandler(userId: string): Promise<boolean> {
  await request({
    method: "DELETE",
    url: `me/followings/${userId}`,
  });
  return false;
}
