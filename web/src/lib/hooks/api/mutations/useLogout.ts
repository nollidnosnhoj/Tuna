import { useMutation, UseMutationResult, useQueryClient } from "@tanstack/react-query";
import { useUser } from "~/components/providers/UserProvider";
import request from "~/lib/http";
import { CurrentUser } from "~/lib/types";
import { ME_QUERY_KEY } from "~/lib/hooks/api/keys";

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
