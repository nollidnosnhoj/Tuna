import React, { createContext, useContext, useMemo, useState } from "react";
import { CurrentUser } from "../features/user/types";

type UserContextType = {
  user: CurrentUser | null;
  updateUser: (updatedUser: CurrentUser | null) => void;
};

export const UserContext = createContext<UserContextType>(
  {} as UserContextType
);

interface UserProviderProps {
  initialUser: CurrentUser | null;
}

export const UserProvider: React.FC<UserProviderProps> = ({
  initialUser,
  children,
}) => {
  const [user, setUser] = useState<CurrentUser | null>(initialUser);

  const updateUser = (updatedUser: CurrentUser | null) => {
    setUser(updatedUser);
  };

  const values = useMemo<UserContextType>(
    () => ({
      user,
      updateUser,
    }),
    [user, updateUser]
  );

  return <UserContext.Provider value={values}>{children}</UserContext.Provider>;
};

export const useUser = () => {
  return useContext(UserContext);
};
