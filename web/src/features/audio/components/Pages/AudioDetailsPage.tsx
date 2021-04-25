import React from "react";
import { Box } from "@chakra-ui/react";
import { useRouter } from "next/router";
import dynamic from "next/dynamic";
import AudioDetails from "~/features/audio/components/Details";
import Page from "~/components/Page";
import { useGetAudio } from "~/features/audio/hooks/queries";
import { useAudioPlayer } from "~/contexts/AudioPlayerContext";

const WavesurferPlayer = dynamic(() => import("~/components/Waveform"), {
  ssr: false,
});

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
        <WavesurferPlayer audio={audio} />
        <AudioDetails audio={audio} />
      </Box>
    </Page>
  );
}
