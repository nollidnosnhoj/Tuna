import { useInterval } from "@chakra-ui/react";
import React, {
  createContext,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import { useQuery } from "react-query";
import { LoginFormValues } from "~/features/auth/components/LoginForm";
import {
  authenticateUser,
  refreshAccessToken,
  revokeRefreshToken,
} from "~/features/auth/services";
import { CurrentUser } from "~/features/user/types";
import API from "~/utils/api";
import { getAccessToken } from "~/utils/cookies";
import { useUser } from "./UserContext";

interface AuthContextProviderProps {
  accessToken: string;
  isLoadingAuth: boolean;
  isLoggedIn: boolean;
  login: (inputs: LoginFormValues) => Promise<void>;
  logout: () => Promise<void>;
}

interface AuthProviderProps {
  accessToken?: string;
}

const ACCESS_TOKEN_EXPIRATION_KEY = "expires";

const AuthContext = createContext<AuthContextProviderProps>(
  {} as AuthContextProviderProps
);

const AuthProvider: React.FC<AuthProviderProps> = (props) => {
  const { user, updateUser } = useUser();
  const [accessToken, setAccessToken] = useState(
    () => props.accessToken || getAccessToken()
  );
  const [accessTokenExpiration, setAccessTokenExpiration] = useState(0);

  const { isFetching, refetch, isSuccess } = useQuery<CurrentUser>(
    "me",
    async () => {
      const { data } = await API.get<CurrentUser>("me", undefined, {
        accessToken,
      });
      return data;
    },
    {
      enabled: Boolean(accessToken),
      onSuccess: (data) => updateUser(data),
      onError: () => updateUser(null),
    }
  );

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
        refreshAccessToken().then(([newToken, newExpires]) => {
          setAccessToken(newToken);
          setExpirationToLocalStorage(newExpires);
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
    [accessToken, isFetching, login, logout, isSuccess]
  );

  return (
    <AuthContext.Provider value={values}>{props.children}</AuthContext.Provider>
  );
};

export default AuthProvider;

export const useAuth = () => {
  return useContext(AuthContext);
};
