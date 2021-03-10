import React, {
  createContext,
  PropsWithChildren,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import { LoginFormValues } from "~/features/auth/components/LoginForm";
import {
  login,
  refreshAccessToken,
  revokeRefreshToken,
} from "../features/auth/services";
import { CurrentUser } from "../features/user/types";
import api from "~/utils/api";
import { getAccessToken } from "~/utils/cookies";
import { successfulToast } from "~/utils/toast";
import { useInterval } from "@chakra-ui/react";

type UserContextType = {
  user?: CurrentUser;
  login: (inputs: LoginFormValues) => Promise<void>;
  logout: (message?: string) => Promise<boolean>;
  updateUser: (updatedUser: CurrentUser) => void;
  isLoading: boolean;
  isLoggedIn: boolean;
};

const UserContext = createContext<UserContextType>({} as UserContextType);

interface UserProviderProps {
  initialUser?: CurrentUser;
}

export function UserProvider(props: PropsWithChildren<UserProviderProps>) {
  const [user, setUser] = useState<CurrentUser | undefined>(props.initialUser);
  const [expires, setExpires] = useState(0);
  const [loadingUser, setLoadingUser] = useState(false);

  async function authenticate(inputs: LoginFormValues) {
    const result = await login(inputs);
    setExpirationToLocalStorage(result.accessTokenExpires);
    await fetchAuthenticatedUser();
  }

  const updateUser = (updatedUser: CurrentUser | undefined) => {
    setUser(updatedUser);
  };

  const fetchAuthenticatedUser = () => {
    return new Promise<CurrentUser>((resolve, reject) => {
      setLoadingUser(() => true);
      api
        .get<CurrentUser>("me")
        .then(({ data }) => {
          setUser(data);
          resolve(data);
        })
        .catch(() => {
          deauthenticate();
        })
        .finally(() => setLoadingUser(() => false));
    });
  };

  async function deauthenticate(logoutMessage?: string) {
    try {
      await revokeRefreshToken();
      updateUser(undefined);
      successfulToast({
        title: "You have logged out.",
        message: logoutMessage,
      });
      return true;
    } catch (err) {
      return false;
    }
  }

  function setExpirationToLocalStorage(exp: number) {
    setExpires(() => exp);
    if (typeof window !== "undefined") {
      window.localStorage.setItem("expires", exp.toString());
    }
  }

  useEffect(() => {
    const localExpires = window.localStorage.getItem("expires");
    if (localExpires) {
      const parsedInt = parseInt(localExpires);
      setExpires(isNaN(parsedInt) ? 0 : parsedInt);
    }
  }, []);

  useEffect(() => {
    const accessToken = getAccessToken();
    if (!user && accessToken) {
      fetchAuthenticatedUser();
    }
  }, [user]);

  useInterval(() => {
    if (user) {
      const now = Date.now() / 1000;
      if (expires <= now) {
        refreshAccessToken().then((result) => {
          setExpirationToLocalStorage(result.accessTokenExpires);
        });
      }
    }
  }, 1000 * 60);

  const values = useMemo<UserContextType>(
    () => ({
      user,
      updateUser,
      login: authenticate,
      logout: deauthenticate,
      isLoading: loadingUser,
      isLoggedIn: Boolean(user),
    }),
    [user, loadingUser]
  );

  return (
    <UserContext.Provider value={values}>{props.children}</UserContext.Provider>
  );
}

export default function useUser() {
  return useContext(UserContext);
}
