import React from "react";
import { Box, Flex } from "@chakra-ui/react";
import { GetServerSideProps, InferGetServerSidePropsType } from "next";
import { useRouter } from "next/router";
import { QueryClient } from "react-query";
import { dehydrate } from "react-query/hydration";
import AudioDetails from "~/features/audio/components/Details";
import Page from "~/components/Page";
import { getAccessToken } from "~/utils/cookies";
import { useAudio } from "~/features/audio/hooks/queries";
import { fetchAudioById } from "~/features/audio/services/fetch";

// Fetch the audio detail and render it onto the server.
export const getServerSideProps: GetServerSideProps = async (context) => {
  const queryClient = new QueryClient();
  const id = context.params?.id as string;
  const accessToken = getAccessToken(context);

  try {
    await queryClient.fetchQuery(["audios", id], () =>
      fetchAudioById(id, { accessToken })
    );
    return {
      props: {
        dehydratedState: dehydrate(queryClient),
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
  const { query } = useRouter();
  const id = query.id as string;

  const { data: audio } = useAudio(id, {
    staleTime: 1000,
  });

  if (!audio) return null;

  return (
    <Page title={audio.title}>
      <Box>
        <AudioDetails audio={audio} />
      </Box>
    </Page>
  );
}
