import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import request from "~/lib/http";
import { ErrorResponse } from "~/lib/types";
import { errorToast } from "~/utils";
import { useUser } from "../../../hooks";
import { CurrentUser } from "../../types";
import { ME_QUERY_KEY } from "../queries/useGetCurrentUser";

type Input = {
  newEmail: string;
};

export function useUpdateEmail(): UseMutationResult<
  void,
  ErrorResponse,
  Input
> {
  const queryClient = useQueryClient();
  const { user, updateUser } = useUser();
  const handler = async (input: Input): Promise<void> => {
    await request({
      method: "patch",
      url: "me/email",
      data: input,
    });
  };

  return useMutation(handler, {
    onError(err) {
      errorToast(err);
    },
    onSuccess(_, input) {
      if (user) {
        updateUser({ ...user, email: input.newEmail });
        queryClient.setQueryData<CurrentUser>(ME_QUERY_KEY, {
          ...user,
          email: input.newEmail,
        });
      }
    },
  });
}
