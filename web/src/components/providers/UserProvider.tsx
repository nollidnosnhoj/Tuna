import React, { PropsWithChildren, useMemo, useState } from "react";
import { CurrentUser } from "../../features/user/types";
import { UserContextType, UserContext } from "../../lib/contexts/UserContext";

interface UserProviderProps {
  initialUser: CurrentUser | null;
}

export function UserProvider(props: PropsWithChildren<UserProviderProps>) {
  const { initialUser, children } = props;
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
}