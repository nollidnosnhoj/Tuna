import { Alert, AlertIcon, AlertDescription } from "@chakra-ui/alert";
import { CloseButton } from "@chakra-ui/close-button";
import { Divider } from "@chakra-ui/react";
import { chakra } from "@chakra-ui/system";
import { useToast } from "@chakra-ui/toast";
import { useRouter } from "next/router";
import React, { useState } from "react";
import Page from "~/components/Page";
import Link from "~/components/UI/Link";
import RegisterForm, {
  RegisterFormInputs,
} from "~/features/auth/components/Forms/Register";
import request from "~/lib/http";
import { getErrorMessage } from "~/utils/error";

export default function RegisterPage() {
  const [error, setError] = useState("");
  const toast = useToast();
  const router = useRouter();

  const handleRegister = async (values: RegisterFormInputs) => {
    try {
      await request({
        method: "post",
        url: "auth/register",
        data: {
          username: values.username,
          password: values.password,
          email: values.email,
        },
      });
      router.push("/").then(() => {
        toast({
          title: "Thank you for registering.",
          description: "You can now login to your account.",
          status: "success",
        });
      });
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
          <RegisterForm onSubmit={handleRegister} />
          <Divider marginY={4} />
          <chakra.div textAlign="center">
            <chakra.span fontSize="small">
              Already have an account? Click <Link href="/login">here</Link> to
              login.
            </chakra.span>
          </chakra.div>
        </chakra.div>
      </chakra.div>
    </Page>
  );
}
