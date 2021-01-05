import { Text } from "@chakra-ui/react";
import Router, { useRouter } from "next/router";
import React, { useEffect } from "react";
import useUser from "~/lib/contexts/user_context";

const AuthRequired: React.FC = ({ children, ...props }) => {
  const { isAuth } = useUser();
  const { asPath } = useRouter();

  useEffect(() => {
    if (!isAuth) {
      Router.push(`/login?redirect=${asPath as string}`);
    }
  }, [isAuth, asPath]);

  if (!isAuth) {
    return (
      <Text>
        You are not authorized to view this page. Redirect to login page...
      </Text>
    );
  }

  return <>{children}</>;
};

export default AuthRequired;
