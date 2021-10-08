import { Alert, AlertIcon, AlertDescription } from "@chakra-ui/alert";
import { CloseButton } from "@chakra-ui/close-button";
import { Divider } from "@chakra-ui/react";
import { chakra } from "@chakra-ui/system";
import { useToast } from "@chakra-ui/toast";
import { useRouter } from "next/router";
import React, { useState } from "react";
import Page from "~/components/Page";
import Link from "~/components/UI/Link";
import { useLogin } from "~/features/auth/api/hooks";
import LoginForm, {
  LoginFormValues,
} from "~/features/auth/components/Forms/Login";
import { getErrorMessage } from "~/utils/error";

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
      router.push(redirectUrl);
    } catch (err) {
      setError(getErrorMessage(err));
    }
  };

  return (
    <Page title="Login">
      <chakra.div
        display="flex"
        justifyContent="center"
        alignItems="center"
        height="50vh"
      >
        <chakra.div maxWidth="500px" width="80%">
          {!!error && (
            <Alert status="error">
              <AlertIcon />
              <AlertDescription>{error}</AlertDescription>
              <CloseButton
                onClick={() => setError("")}
                position="absolute"
                right="8px"
                top="8px"
              />
            </Alert>
          )}
          <LoginForm onSubmit={handleLogin} />
          <Divider marginY={4} />
          <chakra.div textAlign="center">
            <chakra.span fontSize="small">
              New here? Click <Link href="/register">here</Link> to register.
            </chakra.span>
          </chakra.div>
        </chakra.div>
      </chakra.div>
    </Page>
  );
}
