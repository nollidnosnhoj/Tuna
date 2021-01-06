import {
  Box,
  Button,
  Divider,
  Flex,
  Heading,
  Link,
  Text,
} from "@chakra-ui/react";
import React, { useMemo } from "react";
import { Audio, AudioSearchType } from "~/lib/types";
import { useAudiosInfinite } from "~/lib/services/audio";
import { apiErrorToast } from "~/utils/toast";

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
    data,
    page,
    setPage,
    isLoadingMore,
    isEmpty,
    isReachingEnd,
  } = useAudiosInfinite({ type, size, params, username });

  const audios = useMemo<Audio[]>(() => {
    return data ? [].concat(...data) : [];
  }, [data]);

  return (
    <React.Fragment>
      {headerText && (
        <>
          <Heading>{headerText}</Heading>
          <Divider marginY={4} />
        </>
      )}
      {isEmpty && <p>No audio found.</p>}
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
      {!isReachingEnd && (
        <Button
          width="100%"
          variant="outline"
          disabled={isLoadingMore}
          onClick={() => setPage(page + 1)}
        >
          {isLoadingMore ? "Loading..." : "Load more"}
        </Button>
      )}
    </React.Fragment>
  );
};

export default AudioList;
