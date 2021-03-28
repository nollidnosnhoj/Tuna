import { GetServerSideProps } from "next";
import { QueryClient } from "react-query";
import { dehydrate } from "react-query/hydration";
import { getAccessToken } from "~/utils/cookies";
import { fetchAudioById } from "~/features/audio/services/mutations/fetchAudioById";
import AudioDetailsPage from "~/features/audio/components/Pages/AudioDetailsPage";

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

export default function () {
  return <AudioDetailsPage />;
}
