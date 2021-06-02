import { useState } from "react";
import { QueryKey, useMutation, useQuery, useQueryClient } from "react-query";
import { useAuth } from "~/features/auth/hooks/useAuth";
import { useUser } from "~/features/user/hooks/useUser";
import api from "~/lib/api";

type UseFollowResult = {
  isFollowing?: boolean;
  follow: () => void;
};

export const IS_FOLLOWING_QUERY_KEY = (username: string): QueryKey => [
  "isFollowing",
  username,
];

export function useFollow(
  username: string,
  initialData?: boolean
): UseFollowResult {
  const { user } = useUser();
  const { accessToken } = useAuth();
  const queryClient = useQueryClient();
  const [isFollowing, setIsFollowing] =
    useState<boolean | undefined>(initialData);

  useQuery(
    IS_FOLLOWING_QUERY_KEY(username),
    () => api.head(`me/following/${username}`, { accessToken }),
    {
      onSuccess() {
        setIsFollowing(true);
      },
      onError() {
        setIsFollowing(false);
      },
      enabled: isFollowing === undefined && !!user,
    }
  );

  const followHandler = (): Promise<unknown> => {
    const method = isFollowing ? "DELETE" : "PUT";
    return api.request(method, `me/followings/${username}`, { accessToken });
  };

  const { mutateAsync } = useMutation(followHandler, {
    onSuccess() {
      setIsFollowing((prev) => !prev);
      queryClient.setQueryData(IS_FOLLOWING_QUERY_KEY(username), !isFollowing);
    },
  });

  return { isFollowing, follow: mutateAsync };
}
