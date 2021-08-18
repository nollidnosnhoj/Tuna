import { useCallback } from "react";
import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import request from "~/lib/http";
import { ErrorResponse } from "~/lib/types";
import { Profile } from "../types";

export const GET_PROFILE_QUERY_KEY = (username: string): QueryKey => [
  "profile",
  username,
];

export function useGetProfile(
  username: string,
  options: UseQueryOptions<Profile, ErrorResponse> = {}
): UseQueryResult<Profile, ErrorResponse> {
  const fetcher = useCallback(async () => {
    const { data } = await request<Profile>({
      method: "get",
      url: `users/${username}`,
    });

    return data;
  }, [username]);
  return useQuery<Profile, ErrorResponse>(
    GET_PROFILE_QUERY_KEY(username),
    fetcher,
    options
  );
}
