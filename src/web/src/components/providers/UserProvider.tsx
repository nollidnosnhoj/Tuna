import React, {
  createContext,
  PropsWithChildren,
  useContext,
  useMemo,
  useState,
} from "react";
import { useGetCurrentUser } from "../../lib/hooks/api/queries/useGetCurrentUser";
import { CurrentUser } from "~/lib/types";

interface UserProviderProps {
  initialUser: CurrentUser | null;
}

export type UserContextType = {
  isLoadingUser: boolean;
  isLoggedIn: boolean;
  user: CurrentUser | null;
  updateUser: (updatedUser: CurrentUser | null) => void;
  refreshUser: () => Promise<void>;
};

export const UserContext = createContext<UserContextType>(
  {} as UserContextType
);

export function UserProvider(props: PropsWithChildren<UserProviderProps>) {
  const { initialUser, children } = props;
  const [user, setUser] = useState<CurrentUser | null>(initialUser);

  const { isLoading, refetch } = useGetCurrentUser({
    enabled: !user,
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

export function useUser(): UserContextType {
  const context = useContext(UserContext);
  if (!context) throw new Error("Cannot find UserContext.");
  return context;
}
