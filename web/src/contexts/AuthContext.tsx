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

interface AuthProviderProps {
  isLoadingAuth: boolean;
  isLoggedIn: boolean;
  login: (inputs: LoginFormValues) => Promise<void>;
  logout: () => Promise<void>;
}

const ACCESS_TOKEN_EXPIRATION_KEY = "expires";

const AuthContext = createContext<AuthProviderProps>({} as AuthProviderProps);

const AuthProvider: React.FC = (props) => {
  const { user, updateUser } = useUser();
  const [accessTokenExpiration, setAccessTokenExpiration] = useState(0);

  const accessToken = getAccessToken();

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
    const result = await authenticateUser(inputs);
    setExpirationToLocalStorage(result.accessTokenExpires);
    refetch();
  };

  const logout = async () => {
    await revokeRefreshToken();
    updateUser(null);
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
        refreshAccessToken().then((result) => {
          setExpirationToLocalStorage(result.accessTokenExpires);
        });
      }
    }
  }, 1000 * 60);

  const values = useMemo<AuthProviderProps>(
    () => ({
      isLoadingAuth: isFetching,
      isLoggedIn: isSuccess && Boolean(user),
      login,
      logout,
    }),
    [isFetching, login, logout, isSuccess]
  );

  return (
    <AuthContext.Provider value={values}>{props.children}</AuthContext.Provider>
  );
};

export default AuthProvider;

export const useAuth = () => {
  return useContext(AuthContext);
};
