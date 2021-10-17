import { useToast } from "@chakra-ui/toast";
import { useRouter } from "next/router";
import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import request from "~/lib/http";
import { CurrentUser, ErrorResponse } from "~/lib/types";
import { errorToast } from "~/utils";
import { ME_QUERY_KEY } from "../queries/useGetCurrentUser";
import { useUser } from "~/components/providers/UserProvider";

type Input = { currentPassword: string; newPassword: string };

export function useUpdatePassword(): UseMutationResult<
  void,
  ErrorResponse,
  Input
> {
  const toast = useToast();
  const router = useRouter();
  const qc = useQueryClient();
  const { updateUser } = useUser();
  const handler = async (input: Input): Promise<void> => {
    await request({
      method: "patch",
      url: "me/password",
      data: input,
    });
  };

  return useMutation(handler, {
    onError(err) {
      errorToast(err);
    },
    onSuccess() {
      updateUser(null);
      qc.setQueryData<CurrentUser | null>(ME_QUERY_KEY, null);
      router.push("/login").then(() => {
        toast({
          status: "info",
          description: "Please login again with your new password.",
        });
      });
    },
  });
}
