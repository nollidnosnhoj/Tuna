import { createContext } from "react";

export type PageContextType = {
  openLogin: () => void;
  openRegister: () => void;
};

export const PageContext = createContext<PageContextType>(
  {} as PageContextType
);
