import { Button, Box, Flex } from "@chakra-ui/react";
import { yupResolver } from "@hookform/resolvers/yup";
import { GetServerSideProps } from "next";
import Router, { useRouter } from "next/router";
import React, { useEffect, useMemo } from "react";
import { useForm } from "react-hook-form";
import * as yup from "yup";
import PageLayout from "~/components/Layout";
import useUser from "~/lib/contexts/user_context";
import { LoginFormValues } from "~/lib/types";
import InputField from "~/components/InputField";
import { apiErrorToast, successfulToast } from "~/utils/toast";
import request from "~/lib/request";

// THIS CODE CAUSES THE NEXTJS API TO STALL
export const getServerSideProps: GetServerSideProps = async (ctx) => {
  const { req } = ctx;
  try {
    const { data } = await request("me", { ctx: { req } });
    return {
      props: { user: data },
      redirect: {
        destination: "/",
        permanent: false,
      },
    };
  } catch (err) {
    return { props: {} };
  }
};

const LoginPage: React.FC = () => {
  const router = useRouter();
  const { query } = router;
  const { login, isAuth } = useUser();

  if (isAuth) {
    router.push("/");
  }

  const redirect = useMemo(
    () => decodeURIComponent((query.redirect as string) || "/"),
    [query]
  );

  useEffect(() => {
    Router.prefetch(redirect);
  }, [redirect]);

  const {
    register,
    handleSubmit,
    errors,
    formState: { isSubmitting },
  } = useForm<LoginFormValues>({
    resolver: yupResolver(
      yup.object().shape({
        username: yup.string().required(),
        password: yup.string().required(),
      })
    ),
  });

  const onSubmit = async (values: LoginFormValues) => {
    try {
      await login(values);
      Router.push(redirect);
    } catch (err) {
      apiErrorToast(err);
    }
  };

  return (
    <PageLayout title="Login">
      <Flex justify="center">
        <Box width={{ base: "100%", md: "50%" }}>
          <form onSubmit={handleSubmit(onSubmit)}>
            <InputField
              name="username"
              label="Username/Email"
              ref={register}
              error={errors.username}
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
            <Button type="submit" mt={4} isLoading={isSubmitting}>
              Login
            </Button>
          </form>
        </Box>
      </Flex>
    </PageLayout>
  );
};

export default LoginPage;
