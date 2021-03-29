import { GetServerSideProps } from "next";
import { fetchPages } from "~/utils/api";
import { getAccessToken } from "~/utils/cookies";
import { Audio } from "~/features/audio/types";
import AudioListPage, {
  AudioListPageProps,
  SortState,
} from "../../features/audio/components/Pages/AudioListPage";

export const getServerSideProps: GetServerSideProps<AudioListPageProps> = async ({
  query,
  params,
  req,
}) => {
  const accessToken = getAccessToken({ req });
  let sort: SortState;

  let sortParam = params?.sort || "latest";

  if (Array.isArray(sortParam)) {
    sortParam = sortParam[0].toLowerCase();
  } else {
    sortParam = sortParam.toLowerCase();
  }

  switch (sortParam) {
    case "latest":
      sort = "latest";
      break;
    default:
      return {
        notFound: true,
      };
  }

  const { page, ...filter } = query;

  const resultPage = await fetchPages<Audio>("audios", { ...filter, sort }, 1, {
    accessToken,
  });

  return {
    props: {
      sort: sort,
      filter: filter,
      initialPage: resultPage,
    },
  };
};

export default function BrowseAudioNextPage(props: AudioListPageProps) {
  return <AudioListPage {...props} />;
}
