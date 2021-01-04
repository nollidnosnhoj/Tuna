import {
  Box,
  Button,
  Flex,
  Heading,
  IconButton,
  Stack,
  Text,
} from "@chakra-ui/react";
import { GetServerSideProps } from "next";
import Router from "next/router";
import Head from "next/head";
import NextLink from "next/link";
import React from "react";
import request from "~/lib/request";
import { errorToast } from "~/utils/toast";
import { FaRandom } from "react-icons/fa";
import { isAxiosError } from "~/utils";
import { User } from "~/lib/types";
import LoginModal from "~/components/Login/LoginModal";
import RegisterModal from "~/components/Register/RegisterModal";

export const getServerSideProps: GetServerSideProps = async (context) => {
  try {
    const { data } = await request<User>("me/feed", { ctx: context });
    return {
      props: { user: data },
      redirect: {
        destination: "/feed",
        permanent: false,
      },
    };
  } catch (err) {
    return {
      props: {},
    };
  }
};

const Index = () => {
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
            <Stack direction="row">
              <LoginModal buttonSize="lg" />
              <RegisterModal buttonSize="lg" />
              <IconButton
                size="lg"
                aria-label="Random audio"
                icon={<FaRandom />}
                onClick={() => goToRandomAudio()}
              />
            </Stack>
          </Flex>
        </Box>
      </Flex>
    </React.Fragment>
  );
};

export default Index;
