import {
  Box,
  Button,
  Divider,
  Flex,
  Heading,
  Link,
  Text,
} from "@chakra-ui/react";
import React from "react";
import { AudioSearchType } from "~/lib/types/audio";
import { useAudiosInfiniteQuery } from "~/lib/services/audio";

interface AudioListProps {
  type?: AudioSearchType;
  username?: string;
  page?: number;
  size?: number;
  params?: Record<string, any>;
  headerText?: string;
}

const AudioList: React.FC<AudioListProps> = ({
  type = "audios",
  size = 15,
  params,
  username,
  headerText,
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
        <Flex direction="row" marginY="3" key={audio.id}>
          <Box width="100%">
            <Link href={`/audios/${audio.id}`}>
              <Text as="b" fontSize="2xl">
                {audio.title}
              </Text>
            </Link>
            <Text>{audio.user?.username}</Text>
          </Box>
        </Flex>
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
