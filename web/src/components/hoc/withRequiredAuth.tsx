import { Box, Button, Flex, Heading } from "@chakra-ui/react";
import React, { FC } from "react";
import NextLink from "next/link";
import { useAuth } from "~/lib/hooks/useAuth";
import Page from "../Page";

export default function withRequiredAuth(
  Component: FC,
  initialLoggedIn?: boolean
) {
  if (initialLoggedIn) return () => <Component />;

  return function RequireAuthWrapperComponent() {
    const { isLoggedIn } = useAuth();

    if (isLoggedIn) return <Component />;

    return (
      <Page title="Unauthorized">
        <Flex justify="center" align="center" height="50vh">
          <Box>
            <Heading as="h2">You are not authorized.</Heading>
            <Box marginTop={10}>
              <NextLink href="/auth/login">
                <Button width="100%">Login</Button>
              </NextLink>
            </Box>
          </Box>
        </Flex>
      </Page>
    );
  };
}
