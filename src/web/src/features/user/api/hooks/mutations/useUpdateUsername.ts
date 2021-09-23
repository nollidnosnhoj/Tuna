import { useToast } from "@chakra-ui/toast";
import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import request from "~/lib/http";
import { ErrorResponse } from "~/lib/types";
import { errorToast } from "~/utils";
import { useUser } from "../../../hooks";
import { CurrentUser } from "../../types";
import { ME_QUERY_KEY } from "../queries/useGetCurrentUser";

type Input = { newUsername: string };

export function useUpdateUsername(): UseMutationResult<
  void,
  ErrorResponse,
  Input
> {
  const toast = useToast();
  const qc = useQueryClient();
  const { user, updateUser } = useUser();

  const handler = async (inputs: Input): Promise<void> => {
    await request({
      method: "patch",
      url: "me/username",
      data: inputs,
    });
  };

  return useMutation(handler, {
    onError(err) {
      errorToast(err);
    },
    onSuccess(_, { newUsername }) {
      toast({
        status: "success",
        title: "Username updated.",
        description: "You have successfully updated your username.",
      });
      if (user) {
        updateUser({ ...user, userName: newUsername });
        qc.setQueryData<CurrentUser>(ME_QUERY_KEY, {
          ...user,
          userName: newUsername,
        });
      }
    },
  });
}
