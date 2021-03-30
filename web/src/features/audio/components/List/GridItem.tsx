import { Box, Text } from "@chakra-ui/react";
import React, { useMemo } from "react";
import Picture from "~/components/Picture";
import Link from "~/components/Link";
import useUser from "~/hooks/useUser";
import { AudioListItemProps } from "./ListItem";

export default function AudioGridItem(props: AudioListItemProps) {
  const { audio, onPlayClick, isPlaying, removeArtistName = false } = props;

  const { user: currentUser } = useUser();

  const picture = useMemo(() => {
    return audio?.picture
      ? `https://audiochan-public.s3.amazonaws.com/${audio.picture}`
      : "";
  }, [audio.picture]);

  return (
    <Box>
      <Picture source={picture} imageSize={200} borderWidth={1} />
      <Box marginTop={2}>
        <Box>
          <Link
            href={`/audios/view/${audio.id}`}
            _hover={{ textDecoration: "none" }}
          >
            <Text as="b">{audio.title}</Text>
          </Link>
        </Box>
        {!removeArtistName && (
          <Link href={`/users/${audio.user.username}`}>
            <Text as="i">{audio.user.username}</Text>
          </Link>
        )}
      </Box>
    </Box>
  );
}