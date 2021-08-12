import { GetServerSidePropsContext } from "next";
import request from "~/lib/http";
import { ImageUploadResponse, OffsetPagedList } from "~/lib/types";
import { AudioView } from "../../audio/api/types";
import { CurrentUser, Profile } from "./types";

export async function getCurrentUserRequest(): Promise<CurrentUser> {
  const response = await request<CurrentUser>({
    method: "get",
    url: "me",
  });
  return response.data;
}

export async function getProfileRequest(
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

export async function getUserAudiosRequest(
  username: string,
  offset = 0,
  params?: Record<string, string | number | boolean>
): Promise<OffsetPagedList<AudioView>> {
  const { data } = await request<OffsetPagedList<AudioView>>({
    method: "get",
    url: `users/${username}/audios`,
    params: {
      ...params,
      offset,
    },
  });
  return data;
}

export async function getUserFavoriteAudiosRequest(
  username: string,
  offset?: number,
  params?: Record<string, string | number | boolean>
): Promise<OffsetPagedList<AudioView>> {
  const { data } = await request<OffsetPagedList<AudioView>>({
    method: "get",
    url: `users/${username}/favorite/audios`,
    params: {
      ...params,
      offset,
    },
  });
  return data;
}

export async function uploadUserPictureRequest(
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

export async function checkIfCurrentUserIsFollowingRequest(
  userId: number
): Promise<boolean> {
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

export async function followAUserRequest(userId: number): Promise<boolean> {
  await request({
    method: "PUT",
    url: `me/followings/${userId}`,
  });
  return true;
}

export async function unfollowAUserRequest(userId: number): Promise<boolean> {
  await request({
    method: "DELETE",
    url: `me/followings/${userId}`,
  });
  return false;
}
