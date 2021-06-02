import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import { useAuth } from "~/features/auth/hooks";
import api from "~/lib/api";
import { ErrorResponse } from "~/lib/types";
import { Profile } from "../types";

interface FetchUserProfileOptions {
  accessToken?: string;
}

export const fetchUserProfile = async (
  username: string,
  options: FetchUserProfileOptions = {}
): Promise<Profile> => {
  const { data } = await api.get<Profile>(`users/${username}`, undefined, {
    accessToken: options.accessToken,
  });

  return data;
};

export const GET_PROFILE_QUERY_KEY = (username: string): QueryKey => [
  "profile",
  username,
];

export function useGetProfile(
  username: string,
  options: UseQueryOptions<Profile, ErrorResponse> = {}
): UseQueryResult<Profile, ErrorResponse> {
  const { accessToken } = useAuth();
  return useQuery<Profile, ErrorResponse>(
    GET_PROFILE_QUERY_KEY(username),
    () => fetchUserProfile(username, { accessToken }),
    options
  );
}
