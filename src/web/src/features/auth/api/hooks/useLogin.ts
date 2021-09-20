import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { ME_QUERY_KEY } from "~/features/user/api/hooks/useGetCurrentUser";
import { CurrentUser } from "~/features/user/api/types";
import { useUser } from "~/features/user/hooks";
import request from "~/lib/http";
import { ErrorResponse } from "~/lib/types";
import { LoginFormValues } from "../../components/Forms/Login";

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
