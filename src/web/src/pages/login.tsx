import { Button } from "@chakra-ui/button";
import { Stack } from "@chakra-ui/layout";
import { chakra } from "@chakra-ui/system";
import { useToast } from "@chakra-ui/toast";
import NextLink from "next/link";
import { useRouter } from "next/router";
import React, { useState } from "react";
import Page from "~/components/Page";
import AuthContainer from "~/components/ui/AuthContainer";
import LoginForm, { LoginFormValues } from "~/components/forms/LoginForm";
import { getErrorMessage } from "~/utils/error";
import { useLogin } from "~/lib/hooks/api";

export default function LoginPage() {
  const [error, setError] = useState("");
  const toast = useToast();
  const router = useRouter();
  const { mutateAsync: loginAsync } = useLogin();
  const redirectUrl = (router.query["redirecturl"] as string) || "/";

  const handleLogin = async (values: LoginFormValues) => {
    try {
      await loginAsync(values);
      toast({
        status: "success",
        description: "You have logged in successfully. ",
      });
      await router.push(redirectUrl);
    } catch (err) {
      setError(getErrorMessage(err));
    }
  };

  return (
    <Page title="Login">
      <AuthContainer
        headingText="Login"
        error={error}
        footer={
          <Stack>
            <chakra.p fontSize="small">New here?</chakra.p>
            <NextLink href="/signup/user">
              <Button>Create an account</Button>
            </NextLink>
          </Stack>
        }
      >
        <LoginForm onSubmit={handleLogin} />
      </AuthContainer>
    </Page>
  );
}
