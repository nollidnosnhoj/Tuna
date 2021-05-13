import { GetServerSideProps } from "next";
import React from "react";
import { QueryClient } from "react-query";
import { useRouter } from "next/router";
import { dehydrate } from "react-query/hydration";
import Page from "~/components/Page";
import { fetchAudioById } from "~/features/audio/services/mutations/fetchAudioById";
import { getAccessToken } from "~/utils";
import { useGetAudio } from "~/features/audio/hooks";
import AudioDetails from "~/features/audio/components/Details";

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

  const { data: audio } = useGetAudio(id, {
    staleTime: 1000,
  });

  if (!audio) return null;

  return (
    <Page title={audio.title}>
      <AudioDetails audio={audio} />
    </Page>
  );
}
