import { chakra } from "@chakra-ui/react";
import { useEffect } from "react";
import Router from "next/router";
import Page from "~/components/Page";
import { toast } from "~/utils/toast";
import { revokeRefreshTokenRequest } from "~/features/auth/api";
import { useUser } from "~/features/user/hooks";

const LogoutPage: React.FC = () => {
  const { updateUser } = useUser();
  useEffect(() => {
    revokeRefreshTokenRequest()
      .then(() => {
        updateUser(null);
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
