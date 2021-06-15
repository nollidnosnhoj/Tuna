import { useQuery, UseQueryResult } from "react-query";
import { CurrentUser } from "~/features/user/types";
import { useUser } from "~/features/user/hooks";
import request from "~/lib/http";
import { getAccessToken } from "~/lib/http/utils";

export const ME_QUERY_KEY = "me";

export function useGetCurrentUser(): UseQueryResult<CurrentUser> {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [_, updateUser] = useUser();
  const accessToken = getAccessToken();
  return useQuery<CurrentUser>(
    ME_QUERY_KEY,
    async () => {
      const { data } = await request<CurrentUser>({
        method: "get",
        route: "me",
      });
      return data;
    },
    {
      enabled: Boolean(accessToken),
      onSuccess: (data) => updateUser(data),
      onError: () => updateUser(null),
    }
  );
}
