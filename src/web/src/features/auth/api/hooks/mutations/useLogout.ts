import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { ME_QUERY_KEY } from "~/features/user/api/hooks/queries/useGetCurrentUser";
import { CurrentUser } from "~/features/user/api/types";
import { useUser } from "~/features/user/hooks";
import request from "~/lib/http";

export function useLogout(): UseMutationResult<void, void, void> {
  const queryClient = useQueryClient();
  const { updateUser } = useUser();

  const logoutHandler = async (): Promise<void> => {
    await request({
      url: "/auth/logout",
      method: "post",
    });
  };

  return useMutation(logoutHandler, {
    onSuccess() {
      updateUser(null);
      queryClient.setQueryData<CurrentUser | null>(ME_QUERY_KEY, null);
    },
  });
}
