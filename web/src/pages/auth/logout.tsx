import { chakra } from "@chakra-ui/react";
import { useEffect } from "react";
import Router from "next/router";
import Page from "~/components/Page";
import { useAuth } from "~/features/auth/hooks";
import { toast } from "~/utils/toast";

const LogoutPage: React.FC = () => {
  const { logout } = useAuth();

  useEffect(() => {
    logout()
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
