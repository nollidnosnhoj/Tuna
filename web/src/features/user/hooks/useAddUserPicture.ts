import { useQueryClient, useMutation, UseMutationResult } from "react-query";
import { useAuth } from "~/lib/hooks/useAuth";
import api from "~/lib/api";
import { ErrorResponse } from "~/lib/types";

type ImageResult = {
  image: string;
};

export function useAddUserPicture(
  username: string
): UseMutationResult<ImageResult, ErrorResponse, string> {
  const queryClient = useQueryClient();
  const { accessToken } = useAuth();
  const uploadArtwork = async (imageData: string): Promise<ImageResult> => {
    const { data } = await api.patch<ImageResult>(
      `me/picture`,
      { data: imageData },
      { accessToken }
    );
    return data;
  };

  return useMutation(uploadArtwork, {
    onSuccess() {
      queryClient.invalidateQueries(`me`);
      queryClient.invalidateQueries(`users`);
      queryClient.invalidateQueries([`users`, username], { exact: true });
    },
  });
}
