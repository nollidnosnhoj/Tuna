import { useToast } from "@chakra-ui/toast";
import { useRouter } from "next/router";
import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import request from "~/lib/http";
import { useLoginModal } from "~/lib/stores";
import { ErrorResponse } from "~/lib/types";
import { errorToast } from "~/utils";
import { useUser } from "../../../hooks";
import { CurrentUser } from "../../types";
import { ME_QUERY_KEY } from "../queries/useGetCurrentUser";

type Input = { currentPassword: string; newPassword: string };

export function useUpdatePassword(): UseMutationResult<
  void,
  ErrorResponse,
  Input
> {
  const onLoginModalOpen = useLoginModal((state) => state.onOpen);
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
      toast({
        status: "info",
        description: "You will be logged out to sign in again.",
      });
      updateUser(null);
      qc.setQueryData<CurrentUser | null>(ME_QUERY_KEY, null);
      router.push("/").then(() => {
        onLoginModalOpen("login");
      });
    },
  });
}
