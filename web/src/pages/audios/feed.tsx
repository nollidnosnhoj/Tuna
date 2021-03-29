import { GetServerSideProps } from "next";
import AudioFeedPage, {
  AudioFeedPageProps,
} from "~/features/audio/components/Pages/AudioFeedPage";
import { Audio } from "~/features/audio/types";
import { fetchPages } from "~/utils/api";
import { getAccessToken } from "~/utils/cookies";

export const getServerSideProps: GetServerSideProps<AudioFeedPageProps> = async ({
  query,
  req,
}) => {
  const accessToken = getAccessToken({ req });
  const { page, ...filter } = query;

  const resultPage = await fetchPages<Audio>("me/feed", filter, 1, {
    accessToken,
  });

  return {
    props: {
      filter: filter,
      initialPage: resultPage,
    },
  };
};

export default function UserAudioFeedNextPage(props: AudioFeedPageProps) {
  return <AudioFeedPage {...props} />;
}
