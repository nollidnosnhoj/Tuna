import { GetServerSideProps } from "next";
import AudioFeedPage, {
  AudioFeedPageProps,
} from "~/features/audio/components/Pages/AudioFeedPage";
import { Audio } from "~/features/audio/types";
import { fetchPages } from "~/lib/api";
import { getAccessToken } from "~/utils";

export const getServerSideProps: GetServerSideProps<AudioFeedPageProps> = async ({
  query,
  req,
}) => {
  const accessToken = getAccessToken({ req });

  const resultPage = await fetchPages<Audio>("me/feed", query, 1, {
    accessToken,
  });

  return {
    props: {
      filter: query,
      initialPage: resultPage,
    },
  };
};

export default function UserAudioFeedNextPage(props: AudioFeedPageProps) {
  return <AudioFeedPage {...props} />;
}
