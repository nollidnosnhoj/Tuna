import { Box, Flex, chakra } from "@chakra-ui/react";
import React from "react";
import Link from "~/components/Link";
import { AudioPlayerItem } from "../types";

interface NowPlayingSectionProps {
  current?: AudioPlayerItem;
}

export default function NowPlayingSection(props: NowPlayingSectionProps) {
  const { current } = props;

  if (!current) return null;

  const { audioId, artistId, title, artist } = current;

  return (
    <Flex fontSize="16px">
      <Box>
        <Box>
          <Link href={`/audios/${audioId}`}>
            <chakra.strong>{title}</chakra.strong>
          </Link>
        </Box>
        <Box>
          <Link href={`/users/${artist}`}>
            <chakra.span>{artist}</chakra.span>
          </Link>
        </Box>
      </Box>
    </Flex>
  );
}
