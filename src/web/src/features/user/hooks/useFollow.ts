import { QueryKey, useMutation, useQuery, useQueryClient } from "react-query";
import { useUser } from "~/features/user/hooks";
import {
  followAUserRequest,
  checkIfCurrentUserIsFollowingRequest,
  unfollowAUserRequest,
} from "../api";

type UseFollowResult = {
  isFollowing?: boolean;
  follow: () => void;
  isLoading: boolean;
};

export const IS_FOLLOWING_QUERY_KEY = (userId: number): QueryKey => [
  "isFollowing",
  userId,
];

export function useFollow(
  userId: number,
  initialData?: boolean
): UseFollowResult {
  const { user } = useUser();
  const queryClient = useQueryClient();

  const { data, isLoading } = useQuery(
    IS_FOLLOWING_QUERY_KEY(userId),
    () => checkIfCurrentUserIsFollowingRequest(userId),
    {
      enabled: !!user && !!userId,
      initialData: initialData,
    }
  );

  const { mutateAsync } = useMutation(
    () => (data ? followAUserRequest(userId) : unfollowAUserRequest(userId)),
    {
      onSuccess(data) {
        queryClient.setQueryData(IS_FOLLOWING_QUERY_KEY(userId), data);
      },
    }
  );

  return { isFollowing: data, follow: mutateAsync, isLoading };
}
