import { GetServerSideProps } from "next";
import AudioSearchPage, {
  AudioSearchValues,
} from "~/features/audio/components/Pages/AudioSearchPage";

export const getServerSideProps: GetServerSideProps<AudioSearchValues> = async ({
  query,
}) => {
  const searchTermQuery = query["q"] ?? "";
  const sortQuery = query["sort"] ?? "";
  const tagsQuery = query["tag"] ?? "";

  const q = Array.isArray(searchTermQuery)
    ? searchTermQuery[0]
    : searchTermQuery;
  const sort = Array.isArray(sortQuery) ? sortQuery[0] : sortQuery;
  const tags = Array.isArray(tagsQuery) ? tagsQuery : tagsQuery.split(",");

  return {
    props: {
      q,
      sort,
      tags,
    },
  };
};

export default function (props: AudioSearchValues) {
  return <AudioSearchPage {...props} />;
}
