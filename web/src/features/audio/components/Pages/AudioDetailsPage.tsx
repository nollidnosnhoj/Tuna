import React from "react";
import { Box } from "@chakra-ui/react";
import { useRouter } from "next/router";
import AudioDetails from "~/features/audio/components/Details";
import Page from "~/components/Page";
import { useGetAudio } from "~/features/audio/hooks/queries";
import { useAudioPlayer } from "~/lib/contexts/AudioPlayerContext";

export default function AudioDetailsPage() {
  const { query } = useRouter();
  const {
    state: { currentAudio: currentPlaying },
  } = useAudioPlayer();
  const id = query.id as string;

  const { data: audio } = useGetAudio(id, {
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
