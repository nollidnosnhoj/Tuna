import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { ME_QUERY_KEY } from "~/lib/hooks/api/queries/useGetCurrentUser";
import request from "~/lib/http";
import { ErrorResponse, Profile } from "~/lib/types";
import { GET_PROFILE_QUERY_KEY } from "../queries/useGetProfile";

export function useRemoveUserPicture(
  username: string
): UseMutationResult<void, ErrorResponse, void> {
  const queryClient = useQueryClient();
  const mutate = async (): Promise<void> => {
    await request({
      method: "delete",
      url: "me/picture",
    });
  };

  return useMutation(mutate, {
    onSuccess() {
      const profile = queryClient.getQueryData<Profile>(
        GET_PROFILE_QUERY_KEY(username)
      );
      if (profile) {
        queryClient.setQueryData<Profile>(GET_PROFILE_QUERY_KEY(username), {
          ...profile,
          picture: "",
        });
      }
      queryClient.invalidateQueries(ME_QUERY_KEY);
    },
  });
}
