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
import { useRouter } from "next/router";
import Page from "~/components/Page";
import AudioDetails from "~/features/audio/components/Details";
import AudioFileInfo from "~/features/audio/components/Details/AudioFileInfo";
import { useGetAudio } from "~/features/audio/hooks";
import { fetchAudioHandler } from "~/features/audio/api";
import { AudioDetailData } from "~/features/audio/types";

interface AudioPageProps {
  audio?: AudioDetailData;
}

export const getServerSideProps: GetServerSideProps<AudioPageProps> = async (
  context
) => {
  const id = context.params?.id as string;

  try {
    const data = await fetchAudioHandler(id, context);
    return {
      props: {
        audio: data,
      },
    };
  } catch (err) {
    return {
      notFound: true,
    };
  }
};

export default function ViewAudioNextPage(props: AudioPageProps) {
  const { query } = useRouter();
  const id = query.id as string;
  const { data: audio } = useGetAudio(id, {
    staleTime: 1000,
    initialData: props.audio,
  });

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
