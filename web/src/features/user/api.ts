import { GetServerSidePropsContext } from "next";
import request from "~/lib/http";
import { PagedList } from "~/lib/types";
import { AudioData } from "../audio/types";
import { Profile } from "./types";

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
): Promise<Profile> {
  const { data } = await request<Profile>({
    method: "patch",
    url: "me/picture",
    data: {
      data: imageData,
    },
  });
  return data;
}

export async function isFollowingHandler(username: string): Promise<boolean> {
  try {
    const res = await request({
      method: "head",
      url: `me/following/${username}`,
      validateStatus: (status) => status === 404 || status < 400,
    });
    return res.status !== 404;
  } catch (err) {
    return false;
  }
}

export async function followUserHandler(username: string): Promise<boolean> {
  await request({
    method: "PUT",
    url: `me/followings/${username}`,
  });
  return true;
}

export async function unFollowUserHandler(username: string): Promise<boolean> {
  await request({
    method: "DELETE",
    url: `me/followings/${username}`,
  });
  return false;
}
