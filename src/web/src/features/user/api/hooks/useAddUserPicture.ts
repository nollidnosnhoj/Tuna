import { useQueryClient, useMutation, UseMutationResult } from "react-query";
import { ME_QUERY_KEY } from "~/features/user/api/hooks/useGetCurrentUser";
import { ErrorResponse, ImageUploadResponse } from "~/lib/types";
import { uploadUserPictureRequest } from "..";
import { Profile } from "../types";
import { GET_PROFILE_QUERY_KEY } from "./useGetProfile";

export function useAddUserPicture(
  username: string
): UseMutationResult<ImageUploadResponse, ErrorResponse, string> {
  const queryClient = useQueryClient();
  return useMutation(uploadUserPictureRequest, {
    onSuccess(data) {
      const profile = queryClient.getQueryData<Profile>(
        GET_PROFILE_QUERY_KEY(username)
      );
      if (profile) {
        queryClient.setQueryData<Profile>(GET_PROFILE_QUERY_KEY(username), {
          ...profile,
          picture: data.url,
        });
      }
      queryClient.invalidateQueries(ME_QUERY_KEY);
    },
  });
}
