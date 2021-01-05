import React, { useEffect } from "react";
import { Box, Flex, Heading, IconButton, Stack, Text } from "@chakra-ui/react";
import Router from "next/router";
import Head from "next/head";
import { FaRandom } from "react-icons/fa";
import AuthButton from "~/components/Auth/AuthButton";
import useUser from "~/lib/contexts/user_context";
import request from "~/lib/request";
import { isAxiosError } from "~/utils";

const Index = () => {
  const { isAuth } = useUser();

  useEffect(() => {
    Router.prefetch("/feed");
    if (isAuth) Router.push("/feed");
  }, [isAuth]);

  const goToRandomAudio = async () => {
    try {
      const { data } = await request<string>("audios/random-id");
      Router.push(`/audios/${data}`);
    } catch (err) {
      if (!isAxiosError(err)) {
        console.log(err);
      }
    }
  };

  return (
    <React.Fragment>
      <Head>
        <title>Audiochan</title>
      </Head>
      <Flex justify="center" align="center" height="75vh">
        <Box textAlign="center">
          <Heading size="4xl" marginBottom={4}>
            Audiochan
          </Heading>
          <Text fontSize="4xl">Upload music and share!</Text>
          <Flex justify="center" marginTop={4}>
            {!isAuth ? (
              <Stack direction="row">
                <AuthButton authType="login" size="lg" />
                <AuthButton authType="register" size="lg" />
                <IconButton
                  size="lg"
                  aria-label="Random audio"
                  icon={<FaRandom />}
                  onClick={() => goToRandomAudio()}
                />
              </Stack>
            ) : (
              <Text>Redirecting...</Text>
            )}
          </Flex>
        </Box>
      </Flex>
    </React.Fragment>
  );
};

export default Index;
