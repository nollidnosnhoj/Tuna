import { useQuery, UseQueryResult } from "react-query";
import { CurrentUser } from "~/features/user/types";
import API from "~/lib/api";
import { useUser } from "~/features/user/hooks";

export const ME_QUERY_KEY = "me";

export function useGetCurrentUser(
  accessToken: string
): UseQueryResult<CurrentUser> {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [_, updateUser] = useUser();
  return useQuery<CurrentUser>(
    ME_QUERY_KEY,
    async () => {
      const { data } = await API.get<CurrentUser>("me", undefined, {
        accessToken,
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
