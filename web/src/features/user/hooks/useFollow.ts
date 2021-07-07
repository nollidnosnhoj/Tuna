import { QueryKey, useMutation, useQuery, useQueryClient } from "react-query";
import { useUser } from "~/features/user/hooks";
import {
  followUserHandler,
  isFollowingHandler,
  unFollowUserHandler,
} from "../api";

type UseFollowResult = {
  isFollowing?: boolean;
  follow: () => void;
  isLoading: boolean;
};

export const IS_FOLLOWING_QUERY_KEY = (userId: string): QueryKey => [
  "isFollowing",
  userId,
];

export function useFollow(
  userId: string,
  initialData?: boolean
): UseFollowResult {
  const { user } = useUser();
  const queryClient = useQueryClient();

  const { data, isLoading } = useQuery(
    IS_FOLLOWING_QUERY_KEY(userId),
    () => isFollowingHandler(userId),
    {
      enabled: !!user && !!userId,
      initialData: initialData,
    }
  );

  const { mutateAsync } = useMutation(
    () => (data ? followUserHandler(userId) : unFollowUserHandler(userId)),
    {
      onSuccess(data) {
        queryClient.setQueryData(IS_FOLLOWING_QUERY_KEY(userId), data);
      },
    }
  );

  return { isFollowing: data, follow: mutateAsync, isLoading };
}
