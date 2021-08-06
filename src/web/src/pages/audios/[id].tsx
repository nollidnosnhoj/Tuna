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
import Page from "~/components/Page";
import AudioDetails from "~/features/audio/components/Details";
import AudioFileInfo from "~/features/audio/components/Details/FileInfo";
import { useGetAudio } from "~/features/audio/hooks";
import { getAudioRequest } from "~/features/audio/api";
import { AudioView, AudioId } from "~/features/audio/api/types";
import AudioTags from "~/features/audio/components/Details/Tags";

interface AudioPageProps {
  audio?: AudioView;
  audioId: AudioId;
}

export const getServerSideProps: GetServerSideProps<AudioPageProps> = async (
  context
) => {
  const id = context.params?.id as AudioId;

  try {
    const data = await getAudioRequest(id, context);
    return {
      props: {
        audio: data,
        audioId: id,
      },
    };
  } catch (err) {
    return {
      notFound: true,
    };
  }
};

export default function AudioPage(props: AudioPageProps) {
  const { data: audio } = useGetAudio(props.audioId, {
    staleTime: 1000,
    initialData: props.audio,
  });

  if (!audio) return null;

  return (
    <Page title={audio.title}>
      <AudioDetails audio={audio} />
      <Accordion defaultIndex={[0]} allowMultiple>
        {audio.tags && audio.tags.length > 0 && (
          <AccordionItem>
            <h2>
              <AccordionButton>
                <Box flex="1" textAlign="left">
                  Tags
                </Box>
              </AccordionButton>
            </h2>
            <AccordionPanel pb={4}>
              <AudioTags tags={audio.tags} />
            </AccordionPanel>
          </AccordionItem>
        )}
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
            <AudioFileInfo duration={audio.duration} fileSize={audio.size} />
          </AccordionPanel>
        </AccordionItem>
      </Accordion>
    </Page>
  );
}
