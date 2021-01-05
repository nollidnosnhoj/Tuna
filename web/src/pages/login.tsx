import { Box, Flex } from "@chakra-ui/react";
import { useRouter } from "next/router";
import React, { useEffect, useMemo } from "react";
import LoginForm from "~/components/Auth/LoginForm";
import PageLayout from "~/components/Layout";
import useUser from "~/lib/contexts/user_context";

export default function LoginPage() {
  const router = useRouter();
  const { query } = router;
  const { isAuth } = useUser();

  const redirect = useMemo<string>(() => {
    return decodeURIComponent(query.redirect as string);
  }, [query]);

  useEffect(() => {
    router.prefetch(redirect);
  }, [router]);

  useEffect(() => {
    if (isAuth) {
      router.push("/");
    }
  }, [router, isAuth]);

  return (
    <PageLayout title="Login">
      <Flex justify="center">
        <Box width="500px">
          <LoginForm
            onSuccess={() => {
              router.push(redirect);
            }}
          />
        </Box>
      </Flex>
    </PageLayout>
  );
}
