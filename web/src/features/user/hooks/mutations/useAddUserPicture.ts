import { useQueryClient, useMutation } from "react-query";
import { useAuth } from "~/lib/contexts/AuthContext";
import api from "~/lib/api";


export function useAddUserPicture(username: string) {
  const queryClient = useQueryClient();
  const { accessToken } = useAuth();
  const uploadArtwork = async (data: string) => await api.patch<{ image: string; }>(`me/picture`, { data }, { accessToken });

  return useMutation(uploadArtwork, {
    onSuccess() {
      queryClient.invalidateQueries(`me`);
      queryClient.invalidateQueries(`users`);
      queryClient.invalidateQueries([`users`, username], { exact: true });
    }
  });
}
