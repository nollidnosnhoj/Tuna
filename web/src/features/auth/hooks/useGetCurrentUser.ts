import { useQuery, UseQueryOptions, UseQueryResult } from "react-query";
import { CurrentUser } from "~/features/user/types";
import { getCurrentUser } from "../api/getCurrentUser";

export const ME_QUERY_KEY = "me";

export function useGetCurrentUser(
  options: UseQueryOptions<CurrentUser> = {}
): UseQueryResult<CurrentUser> {
  return useQuery<CurrentUser>(ME_QUERY_KEY, () => getCurrentUser(), options);
}
