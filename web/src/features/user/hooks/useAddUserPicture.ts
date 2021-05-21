import { useQueryClient, useMutation, UseMutationResult } from "react-query";
import { useAuth } from "~/lib/hooks/useAuth";
import api from "~/lib/api";
import { ErrorResponse } from "~/lib/types";
import { Profile } from "../types";

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
      queryClient.setQueryData<Profile>([`users`, username], data);
      queryClient.invalidateQueries(`me`);
      queryClient.invalidateQueries(`users`);
    },
  });
}
