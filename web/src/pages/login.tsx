import { Box, Flex } from "@chakra-ui/react";
import Router, { useRouter } from "next/router";
import React, { useEffect, useMemo } from "react";
import LoginForm from "~/components/Auth/LoginForm";
import Page from "~/components/Layout";
import useUser from "~/lib/contexts/user_context";

export default function LoginPage() {
  const { query } = useRouter();
  const { isAuth } = useUser();

  const redirect = useMemo<string>(() => {
    return decodeURIComponent((query.redirect as string) || "/feed");
  }, [query]);

  useEffect(() => {
    if (isAuth) {
      Router.push(redirect);
    }
  }, [isAuth]);

  useEffect(() => {
    Router.prefetch(redirect);
  }, [redirect]);

  return (
    <Page title="Login">
      <Flex justify="center">
        <Box width="500px">
          <LoginForm
            onSuccess={() => {
              Router.push(redirect);
            }}
          />
        </Box>
      </Flex>
    </Page>
  );
}
