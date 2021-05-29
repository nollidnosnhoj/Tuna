import { useInterval } from "@chakra-ui/react";
import React, { PropsWithChildren, useEffect, useMemo, useState } from "react";
import { LoginFormValues } from "~/features/auth/components/LoginForm";
import {
  authenticateUser,
  revokeRefreshToken,
  refreshAccessToken,
} from "~/features/auth/services";
import {
  ACCESS_TOKEN_EXPIRATION_KEY,
  AuthContextProviderProps,
  AuthContext,
} from "~/features/auth/contexts";
import { useGetCurrentUser } from "~/features/auth/hooks";
import { useUser } from "~/features/user/hooks/useUser";
import { getAccessToken, errorToast } from "~/utils";

interface AuthProviderProps {
  accessToken?: string;
}

export function AuthProvider(props: PropsWithChildren<AuthProviderProps>) {
  const { user, updateUser } = useUser();
  const [accessToken, setAccessToken] = useState(
    () => props.accessToken || getAccessToken()
  );
  const [accessTokenExpiration, setAccessTokenExpiration] = useState(0);

  const { isFetching, refetch, isSuccess } = useGetCurrentUser(accessToken);

  const setExpirationToLocalStorage = (exp: number) => {
    setAccessTokenExpiration(() => {
      if (typeof window !== "undefined") {
        exp <= 0
          ? window.localStorage.removeItem(ACCESS_TOKEN_EXPIRATION_KEY)
          : window.localStorage.setItem(
              ACCESS_TOKEN_EXPIRATION_KEY,
              exp.toString()
            );
      }
      return exp;
    });
  };

  const login = async (inputs: LoginFormValues) => {
    const [newToken, newExpires] = await authenticateUser(inputs);
    setAccessToken(newToken);
    setExpirationToLocalStorage(newExpires);
    refetch();
  };

  const logout = async () => {
    await revokeRefreshToken();
    updateUser(null);
    setAccessToken("");
    setExpirationToLocalStorage(0);
  };

  useEffect(() => {
    const localExpires = window.localStorage.getItem(
      ACCESS_TOKEN_EXPIRATION_KEY
    );
    if (localExpires) {
      const parsedInt = parseInt(localExpires);
      setAccessTokenExpiration(isNaN(parsedInt) ? 0 : parsedInt);
    }
  }, []);

  useInterval(() => {
    if (user) {
      const now = Date.now() / 1000;
      if (accessTokenExpiration <= now) {
        refreshAccessToken()
          .then(([newToken, newExpires]) => {
            setAccessToken(newToken);
            setExpirationToLocalStorage(newExpires);
          })
          .catch(() => {
            errorToast({
              message: "Session expired. Please login.",
            });
          });
      }
    }
  }, 1000 * 60);

  const values = useMemo<AuthContextProviderProps>(
    () => ({
      accessToken,
      isLoadingAuth: isFetching,
      isLoggedIn: isSuccess && Boolean(user),
      login,
      logout,
    }),
    [accessToken, isFetching, login, logout, isSuccess, user]
  );

  return (
    <AuthContext.Provider value={values}>{props.children}</AuthContext.Provider>
  );
}
