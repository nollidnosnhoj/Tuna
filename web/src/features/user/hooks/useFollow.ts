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

export const IS_FOLLOWING_QUERY_KEY = (username: string): QueryKey => [
  "isFollowing",
  username,
];

export function useFollow(
  username: string,
  initialData?: boolean
): UseFollowResult {
  const [user] = useUser();
  const queryClient = useQueryClient();

  const { data, isLoading } = useQuery(
    IS_FOLLOWING_QUERY_KEY(username),
    () => isFollowingHandler(username),
    {
      enabled: !!user,
      initialData: initialData,
    }
  );

  const { mutateAsync } = useMutation(
    () => (data ? followUserHandler(username) : unFollowUserHandler(username)),
    {
      onSuccess(data) {
        queryClient.setQueryData(IS_FOLLOWING_QUERY_KEY(username), data);
      },
    }
  );

  return { isFollowing: data, follow: mutateAsync, isLoading };
}
