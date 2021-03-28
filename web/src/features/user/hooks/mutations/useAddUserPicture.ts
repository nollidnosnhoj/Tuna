import { useQueryClient, useMutation } from "react-query";
import api from "~/utils/api";


export function useAddUserPicture(username: string) {
  const queryClient = useQueryClient();
  const uploadArtwork = async (data: string) => await api.patch<{ image: string; }>(`me/picture`, { data });

  return useMutation(uploadArtwork, {
    onSuccess() {
      queryClient.invalidateQueries(`me`);
      queryClient.invalidateQueries(`users`);
      queryClient.invalidateQueries([`users`, username], { exact: true });
    }
  });
}
