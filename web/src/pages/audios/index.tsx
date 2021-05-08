import { GetServerSideProps } from "next";
import { fetch } from "~/utils/api";
import { getAccessToken } from "~/utils/cookies";
import { Audio } from "~/features/audio/types";
import AudioListPage, {
  AudioListPageProps,
} from "../../features/audio/components/Pages/AudioListPage";
import { CursorPagedList } from "~/lib/types";

export const getServerSideProps: GetServerSideProps<AudioListPageProps> = async ({
  query,
  req,
}) => {
  const accessToken = getAccessToken({ req });
  const resultPage = await fetch<CursorPagedList<Audio>>("audios", query, {
    accessToken,
  });

  return {
    props: {
      initialPage: resultPage,
    },
  };
};

export default function BrowseAudioNextPage(props: AudioListPageProps) {
  return <AudioListPage {...props} />;
}
