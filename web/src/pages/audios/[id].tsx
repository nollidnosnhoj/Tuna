import React, { useMemo, useState } from "react";
import {
  Box,
  Flex,
  useDisclosure,
  Button,
  Text,
  Image,
} from "@chakra-ui/react";
import { GetServerSideProps, InferGetServerSidePropsType } from "next";
import dynamic from "next/dynamic";
import { useRouter } from "next/router";
import { QueryClient } from "react-query";
import { dehydrate } from "react-query/hydration";
import AudioDetails from "~/components/Audio/Details";
import Container from "~/components/Shared/Container";
import Page from "~/components/Shared/Page";
import AudioEdit from "~/components/Audio/Edit";
import useUser from "~/lib/contexts/user_context";
import { fetchAudioById, useAddArtwork, useAudio } from "~/lib/services/audio";
import { getAccessToken } from "~/utils/cookies";
import ImageDropzone from "~/components/Shared/ImageDropzone";
import AudioImage from "~/components/Audio/Image";

const DynamicAudioPlayer = dynamic(() => import("~/components/Audio/Player"), {
  ssr: false,
});

interface PageProps {
  isDevelopment: boolean;
}

// Fetch the audio detail and render it onto the server.
export const getServerSideProps: GetServerSideProps<PageProps> = async (
  context
) => {
  const queryClient = new QueryClient();
  const id = context.params.id as string;
  const accessToken = getAccessToken(context);

  try {
    await queryClient.fetchQuery(["audios", id], () =>
      fetchAudioById(id, { accessToken })
    );
    return {
      props: {
        dehydratedState: dehydrate(queryClient),
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

  const { data: audio } = useAudio(id, {
    staleTime: 1000,
  });

  if (!audio) {
    return (
      <Page title="Audio was not found.">
        <Text>Audio was not found.</Text>
      </Page>
    );
  }

  const {
    mutateAsync: uploadArtwork,
    isLoading: isAddingArtwork,
  } = useAddArtwork(audio.id);

  const audioUrl = useMemo<string>(() => {
    return audio
      ? props.isDevelopment && audio.artworkUrl
        ? "https://localhost:5001/uploads/" + audio.artworkUrl
        : audio.artworkUrl
      : "";
  }, [audio]);

  return (
    <Page
      title={audio.title ?? "Removed"}
      beforeContainer={
        <Container>
          <DynamicAudioPlayer audio={audio} isDev={props.isDevelopment} />
        </Container>
      }
    >
      <Flex>
        <Box flex="2">
          <Flex>
            <Box flex="1" marginRight={4}>
              <AudioImage
                name="image"
                imageData={audioUrl}
                disabled={isAddingArtwork}
                canReplace={audio.user.id === user?.id}
                onChange={async (file) => {
                  await uploadArtwork(file);
                }}
              />
            </Box>
            <Box flex="3">
              <AudioDetails
                title={audio.title ?? ""}
                description={audio.description ?? ""}
                username={audio.user?.username ?? "ERROR"}
                created={audio.created ?? ""}
              />
            </Box>
          </Flex>
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
