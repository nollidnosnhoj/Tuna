import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { ME_QUERY_KEY } from "~/lib/hooks/api/queries/useGetCurrentUser";
import request from "~/lib/http";
import { ErrorResponse, ImageUploadResponse, Profile } from "~/lib/types";
import { GET_PROFILE_QUERY_KEY } from "../queries/useGetProfile";

export function useAddUserPicture(
  username: string
): UseMutationResult<ImageUploadResponse, ErrorResponse, string> {
  const queryClient = useQueryClient();
  const mutate = async (imageData: string): Promise<ImageUploadResponse> => {
    const { data } = await request<ImageUploadResponse>({
      method: "patch",
      url: "me/picture",
      data: {
        data: imageData,
      },
    });
    return data;
  };

  return useMutation(mutate, {
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
