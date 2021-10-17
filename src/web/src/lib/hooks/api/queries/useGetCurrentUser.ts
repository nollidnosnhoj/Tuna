import { useQuery, UseQueryOptions, UseQueryResult } from "react-query";
import request from "~/lib/http";
import { CurrentUser } from "~/lib/types";

export const ME_QUERY_KEY = "me";

export function useGetCurrentUser(
  options: UseQueryOptions<CurrentUser> = {}
): UseQueryResult<CurrentUser> {
  const fetcher = async (): Promise<CurrentUser> => {
    const response = await request<CurrentUser>({
      method: "get",
      url: "me",
    });
    return response.data;
  };
  return useQuery<CurrentUser>(ME_QUERY_KEY, fetcher, options);
}
