
import { useState, useEffect } from "react";
import { useQuery, UseQueryOptions } from "react-query";
import { apiErrorToast } from "~/utils/toast";
import request from "../request";
import { ErrorResponse } from "../types";
import { Profile } from "../types/user";

export const useFollow = (username: string) => {
  const [isFollowing, setIsFollowing] = useState<boolean | undefined>(undefined);

  useEffect(() => {
    const checkIsFollowing = async () => {
      try {
        await request(`me/followings/${username}`, { method: "head" });
        setIsFollowing(true);
      } catch (err) {
        setIsFollowing(false);
      }
    };
    checkIsFollowing();
  }, []);

  const followHandler = async () => {
    try {
      await request(`me/followings/${username}`, {
        method: isFollowing ? "delete" : "put",
      });
      setIsFollowing(!isFollowing);
    } catch (err) {
      apiErrorToast(err);
    }
  }

  return { isFollowing, follow: followHandler };
}

interface FetchUserProfileOptions {
  accessToken?: string;
}

export const fetchUserProfile = async (username: string, options: FetchUserProfileOptions = {}) => {
  const { data } = await request<Profile>(`users/${username}`, {
    method: 'get',
    accessToken: options.accessToken
  });

  return data;
}

export const useProfile = (username: string, options: UseQueryOptions<Profile, ErrorResponse> = {}) => {
  return useQuery<Profile, ErrorResponse>(["users", username], () => fetchUserProfile(username), options);
}