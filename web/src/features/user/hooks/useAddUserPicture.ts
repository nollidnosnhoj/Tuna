import { useQueryClient, useMutation, UseMutationResult } from "react-query";
import { ME_QUERY_KEY } from "~/features/auth/hooks";
import { useAuth } from "~/features/auth/hooks/useAuth";
import api from "~/lib/api";
import { ErrorResponse } from "~/lib/types";
import { Profile } from "../types";
import { GET_PROFILE_QUERY_KEY } from "./useGetProfile";

export function useAddUserPicture(
  username: string
): UseMutationResult<Profile, ErrorResponse, string> {
  const queryClient = useQueryClient();
  const { accessToken } = useAuth();
  const uploadArtwork = async (imageData: string): Promise<Profile> => {
    const { data } = await api.patch<Profile>(
      `me/picture`,
      { data: imageData },
      { accessToken }
    );
    return data;
  };

  return useMutation(uploadArtwork, {
    onSuccess(data) {
      queryClient.setQueryData<Profile>(GET_PROFILE_QUERY_KEY(username), data);
      queryClient.invalidateQueries(ME_QUERY_KEY);
    },
  });
}
