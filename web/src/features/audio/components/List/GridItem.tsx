import { Box, Text } from "@chakra-ui/react";
import NextImage from "next/image";
import React from "react";
import Link from "~/components/Link";
import { AudioListItemProps } from "./StackItem";
import PictureContainer from "~/components/Picture/PictureContainer";

export default function AudioGridItem(props: AudioListItemProps) {
  const { audio, removeArtistName = false } = props;

  return (
    <Box>
      <PictureContainer width={200} borderWidth="1px">
        {audio.picture && (
          <NextImage
            src={audio.picture}
            layout="fill"
            objectFit="cover"
            loading="lazy"
          />
        )}
      </PictureContainer>
      <Box marginTop={2}>
        <Box>
          <Link
            href={`/audios/${audio.id}`}
            _hover={{ textDecoration: "none" }}
          >
            <Text as="b">{audio.title}</Text>
          </Link>
        </Box>
        {!removeArtistName && (
          <Link href={`/users/${audio.author.username}`}>
            <Text as="i">{audio.author.username}</Text>
          </Link>
        )}
      </Box>
    </Box>
  );
}
