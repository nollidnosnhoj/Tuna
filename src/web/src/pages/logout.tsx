import { chakra } from "@chakra-ui/react";
import { useEffect } from "react";
import Router from "next/router";
import Page from "~/components/Page";
import { toast } from "~/utils/toast";
import { useLogout } from "~/features/auth/api/hooks";

const LogoutPage: React.FC = () => {
  const { mutateAsync: logoutAsync } = useLogout();
  useEffect(() => {
    logoutAsync()
      .then(() => {
        Router.push("/").then(() => {
          toast("success", {
            title: "You have successfully logged out.",
          });
        });
      })
      .catch((err) => console.error(err));
  }, []);

  return (
    <Page title="Logging out" requiresAuth>
      <chakra.span>Logging out...</chakra.span>
    </Page>
  );
};

export default LogoutPage;
