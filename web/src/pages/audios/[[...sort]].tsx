import { Box, HStack, Spacer } from "@chakra-ui/layout";
import { GetServerSideProps } from "next";
import { useRouter } from "next/router";
import React, { useCallback, useEffect, useState } from "react";
import queryString from "query-string";
import GenreSelect from "~/components/Form/GenreSelect";
import Page from "~/components/Page";
import AudioInfiniteList from "~/features/audio/components/List/AudioInfiniteList";
import AudioListSubHeader from "~/features/audio/components/ListSubheader";

type SortState = "latest" | "favorites";

const sortPageTitles: { [K in SortState]: string } = {
  latest: "Latest Audios",
  favorites: "Most Liked Audios",
};

interface AudioListPageProps {
  sort: SortState;
  filter: Record<string, string | string[] | undefined>;
}

export const getServerSideProps: GetServerSideProps<AudioListPageProps> = async ({
  query,
  params,
}) => {
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
    case "favorites":
      sort = "favorites";
      break;
    default:
      return {
        notFound: true,
      };
  }

  const { page, ...filter } = query;

  return {
    props: {
      sort: sort,
      filter: filter,
    },
  };
};

interface AudioListFilter {
  genre?: string;
}

export default function AudioListPage(props: AudioListPageProps) {
  const router = useRouter();
  const { sort, filter } = props;

  const [listFilter, setListFilter] = useState<AudioListFilter>(() => ({
    genre:
      (filter &&
        filter.genre &&
        (Array.isArray(filter.genre) ? filter.genre[0] : filter.genre)) ||
      "",
  }));

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

  useEffect(() => {});

  return (
    <Page
      title={sortPageTitles[sort]}
      beforeContainer={<AudioListSubHeader current={sort} />}
    >
      <HStack>
        <Box>
          <GenreSelect
            name="genre"
            size="sm"
            value={listFilter.genre || ""}
            onChange={(e) => handleChange({ genre: e.target.value })}
            addAllGenres
          />
        </Box>
      </HStack>
      <AudioInfiniteList queryParams={{ ...listFilter, sort }} size={15} />
    </Page>
  );
}
