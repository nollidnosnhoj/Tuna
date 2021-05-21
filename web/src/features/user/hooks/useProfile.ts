import { useQuery, UseQueryOptions, UseQueryResult } from "react-query";
import { useAuth } from "~/lib/hooks/useAuth";
import { ErrorResponse } from "~/lib/types";
import { fetchUserProfile } from "../services";
import { Profile } from "../types";

export function useProfile(
  username: string,
  options: UseQueryOptions<Profile, ErrorResponse> = {}
): UseQueryResult<Profile, ErrorResponse> {
  const { accessToken } = useAuth();
  return useQuery<Profile, ErrorResponse>(
    ["users", username],
    () => fetchUserProfile(username, { accessToken }),
    options
  );
}
