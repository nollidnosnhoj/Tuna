import { useState, useEffect } from "react";
import { useAuth } from "~/contexts/AuthContext";
import {useUser} from '~/contexts/UserContext';
import api from "~/utils/api";
import { apiErrorToast } from "~/utils/toast";


export function useFollow(username: string, initialData?: boolean) {
  const { user } = useUser();
  const { accessToken } = useAuth();
  const [isFollowing, setIsFollowing] = useState<boolean | undefined>(initialData);

  useEffect(() => {
    if (user && isFollowing === undefined) {
      api.head(`me/following/${username}`, { accessToken })
        .then(() => {
          setIsFollowing(true);
        })
        .catch(() => {
          setIsFollowing(false);
        });
    }
  }, []);

  const followHandler = () => {
    const method = isFollowing ? 'DELETE' : 'PUT';
    api.request(method, `me/followings/${username}`, { accessToken })
      .then(() => setIsFollowing(!isFollowing))
      .catch(err => apiErrorToast(err));
  };

  return { isFollowing, follow: followHandler };
}
