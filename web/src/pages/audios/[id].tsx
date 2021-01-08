import React from "react";
import { Box, Flex, useDisclosure, Button } from "@chakra-ui/react";
import { GetServerSideProps, InferGetServerSidePropsType } from "next";
import dynamic from "next/dynamic";
import { useRouter } from "next/router";
import AudioDetails from "~/components/Audio/Details";
import Container from "~/components/Container";
import Page from "~/components/Layout";
import AudioEdit from "~/components/Audio/Edit";
import useUser from "~/lib/contexts/user_context";
import { Audio } from "~/lib/types";
import request from "~/lib/request";
import { useAudio, useFavorite } from "~/lib/services/audio";
import { getCookie } from "~/utils/cookies";
import { ACCESS_TOKEN_KEY } from "~/constants";

const DynamicAudioPlayer = dynamic(() => import("~/components/Audio/Player"), {
  ssr: false,
});

interface PageProps {
  initialData: Audio;
  isDevelopment: boolean;
}

// Fetch the audio detail and render it onto the server.
export const getServerSideProps: GetServerSideProps<PageProps> = async (
  context
) => {
  const id = context.params.id as string;
  const accessToken = getCookie(ACCESS_TOKEN_KEY, context);

  try {
    const { data } = await request<Audio>(`audios/${id}`, {
      accessToken: accessToken,
    });

    return {
      props: {
        initialData: data,
        isDevelopment: process.env.NODE_ENV === "development",
      },
    };
  } catch (err) {
    return {
      notFound: true,
    };
  }
};

export default function AudioDetailsPage(
  props: InferGetServerSidePropsType<typeof getServerSideProps>
) {
  const { user, isAuth } = useUser();
  const { query } = useRouter();
  const id = query.id as string;

  const {
    isOpen: isEditOpen,
    onOpen: onEditOpen,
    onClose: onEditClose,
  } = useDisclosure();

  const { data: audio } = useAudio(id, props.initialData);

  const { isFavorite, favorite } = useFavorite(id);

  // get
  const audioUrl = props.isDevelopment
    ? "https://localhost:5001/uploads/" + audio?.url
    : audio?.url;

  return (
    <Page
      title={audio.title ?? "Removed"}
      beforeContainer={
        <Container>
          <DynamicAudioPlayer url={audioUrl} />
        </Container>
      }
    >
      <Flex>
        <Box flex="2">
          <AudioDetails
            title={audio.title ?? ""}
            description={audio.description ?? ""}
            username={audio.user?.username ?? "ERROR"}
            created={audio.created ?? ""}
          />
        </Box>
        <Box flex="1">
          {isAuth && user?.id === audio.user?.id && (
            <Button width="100%" onClick={onEditOpen} colorScheme="primary">
              Edit
            </Button>
          )}
        </Box>
      </Flex>
      <AudioEdit model={audio} isOpen={isEditOpen} onClose={onEditClose} />
    </Page>
  );
}
