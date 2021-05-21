import { useState, useEffect } from "react";
import { useAuth } from "~/lib/hooks/useAuth";
import { useUser } from "~/lib/hooks/useUser";
import api from "~/lib/api";
import { errorToast } from "~/utils";

type UseFollowResult = {
  isFollowing?: boolean;
  follow: () => void;
};

export function useFollow(
  username: string,
  initialData?: boolean
): UseFollowResult {
  const { user } = useUser();
  const { accessToken } = useAuth();
  const [isFollowing, setIsFollowing] =
    useState<boolean | undefined>(initialData);

  useEffect(() => {
    if (user && isFollowing === undefined) {
      api
        .head(`me/following/${username}`, { accessToken })
        .then(() => {
          setIsFollowing(true);
        })
        .catch(() => {
          setIsFollowing(false);
        });
    }
  }, []);

  const followHandler = (): void => {
    const method = isFollowing ? "DELETE" : "PUT";
    api
      .request(method, `me/followings/${username}`, { accessToken })
      .then(() => setIsFollowing(!isFollowing))
      .catch((err) => errorToast(err));
  };

  return { isFollowing, follow: followHandler };
}
