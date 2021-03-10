import { useQuery, UseQueryOptions } from "react-query";
import { ErrorResponse } from "../../../lib/types";
import { Profile } from "../types";
import { fetchUserProfile } from "../services/fetch";

export const useProfile = (username: string, options: UseQueryOptions<Profile, ErrorResponse> = {}) => {
  return useQuery<Profile, ErrorResponse>(["users", username], () => fetchUserProfile(username), options);
}

