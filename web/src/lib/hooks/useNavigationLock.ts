import React, { SetStateAction, useEffect, useState } from "react";
import { useRouter } from "next/router";

/** Ask for confirmation before changing page or leaving site.
 *
 * @see https://git.io/JOskG
 */
export const useNavigationLock = (
  warningText = "You have unsaved changes – are you sure you wish to leave this page?"
): [boolean, React.Dispatch<SetStateAction<boolean>>] => {
  const router = useRouter();
  const [unsaved, setUnsaved] = useState(false);

  useEffect(() => {
    const handleWindowClose = (e: BeforeUnloadEvent): string | undefined => {
      if (!unsaved) return;
      e.preventDefault();
      return (e.returnValue = warningText);
    };

    const handleBrowseAway = (): void => {
      if (!unsaved) return;
      if (window.confirm(warningText)) return;
      router.events.emit("routeChangeError");
      throw "routeChange aborted.";
    };

    window.addEventListener("beforeunload", handleWindowClose);

    router.events.on("routeChangeStart", handleBrowseAway);

    return () => {
      window.removeEventListener("beforeunload", handleWindowClose);
      router.events.off("routeChangeStart", handleBrowseAway);
    };
  }, [unsaved]);

  return [unsaved, setUnsaved];
};
