import { Button } from "@chakra-ui/react";
import { useToast } from "@chakra-ui/toast";
import NextLink from "next/link";
import { useRouter } from "next/router";
import React, { useState } from "react";
import ArtistSignUpForm, {
  ArtistSignUpInputs,
} from "~/components/forms/signup/ArtistSignUpForm";
import Page from "~/components/Page";
import AuthContainer from "~/components/ui/AuthContainer";
import request from "~/lib/http";
import { getErrorMessage } from "~/utils/error";

export default function ArtistSignUpPage() {
  const [error, setError] = useState("");
  const toast = useToast();
  const router = useRouter();

  const handleRegister = async (values: ArtistSignUpInputs) => {
    try {
      await request({
        method: "post",
        url: "auth/register",
        data: {
          username: values.username,
          password: values.password,
          email: values.email,
          displayName: values.displayName,
          isArtist: true,
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
    <Page title="Sign up as Artist">
      <AuthContainer
        headingText="Sign Up As Artist"
        error={error}
        footer={
          <NextLink href="/signup/user">
            <Button size="sm" width="full">
              Sign up as a User
            </Button>
          </NextLink>
        }
      >
        <ArtistSignUpForm onSubmit={handleRegister} />
      </AuthContainer>
    </Page>
  );
}
