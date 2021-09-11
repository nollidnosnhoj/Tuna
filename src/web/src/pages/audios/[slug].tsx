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
import { useGetAudio } from "~/features/audio/api/hooks";
import { AudioView } from "~/features/audio/api/types";
import AudioTags from "~/features/audio/components/Details/Tags";
import request from "~/lib/http";

interface AudioPageProps {
  audio: AudioView;
  slug: string;
}

export const getServerSideProps: GetServerSideProps<AudioPageProps> = async (
  context
) => {
  const slug = context.params?.slug as string;
  const { req, res } = context;
  try {
    const { data } = await request<AudioView>({
      method: "get",
      url: `audios/${slug}`,
      req,
      res,
    });
    return {
      props: {
        audio: data,
        slug,
      },
    };
  } catch (err) {
    return {
      notFound: true,
    };
  }
};

export default function AudioPage({ audio: initAudio, slug }: AudioPageProps) {
  const { data: audio } = useGetAudio(slug, {
    staleTime: 1000,
    initialData: initAudio,
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
