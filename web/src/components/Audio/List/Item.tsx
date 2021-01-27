import {
  AspectRatio,
  Badge,
  Box,
  Flex,
  Heading,
  Icon,
  Image,
  Stack,
  Text,
  Tooltip,
} from "@chakra-ui/react";
import React from "react";
import { MdLoop } from "react-icons/md";
import Link from "~/components/Shared/Link";
import { AudioListModel } from "~/lib/types/audio";
import { getThumbnailUrl } from "~/utils";
import { formatDuration } from "~/utils/time";

interface AudioListItemProps {
  audio: AudioListModel;
  removeArtistName?: boolean;
}

const AudioListItem: React.FC<AudioListItemProps> = ({
  audio,
  removeArtistName = false,
}) => {
  return (
    <Flex
      marginBottom={4}
      borderWidth="1px"
      rounded="md"
      boxShadow="lg"
      overflow="hidden"
    >
      <AspectRatio
        ratio={1}
        boxSize="120px"
        bgGradient="linear(to-r, green.200, pink.500)"
      >
        <Image
          src={getThumbnailUrl(audio.pictureUrl, "medium")}
          objectFit="cover"
        />
      </AspectRatio>
      <Flex width="100%" paddingY={2} paddingX={4}>
        <Box flex="3">
          <Heading as="h3" size="md">
            <Link href={`audios/${audio.id}`}>{audio.title}</Link>
            {audio.isLoop && (
              <Tooltip label="Loop" fontSize="sm">
                <span>
                  <Icon as={MdLoop} size="sm" marginLeft={2} />
                </span>
              </Tooltip>
            )}
          </Heading>
          {!removeArtistName && (
            <Link href={`users/${audio.user.username}`}>
              <Text as="i">{audio.user.username}</Text>
            </Link>
          )}
        </Box>
        <Flex flex="1" justify="flex-end">
          <Stack direction="column" spacing={2} textAlign="right">
            <div>
              <Badge>{audio.genre}</Badge>
            </div>
            <div>
              <Text fontSize="sm">{formatDuration(audio.duration)}</Text>
            </div>
          </Stack>
        </Flex>
      </Flex>
    </Flex>
  );
};

export default AudioListItem;
