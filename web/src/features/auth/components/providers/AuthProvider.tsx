import React, { PropsWithChildren, useMemo } from "react";
import { LoginFormValues } from "~/features/auth/components/LoginForm";
import { authenticateUser, revokeRefreshToken } from "~/features/auth/api";
import {
  AuthContextProviderProps,
  AuthContext,
} from "~/features/auth/contexts";
import { useGetCurrentUser } from "~/features/auth/hooks";
import { useUser } from "~/features/user/hooks";

interface AuthProviderProps {
  accessToken?: string;
}

export function AuthProvider(props: PropsWithChildren<AuthProviderProps>) {
  const [user, updateUser] = useUser();

  const { isFetching, refetch, isSuccess } = useGetCurrentUser();

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
