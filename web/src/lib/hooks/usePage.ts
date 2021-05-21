import { useContext } from "react";
import { PageContext, PageContextType } from "../contexts";

export const usePage = (): PageContextType => {
  const ctx = useContext(PageContext);
  if (!ctx) throw new Error("Cannot use PageContext");
  return ctx;
};
