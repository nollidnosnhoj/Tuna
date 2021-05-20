import { Box, Flex, chakra, useDisclosure } from "@chakra-ui/react";
import React from "react";
import NextImage from "next/image";
import Link from "~/components/Link";
import { AudioPlayerItem } from "~/lib/contexts/types";
import PictureContainer from "../Picture/PictureContainer";
import PictureModal from "../Picture/PictureModal";

interface NowPlayingSectionProps {
  current?: AudioPlayerItem;
}

export default function NowPlayingSection(props: NowPlayingSectionProps) {
  const { current } = props;

  const { isOpen, onClose, onOpen } = useDisclosure();

  if (!current) return null;

  const { audioId, title, artist, cover } = current;

  return (
    <Flex fontSize="16px" alignItems="center">
      <Box marginRight={4}>
        <PictureContainer
          width={75}
          borderWidth="1px"
          onClick={onOpen}
          cursor="pointer"
        >
          {cover && (
            <NextImage
              src={cover}
              layout="fill"
              objectFit="cover"
              loading="lazy"
            />
          )}
        </PictureContainer>
        <PictureModal
          title={title}
          src={cover}
          isOpen={isOpen}
          onClose={onClose}
        />
      </Box>
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
