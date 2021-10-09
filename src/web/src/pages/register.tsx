import { Button, Stack } from "@chakra-ui/react";
import { chakra } from "@chakra-ui/system";
import { useToast } from "@chakra-ui/toast";
import NextLink from "next/link";
import { useRouter } from "next/router";
import React, { useState } from "react";
import Page from "~/components/Page";
import AuthContainer from "~/features/auth/components/AuthContainer";
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
    <Page title="Register">
      <AuthContainer
        headingText="Register"
        error={error}
        footer={
          <Stack>
            <chakra.p fontSize="small">Already have an account?</chakra.p>
            <NextLink href="/login">
              <Button>Login</Button>
            </NextLink>
          </Stack>
        }
      >
        <RegisterForm onSubmit={handleRegister} />
      </AuthContainer>
    </Page>
  );
}
