import {
  Badge,
  Box,
  Flex,
  Heading,
  IconButton,
  Spacer,
  Stack,
  Text,
  useColorModeValue,
  useDisclosure,
  VStack,
  Menu,
  MenuButton,
  MenuItem,
  MenuList,
} from "@chakra-ui/react";
import Router from "next/router";
import React, { useEffect } from "react";
import { EditIcon } from "@chakra-ui/icons";
import { MdQueueMusic } from "react-icons/md";
import { HiDotsVertical } from "react-icons/hi";
import AudioEditDrawer from "./AudioEditDrawer";
import AudioTags from "./AudioTags";
import AudioPicture from "../AudioPicture";
import Link from "~/components/Link";
import { AudioDetailData } from "~/features/audio/types";
import { useUser } from "~/features/user/hooks/useUser";
import { relativeDate } from "~/utils/time";
import { mapAudioForAudioQueue } from "~/utils/audioplayer";
import AudioPlayButton from "../AudioPlayButton";
import { useAudioPlayer } from "~/lib/hooks";

interface AudioDetailProps {
  audio: AudioDetailData;
}

const AudioDetails: React.FC<AudioDetailProps> = ({ audio }) => {
  const secondaryColor = useColorModeValue("black.300", "gray.300");
  const { user: currentUser } = useUser();
  const { addToQueue } = useAudioPlayer();

  const {
    isOpen: isEditOpen,
    onOpen: onEditOpen,
    onClose: onEditClose,
  } = useDisclosure();

  useEffect(() => {
    Router.prefetch(`/users/${audio.author.username}`);
  }, []);

  return (
    <Box>
      <Flex marginBottom={4} justifyContent="center">
        <Box flex="1" marginRight={4}>
          <AudioPicture
            audioId={audio.id}
            pictureTitle={audio.title}
            pictureSrc={audio.picture || ""}
            canModify={currentUser?.id === audio.author.id}
          />
        </Box>
        <Box flex="5">
          <Stack direction="row" marginBottom={4}>
            <AudioPlayButton audio={audio} />
            <Stack direction="column" spacing="0" fontSize="sm">
              <Link href={`/users/${audio.author.username}`}>
                <Text fontWeight="500">{audio.author.username}</Text>
              </Link>
              <Text color={secondaryColor}>{relativeDate(audio.uploaded)}</Text>
            </Stack>
            <Spacer />
            <Menu placement="bottom-end">
              <MenuButton
                as={IconButton}
                icon={<HiDotsVertical />}
                variant="ghost"
                isRound
              />
              <MenuList>
                {audio.author.id === currentUser?.id && (
                  <MenuItem icon={<EditIcon />} onClick={onEditOpen}>
                    Edit
                  </MenuItem>
                )}
                <MenuItem
                  icon={<MdQueueMusic />}
                  onClick={() => addToQueue(mapAudioForAudioQueue(audio))}
                >
                  Add to queue
                </MenuItem>
              </MenuList>
            </Menu>
            <AudioEditDrawer
              audio={audio}
              isOpen={isEditOpen}
              onClose={onEditClose}
            />
          </Stack>
          <Stack direction="column" spacing={2} width="100%">
            <Flex as="header">
              <Box>
                <Flex alignItems="center">
                  <Heading as="h1" fontSize="2xl">
                    {audio.title}
                  </Heading>
                </Flex>
              </Box>
              <Spacer />
              <VStack spacing={2} alignItems="normal" textAlign="right">
                {!audio.isPublic && <Badge>PRIVATE</Badge>}
              </VStack>
            </Flex>
            <AudioTags tags={audio.tags} />
          </Stack>
        </Box>
      </Flex>
    </Box>
  );
};

export default AudioDetails;
