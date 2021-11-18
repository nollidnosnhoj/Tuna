import { Button } from "@chakra-ui/react";
import { useToast } from "@chakra-ui/toast";
import NextLink from "next/link";
import { useRouter } from "next/router";
import React, { useState } from "react";
import UserSignUpForm, {
  UserSignUpInputs,
} from "~/components/forms/signup/UserSignUpForm";
import Page from "~/components/Page";
import AuthContainer from "~/components/ui/AuthContainer";
import request from "~/lib/http";
import { getErrorMessage } from "~/utils/error";

export default function UserSignUpPage() {
  const [error, setError] = useState("");
  const toast = useToast();
  const router = useRouter();

  const handleRegister = async (values: UserSignUpInputs) => {
    try {
      await request({
        method: "post",
        url: "auth/register",
        data: {
          username: values.username,
          password: values.password,
          email: values.email,
          isArtist: false,
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
    <Page title="Sign up as User">
      <AuthContainer
        headingText="Sign Up As User"
        error={error}
        footer={
          <NextLink href="/signup/artist">
            <Button size="sm" width="full">
              Sign up as an Artist
            </Button>
          </NextLink>
        }
      >
        <UserSignUpForm onSubmit={handleRegister} />
      </AuthContainer>
    </Page>
  );
}