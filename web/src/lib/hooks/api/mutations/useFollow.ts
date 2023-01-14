import { useCallback } from "react";
import { QueryKey, useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import request from "~/lib/http";
import { ID } from "~/lib/types";
import { useUser } from "~/components/providers/UserProvider";

type UseFollowResult = {
  isFollowing?: boolean;
  follow: () => void;
  isLoading: boolean;
};

export const IS_FOLLOWING_QUERY_KEY = (userId: ID): QueryKey => [
  "isFollowing",
  userId,
];

export function useFollow(userId: ID, initialData?: boolean): UseFollowResult {
  const { user } = useUser();
  const queryClient = useQueryClient();

  const fetcher = useCallback(async () => {
    try {
      const res = await request({
        method: "head",
        url: `me/following/${userId}`,
        validateStatus: (status) => status === 404 || status < 400,
      });
      return res.status !== 404;
    } catch (err) {
      return false;
    }
  }, [userId]);

  const { data: isFollowing, isLoading } = useQuery(
    IS_FOLLOWING_QUERY_KEY(userId),
    fetcher,
    {
      enabled: !!user && !!userId,
      initialData: initialData,
    }
  );

  const mutate = useCallback(async () => {
    if (isFollowing) {
      await request({
        method: "DELETE",
        url: `me/followings/${userId}`,
      });
    } else {
      await request({
        method: "PUT",
        url: `me/followings/${userId}`,
      });
    }
    return true;
  }, [userId, isFollowing]);

  const { mutateAsync } = useMutation(mutate, {
    onSuccess() {
      queryClient.setQueryData(IS_FOLLOWING_QUERY_KEY(userId), !isFollowing);
    },
  });

  return { isFollowing, follow: mutateAsync, isLoading };
}
