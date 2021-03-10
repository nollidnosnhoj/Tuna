import { Box, Flex } from "@chakra-ui/react";
import { useRouter } from "next/router";
import React, { useCallback, useMemo } from "react";
import Page from "~/components/Page";
import useUser from "~/contexts/userContext";
import LoginForm from "~/features/auth/components/LoginForm";

export default function LoginPage() {
  const router = useRouter();
  const { query } = router;

  const redirect = useMemo(() => {
    return (query.redirect as string) || "/";
  }, [query]);

  const { isLoggedIn } = useUser();

  const onRedirect = useCallback(() => {
    router.push(redirect || "/");
  }, [router, redirect]);

  if (isLoggedIn) {
    onRedirect();
  }

  return (
    <Page title="Login">
      <Flex justify="center" marginTop={50}>
        <Box width="550px">
          <LoginForm onSuccess={onRedirect} />
        </Box>
      </Flex>
    </Page>
  );
}
