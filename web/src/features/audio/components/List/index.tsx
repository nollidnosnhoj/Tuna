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
  type?: AudioListType;
  audios: Audio[];
  page?: number;
  totalPages?: number;
  fetchPage?: (args: any) => void;
  count?: number;
  hasNext?: boolean;
  hasPrevious?: boolean;
  isPreviousData?: boolean;
  isFetching?: boolean;
  defaultLayout?: AudioListLayout;
  notFoundContent?: string | React.ReactNode;
};

export default function AudioList(props: AudioListProps) {
  const { audios, defaultLayout = "list", notFoundContent, ...rest } = props;

  let type: AudioListType | undefined;

  if ("type" in rest) {
    type = rest["type"];
    delete rest["type"];
  }

  const [layout, setLayout] = useState<AudioListLayout>(defaultLayout);

  return (
    <Box>
      {audios.length === 0 && notFoundContent}
      {audios.length > 0 &&
        audios.map((audio, index) => {
          switch (layout) {
            case "list":
              return <AudioListItem key={index} audio={audio} />;
            case "grid":
              throw new Error("Grid item has not been implemented.");
          }
        })}
      <Box>
        {type === "infinite" && <InfiniteListControls {...rest} />}
        {type === "paginated" && <PaginationListControls {...rest} />}
      </Box>
    </Box>
  );
}
