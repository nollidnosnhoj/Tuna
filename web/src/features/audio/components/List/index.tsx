import { Box } from "@chakra-ui/layout";
import React, { useState } from "react";
import NextImage from "next/image";
import { Audio } from "../../types";
import AudioListItem from "./Item";
import InfiniteListControls from "~/components/List/InfiniteListControls";
import PaginationListControls from "~/components/List/PaginationListControls";

type AudioListLayout = "list" | "grid";
type AudioListType = "infinite" | "paginated";

type AudioListProps = {
  audios: Audio[];
  type?: AudioListType;
  count?: number;
  isFetching?: boolean;
  defaultLayout?: AudioListLayout;
} & (
  | {
      type: "infinite";
      fetchNext: () => void;
      hasNext?: boolean;
    }
  | {
      type: "paginated";
      page: number;
      setPage: (page: number) => void;
      totalPages: number;
      hasNext?: boolean;
      hasPrevious?: boolean;
      isPreviousData?: boolean;
    }
);

export default function AudioList(props: AudioListProps) {
  const {
    audios,
    defaultLayout = "list",
    isFetching = false,
    count = audios.length,
    ...rest
  } = props;

  const [layout, setLayout] = useState<AudioListLayout>(defaultLayout);

  return (
    <Box>
      {audios.map((audio, index) => {
        switch (layout) {
          case "list":
            return <AudioListItem audio={audio} />;
          case "grid":
            throw new Error("Grid item has not been implemented.");
        }
      })}
      <Box>
        {rest.type === "infinite" && (
          <InfiniteListControls isFetching={isFetching} {...rest} />
        )}
        {rest.type === "paginated" && (
          <PaginationListControls isFetching={isFetching} {...rest} />
        )}
      </Box>
    </Box>
  );
}
