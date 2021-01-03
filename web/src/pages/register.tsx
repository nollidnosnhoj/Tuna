import { Button, Box, Flex } from "@chakra-ui/react";
import { yupResolver } from "@hookform/resolvers/yup";
import { GetServerSideProps } from "next";
import Router, { useRouter } from "next/router";
import React, { useEffect, useMemo, useState } from "react";
import { useForm } from "react-hook-form";
import * as yup from "yup";
import PageLayout from "~/components/Layout";
import request from "~/lib/request";
import InputField from "~/components/InputField";
import { passwordRule, usernameRule } from "~/utils/validators";
import { validationMessages } from "~/utils";
import { apiErrorToast, successfulToast } from "~/utils/toast";
import useUser from "~/lib/contexts/user_context";

type RegisterFormInputs = {
  username: string;
  password: string;
  email: string;
  confirmPassword: string;
};

const defaultValues: RegisterFormInputs = {
  username: "",
  password: "",
  email: "",
  confirmPassword: "",
};

//// THIS CODE CAUSES THE NEXTJS API TO STALL
// export const getServerSideProps: GetServerSideProps = async (ctx) => {
//   const { res } = ctx;
//   try {
//     await request("me", { method: "head", ctx });
//     res.writeHead(302, {
//       Location: "/",
//     });
//     res.end();
//   } catch (err) {}

//   return { props: {} };
// };

const RegisterPage: React.FC = () => {
  const { query } = useRouter();
  const { isAuth } = useUser();

  if (isAuth) {
    Router.push("/");
  }

  const redirect = useMemo(
    () => decodeURIComponent((query.redirect as string) || "/"),
    [query]
  );

  useEffect(() => {
    Router.prefetch(redirect);
  }, [redirect]);

  const { register, handleSubmit, errors } = useForm<RegisterFormInputs>({
    defaultValues: defaultValues,
    resolver: yupResolver(
      yup.object().shape({
        username: usernameRule("Username"),
        password: passwordRule("Password"),
        email: yup
          .string()
          .required(validationMessages.required("Email"))
          .email(),
        confirmPassword: yup
          .string()
          .oneOf([yup.ref("password")], "Password does not match."),
      })
    ),
  });

  const [isSubmitting, setSubmitting] = useState(false);

  const onSubmit = async (values: RegisterFormInputs) => {
    setSubmitting(true);

    const registrationRequest = {
      username: values.username,
      password: values.password,
      email: values.email,
    };

    try {
      await request("/auth/register", {
        method: "post",
        body: registrationRequest,
      });

      successfulToast({
        title: "Thank you for registering.",
        message: "You can now login to your account.",
      });

      Router.push(redirect);
    } catch (err) {
      apiErrorToast(err);
    }

    setSubmitting(false);
  };

  return (
    <PageLayout title="Login">
      <Flex justify="center">
        <Box width={{ base: "100%", md: "50%" }}>
          <form onSubmit={handleSubmit(onSubmit)}>
            <InputField
              name="username"
              label="Username"
              ref={register}
              error={errors.username}
              isRequired
            />
            <InputField
              name="email"
              label="Email"
              ref={register}
              error={errors.email}
              isRequired
            />
            <InputField
              name="password"
              type="password"
              label="Password"
              ref={register}
              error={errors.password}
              isRequired
            />
            <InputField
              name="password"
              type="password"
              label="Confirm Password"
              ref={register}
              error={errors.confirmPassword}
              isRequired
            />
            <Button type="submit" mt={4} isLoading={isSubmitting}>
              Login
            </Button>
          </form>
        </Box>
      </Flex>
    </PageLayout>
  );
};

export default RegisterPage;
