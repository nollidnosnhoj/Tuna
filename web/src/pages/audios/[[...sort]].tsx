import { GetServerSideProps } from "next";
import { useRouter } from "next/router";
import React, { useCallback, useState } from "react";
import queryString from "query-string";
import Page from "~/components/Page";
import AudioListSubHeader from "~/features/audio/components/ListSubheader";
import AudioList from "~/features/audio/components/List";
import { useAudiosInfinite } from "~/features/audio/hooks/queries";
import InfiniteListControls from "~/components/List/InfiniteListControls";
import { fetchPages } from "~/utils/api";
import { getAccessToken } from "~/utils/cookies";
import { PagedList } from "~/lib/types";
import { Audio } from "~/features/audio/types";

type SortState = "latest";

const sortPageTitles: { [K in SortState]: string } = {
  latest: "Latest Audios",
};

interface AudioListPageProps {
  sort: SortState;
  filter: Record<string, string | string[] | undefined>;
  initialPage: PagedList<Audio>;
}

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

interface AudioListFilter {}

export default function AudioListPage(props: AudioListPageProps) {
  const router = useRouter();
  const { sort, filter, initialPage } = props;

  const [listFilter, setListFilter] = useState<AudioListFilter>({});

  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useAudiosInfinite("audios", { ...listFilter, sort }, undefined, {
    staleTime: 1000, // Prevent double fetching
    initialData: {
      pages: [initialPage],
      pageParams: [1],
    },
  });

  const handleChange = useCallback((newFilter: AudioListFilter) => {
    const mergedFilter = { ...listFilter, ...newFilter };
    router.replace(
      `/audios?${queryString.stringify(mergedFilter)}`,
      undefined,
      {
        shallow: true,
      }
    );
    setListFilter(mergedFilter);
  }, []);

  return (
    <Page
      title={sortPageTitles[sort]}
      beforeContainer={<AudioListSubHeader current={sort} />}
    >
      <AudioList audios={audios} />
      <InfiniteListControls
        fetchNext={fetchNextPage}
        hasNext={hasNextPage}
        isFetching={isFetchingNextPage}
      />
    </Page>
  );
}
