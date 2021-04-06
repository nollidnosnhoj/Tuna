import { useRouter } from "next/router";
import React, { useCallback, useState } from "react";
import queryString from "query-string";
import Page from "~/components/Page";
import AudioListSubHeader from "~/features/audio/components/ListSubheader";
import AudioList from "~/features/audio/components/List";
import { Audio } from "~/features/audio/types";
import useAudioList from "~/features/audio/hooks/queries/useAudioList";
import InfiniteListControls from "~/components/List/InfiniteListControls";
import { CursorPagedList } from "~/lib/types";

export interface AudioListPageProps {
  filter: Record<string, string | string[] | undefined>;
  initialPage?: CursorPagedList<Audio>;
}

interface AudioListFilter {}

export default function AudioListPage(props: AudioListPageProps) {
  const router = useRouter();
  const { filter, initialPage } = props;

  const [listFilter, setListFilter] = useState<AudioListFilter>({});

  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useAudioList({ ...listFilter }, undefined, {
    staleTime: 1000,
    initialData: initialPage && {
      pages: [initialPage],
      pageParams: [0],
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
    <Page title="Browse Latest Public Audios">
      <AudioList audios={audios} />
      <InfiniteListControls
        fetchNext={fetchNextPage}
        hasNext={hasNextPage}
        isFetching={isFetchingNextPage}
      />
    </Page>
  );
}
