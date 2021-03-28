import { Profile } from '~/features/user/types';
import api from '~/utils/api'

interface FetchUserProfileOptions {
  accessToken?: string;
}

export const fetchUserProfile = async (username: string, options: FetchUserProfileOptions = {}) => {
  const { data } = await api.get<Profile>(`users/${username}`, {
    accessToken: options.accessToken
  });

  return data;
}