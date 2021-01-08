import React, {
  createContext,
  PropsWithChildren,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import Router from "next/router";
import { LoginFormValues } from "~/components/Auth/LoginForm";
import fetcher from "../fetcher";
import { login, revokeRefreshToken } from "../services/auth";
import { User } from "../types";
import { getAccessToken } from "~/utils/cookies";
import { successfulToast } from "~/utils/toast";

type UserContextType = {
  isAuth: boolean;
  user: User;
  login: (inputs: LoginFormValues) => Promise<void>;
  logout: () => Promise<void>;
  updateUser: (updatedUser: User) => void;
  isLoading: boolean;
};

const UserContext = createContext<UserContextType>(null);

interface UserProviderProps {
  initialUser?: User;
}

export function UserProvider(props: PropsWithChildren<UserProviderProps>) {
  const [user, setUser] = useState<User>(props.initialUser);
  const [loading, setLoading] = useState(false);

  async function authenticate(inputs: LoginFormValues) {
    try {
      await login(inputs);
      await fetchAuthenticatedUser();
    } catch (err) {}
  }

  const updateUser = (updatedUser: User) => {
    setUser(updatedUser);
  };

  const fetchAuthenticatedUser = async () => {
    try {
      setLoading(true);
      const fetchedUser = await fetcher<User>("me");
      setUser(fetchedUser);
    } catch (err) {
    } finally {
      setLoading(false);
    }
  };

  async function deauthenticate() {
    try {
      await revokeRefreshToken();
      updateUser(null);
      successfulToast({ message: "You have successfully logged out." });
    } catch (err) {
      console.error(err);
    }
    Router.reload();
  }

  useEffect(() => {
    const accessToken = getAccessToken();
    if (!user && accessToken) {
      fetchAuthenticatedUser();
    }
  }, [user]);

  const values = useMemo<UserContextType>(
    () => ({
      user,
      updateUser,
      login: authenticate,
      logout: deauthenticate,
      isLoading: loading,
      isAuth: !!user,
    }),
    [user, loading]
  );

  return (
    <UserContext.Provider value={values}>{props.children}</UserContext.Provider>
  );
}

export default function useUser() {
  return useContext(UserContext);
}
