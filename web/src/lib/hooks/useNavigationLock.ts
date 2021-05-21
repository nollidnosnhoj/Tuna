import { useEffect } from "react";
import { useRouter } from "next/router";

/** Ask for confirmation before changing page or leaving site.
 *
 * @see https://git.io/JOskG
 */
export const useNavigationLock = (
  isEnabled = true,
  warningText = "You have unsaved changes – are you sure you wish to leave this page?"
): void => {
  const router = useRouter();

  useEffect(() => {
    const handleWindowClose = (e: BeforeUnloadEvent): string | undefined => {
      if (!isEnabled) return;
      e.preventDefault();
      return (e.returnValue = warningText);
    };

    const handleBrowseAway = (): void => {
      if (!isEnabled) return;
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
  }, [isEnabled]);
};
