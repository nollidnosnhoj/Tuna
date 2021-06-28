import { useQueryClient, useMutation, UseMutationResult } from "react-query";
import { ME_QUERY_KEY } from "~/features/user/hooks/useGetCurrentUser";
import { ErrorResponse } from "~/lib/types";
import { uploadUserPictureHandler } from "../api";
import { Profile } from "../types";
import { GET_PROFILE_QUERY_KEY } from "./useGetProfile";

export function useAddUserPicture(
  username: string
): UseMutationResult<Profile, ErrorResponse, string> {
  const queryClient = useQueryClient();
  return useMutation(uploadUserPictureHandler, {
    onSuccess(data) {
      queryClient.setQueryData<Profile>(GET_PROFILE_QUERY_KEY(username), data);
      queryClient.invalidateQueries(ME_QUERY_KEY);
    },
  });
}
