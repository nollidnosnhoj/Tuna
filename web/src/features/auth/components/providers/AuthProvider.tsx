import React, { PropsWithChildren, useMemo } from "react";
import { LoginFormValues } from "~/features/auth/components/LoginForm";
import { authenticateUser, revokeRefreshToken } from "~/features/auth/api";
import {
  AuthContextProviderProps,
  AuthContext,
} from "~/features/auth/contexts";
import { useGetCurrentUser } from "~/features/auth/hooks";
import { useUser } from "~/features/user/hooks";
import { getAccessToken } from "~/lib/http/utils";

interface AuthProviderProps {
  accessToken?: string;
}

export function AuthProvider(props: PropsWithChildren<AuthProviderProps>) {
  const [user, updateUser] = useUser();
  const accessToken = getAccessToken();

  const { isFetching, refetch, isSuccess } = useGetCurrentUser({
    enabled: !user && !!accessToken,
    onSuccess: (data) => updateUser(data),
    onError: () => updateUser(null),
  });

  const login = async (inputs: LoginFormValues) => {
    await authenticateUser(inputs);
    refetch();
  };

  const logout = async () => {
    await revokeRefreshToken();
    updateUser(null);
  };

  const values = useMemo<AuthContextProviderProps>(
    () => ({
      isLoadingAuth: isFetching,
      isLoggedIn: isSuccess && Boolean(user),
      login,
      logout,
    }),
    [isFetching, login, logout, isSuccess, user]
  );

  return (
    <AuthContext.Provider value={values}>{props.children}</AuthContext.Provider>
  );
}
