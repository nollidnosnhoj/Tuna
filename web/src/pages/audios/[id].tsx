import {
  Accordion,
  AccordionItem,
  AccordionButton,
  Box,
  AccordionIcon,
  AccordionPanel,
} from "@chakra-ui/react";
import { GetServerSideProps } from "next";
import React from "react";
import { QueryClient, useQuery } from "react-query";
import { useRouter } from "next/router";
import { dehydrate } from "react-query/hydration";
import Page from "~/components/Page";
import { fetchAudioById } from "~/features/audio/services/mutations/fetchAudioById";
import { getAccessToken } from "~/utils";
import AudioDetails from "~/features/audio/components/Details";
import AudioFileInfo from "~/features/audio/components/AudioFileInfo";
import { AudioDetail } from "~/features/audio/types";
import { ErrorResponse } from "~/lib/types";
import { useAuth } from "~/lib/hooks/useAuth";

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

export default function ViewAudioNextPage() {
  const { query } = useRouter();
  const id = query.id as string;
  const { accessToken } = useAuth();
  const { data: audio } = useQuery<AudioDetail, ErrorResponse>(
    ["audios", id],
    () => fetchAudioById(id, { accessToken }),
    {
      staleTime: 1000,
    }
  );

  if (!audio) return null;

  return (
    <Page title={audio.title}>
      <AudioDetails audio={audio} />
      <Accordion defaultIndex={[0]} allowMultiple>
        <AccordionItem>
          <h2>
            <AccordionButton>
              <Box flex="1" textAlign="left">
                Description
              </Box>
              <AccordionIcon />
            </AccordionButton>
          </h2>
          <AccordionPanel pb={4}>
            {audio.description || "No information given."}
          </AccordionPanel>
        </AccordionItem>
        <AccordionItem>
          <h2>
            <AccordionButton>
              <Box flex="1" textAlign="left">
                File Info
              </Box>
              <AccordionIcon />
            </AccordionButton>
          </h2>
          <AccordionPanel pb={4}>
            <AudioFileInfo
              duration={audio.duration}
              fileSize={audio.fileSize}
            />
          </AccordionPanel>
        </AccordionItem>
      </Accordion>
    </Page>
  );
}
