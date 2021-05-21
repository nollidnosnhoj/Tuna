import { useQuery, UseQueryResult } from "react-query";
import { CurrentUser } from "~/features/user/types";
import API from "~/lib/api";
import { useUser } from "~/lib/hooks/useUser";

export function useGetCurrentUser(
  accessToken: string
): UseQueryResult<CurrentUser> {
  const { updateUser } = useUser();
  return useQuery<CurrentUser>(
    "me",
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
