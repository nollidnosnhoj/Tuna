
import { useState, useEffect } from "react";
import { apiErrorToast } from "~/utils/toast";
import request from "../request";

export const useFollow = (username: string) => {
  const [isFollowing, setIsFollowing] = useState<boolean | undefined>(undefined);

  useEffect(() => {
    const checkIsFollowing = async () => {
      try {
        await request(`me/users/${username}/follow`, { method: "head" });
        setIsFollowing(true);
      } catch (err) {
        setIsFollowing(false);
      }
    };
    checkIsFollowing();
  }, []);

  const followHandler = async () => {
    try {
      await request(`me/users/${username}/follow`, {
        method: isFollowing ? "delete" : "put",
      });
      setIsFollowing(!isFollowing);
    } catch (err) {
      apiErrorToast(err);
    }
  }

  return { isFollowing, follow: followHandler };
}