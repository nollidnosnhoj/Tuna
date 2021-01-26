import { Button, Divider, Heading } from "@chakra-ui/react";
import React from "react";
import { AudioSearchType } from "~/lib/types/audio";
import { useAudiosInfiniteQuery } from "~/lib/services/audio";
import AudioListItem from "./Item";

interface AudioListProps {
  type?: AudioSearchType;
  username?: string;
  page?: number;
  size?: number;
  params?: Record<string, any>;
  headerText?: string;
  removeArtistName?: boolean;
}

const AudioList: React.FC<AudioListProps> = ({
  type = "audios",
  size = 15,
  params,
  username,
  headerText,
  removeArtistName = false,
  ...props
}) => {
  const {
    items: audios,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useAudiosInfiniteQuery({ type, size, params, username });

  return (
    <React.Fragment>
      {headerText && (
        <>
          <Heading>{headerText}</Heading>
          <Divider marginY={4} />
        </>
      )}
      {audios.length === 0 && <p>No audio found.</p>}
      {audios.map((audio) => (
        <AudioListItem
          key={audio.id}
          audio={audio}
          removeArtistName={removeArtistName}
        />
      ))}
      {hasNextPage && (
        <Button
          width="100%"
          variant="outline"
          disabled={isFetchingNextPage}
          onClick={() => fetchNextPage()}
        >
          {isFetchingNextPage ? "Loading..." : "Load more"}
        </Button>
      )}
    </React.Fragment>
  );
};

export default AudioList;
