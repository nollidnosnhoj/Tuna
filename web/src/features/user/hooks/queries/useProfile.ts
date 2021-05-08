import { useQuery, UseQueryOptions, UseQueryResult } from "react-query";
import { ErrorResponse } from "../../../../lib/types";
import { Profile } from "../../types";
import { fetchUserProfile } from "../../services/fetchUserProfile";
import { useAuth } from "~/lib/hooks/useAuth";

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
