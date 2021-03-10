import { Box, Button, Flex, Heading } from "@chakra-ui/react";
import React, { FC } from "react";
import NextLink from "next/link";
import useUser from "~/contexts/userContext";
import Page from "../Page";

export default function withRequiredAuth(Component: FC) {
  return function RequireAuthWrapperComponent() {
    const { isLoggedIn } = useUser();

    if (isLoggedIn) return <Component />;

    return (
      <Page title="Unauthorized">
        <Flex justify="center" align="center" height="50vh">
          <Box>
            <Heading as="h2">You are not authorized.</Heading>
            <Box marginTop={10}>
              <NextLink href="/login">
                <Button width="100%">Login</Button>
              </NextLink>
            </Box>
          </Box>
        </Flex>
      </Page>
    );
  };
}
