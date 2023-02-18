import { useMutation, UseMutationResult, useQueryClient } from "@tanstack/react-query";
import { useUser } from "~/components/providers/UserProvider";
import request from "~/lib/http";
import { CurrentUser, ErrorResponse } from "~/lib/types";
import { LoginFormValues } from "../../../../components/forms/LoginForm";
import { ME_QUERY_KEY } from "~/lib/hooks/api/keys";

export function useLogin(): UseMutationResult<
  CurrentUser,
  ErrorResponse,
  LoginFormValues
> {
  const queryClient = useQueryClient();
  const { updateUser } = useUser();

  const loginHandler = async (
    inputs: LoginFormValues
  ): Promise<CurrentUser> => {
    const { data } = await request<CurrentUser>({
      url: "/auth/login",
      data: inputs,
      method: "post",
    });
    return data;
  };

  return useMutation(loginHandler, {
    onSuccess(data) {
      updateUser(data);
      queryClient.setQueryData<CurrentUser>(ME_QUERY_KEY, data);
    },
  });
}
