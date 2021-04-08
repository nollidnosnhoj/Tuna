import { useState, useEffect } from "react";
import useUser from '~/hooks/useUser';
import api from "~/utils/api";
import { apiErrorToast } from "~/utils/toast";


export function useFollow(username: string, initialData?: boolean) {
  const { user } = useUser();
  const [isFollowing, setIsFollowing] = useState<boolean | undefined>(initialData);

  useEffect(() => {
    if (user && isFollowing === undefined) {
      api.head(`me/following/${username}`)
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
    api.request(`me/followings/${username}`, method)
      .then(() => setIsFollowing(!isFollowing))
      .catch(err => apiErrorToast(err));
  };

  return { isFollowing, follow: followHandler };
}
