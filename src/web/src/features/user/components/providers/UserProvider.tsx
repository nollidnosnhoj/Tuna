import React, { PropsWithChildren, useMemo, useState } from "react";
import { CurrentUser } from "../../api/types";
import { UserContextType, UserContext } from "../../contexts";
import { useGetCurrentUser } from "../../api/hooks/useGetCurrentUser";
import { getAccessToken } from "~/lib/http/utils";

interface UserProviderProps {
  initialUser: CurrentUser | null;
}

export function UserProvider(props: PropsWithChildren<UserProviderProps>) {
  const { initialUser, children } = props;
  const accessToken = getAccessToken();
  const [user, setUser] = useState<CurrentUser | null>(initialUser);

  const { isLoading, refetch } = useGetCurrentUser({
    enabled: !!accessToken,
    onSuccess(data) {
      setUser(data);
    },
    onError() {
      setUser(null);
    },
  });

  const updateUser = (updatedUser: CurrentUser | null) => {
    setUser(updatedUser);
  };

  const refreshUser = async () => {
    await refetch();
  };

  const values = useMemo<UserContextType>(
    () => ({
      isLoadingUser: isLoading,
      isLoggedIn: !!user,
      refreshUser,
      user,
      updateUser,
    }),
    [user, updateUser, refreshUser, isLoading]
  );

  return <UserContext.Provider value={values}>{children}</UserContext.Provider>;
}
