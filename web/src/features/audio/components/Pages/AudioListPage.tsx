import { useRouter } from "next/router";
import React, { useCallback, useState } from "react";
import queryString from "query-string";
import Page from "~/components/Page";
import AudioListSubHeader from "~/features/audio/components/ListSubheader";
import AudioList from "~/features/audio/components/List";
import { Audio } from "~/features/audio/types";
import { useGetAudioListInfinite } from "~/features/audio/hooks/queries/useAudiosInfinite";
import InfiniteListControls from "~/components/List/InfiniteListControls";
import { PagedList } from "~/lib/types";

export type SortState = "latest";

const sortPageTitles: { [K in SortState]: string } = {
  latest: "Latest Audios",
};

export interface AudioListPageProps {
  sort: SortState;
  filter: Record<string, string | string[] | undefined>;
  initialPage?: PagedList<Audio>;
}

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
  } = useGetAudioListInfinite("audios", { ...listFilter, sort }, undefined, {
    staleTime: 1000,
    initialData: initialPage && {
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
