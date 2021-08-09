import {
  QueryKey,
  useQuery,
  UseQueryOptions,
  UseQueryResult,
} from "react-query";
import { ErrorResponse } from "~/lib/types";
import { getProfileRequest } from "..";
import { Profile } from "../types";

export const GET_PROFILE_QUERY_KEY = (username: string): QueryKey => [
  "profile",
  username,
];

export function useGetProfile(
  username: string,
  options: UseQueryOptions<Profile, ErrorResponse> = {}
): UseQueryResult<Profile, ErrorResponse> {
  return useQuery<Profile, ErrorResponse>(
    GET_PROFILE_QUERY_KEY(username),
    () => getProfileRequest(username),
    options
  );
}
