import {
  Box,
  Flex,
  Heading,
  IconButton,
  Stack,
  useColorModeValue,
  useDisclosure,
  Menu,
  MenuButton,
  MenuItem,
  MenuList,
  chakra,
} from "@chakra-ui/react";
import Router from "next/router";
import React, { useEffect } from "react";
import { EditIcon } from "@chakra-ui/icons";
import { MdQueueMusic } from "react-icons/md";
import { HiDotsHorizontal } from "react-icons/hi";
import AudioEditDrawer from "./AudioEditDrawer";
import Link from "~/components/ui/Link";
import { AudioDetailData, Visibility } from "~/features/audio/types";
import { useUser } from "~/features/user/hooks";
import { relativeDate } from "~/utils/time";
import { mapAudioForAudioQueue } from "~/utils/audioplayer";
import AudioPlayButton from "../AudioPlayButton";
import { useAudioQueue } from "~/lib/stores";
import AudioFavoriteButton from "./AudioFavoriteButton";
import PictureController from "~/components/Picture";
import { useAddAudioPicture } from "../../hooks";
import AudioShareModal from "./AudioShareModal";
import { FaShare } from "react-icons/fa";

interface AudioDetailProps {
  audio: AudioDetailData;
}

const AudioDetails: React.FC<AudioDetailProps> = ({ audio }) => {
  const secondaryColor = useColorModeValue("black.300", "gray.300");
  const { user: currentUser } = useUser();
  const addToQueue = useAudioQueue((state) => state.addToQueue);

  const {
    isOpen: isEditOpen,
    onOpen: onEditOpen,
    onClose: onEditClose,
  } = useDisclosure();

  const {
    isOpen: isShareOpen,
    onOpen: onShareOpen,
    onClose: onShareClose,
  } = useDisclosure();

  const { mutateAsync: addPictureAsync, isLoading: isAddingPicture } =
    useAddAudioPicture(audio.id);

  useEffect(() => {
    Router.prefetch(`/users/${audio.user.username}`);
  }, []);

  return (
    <>
      <Flex
        marginBottom={4}
        justifyContent="center"
        direction={{ base: "column", md: "row" }}
      >
        <Flex
          flex="1"
          marginRight={4}
          justify={{ base: "center", md: "normal" }}
        >
          <PictureController
            title={audio.title}
            src={audio.picture || ""}
            onChange={async (croppedData) => {
              await addPictureAsync(croppedData);
            }}
            isUploading={isAddingPicture}
            canEdit={currentUser?.id === audio.user.id}
          />
        </Flex>
        <Box flex="6">
          <Stack direction="row" marginBottom={4}>
            <chakra.div marginTop={{ base: 4, md: 0 }}>
              <Heading as="h1" fontSize={{ base: "3xl", md: "5xl" }}>
                {audio.title}
              </Heading>
              <chakra.div display="flex">
                <Link href={`/users/${audio.user.username}`} fontWeight="500">
                  {audio.user.username}
                </Link>
                <chakra.span
                  color={secondaryColor}
                  _before={{ content: `"•"`, marginX: 2 }}
                >
                  {relativeDate(audio.created)}
                </chakra.span>
              </chakra.div>
            </chakra.div>
          </Stack>
          <Stack direction="column" spacing={2} width="100%">
            <Flex as="header">
              <Box>
                <Stack direction="row" alignItems="center">
                  <AudioPlayButton audio={audio} />
                  <AudioFavoriteButton audioId={audio.id} />
                  <Menu placement="bottom-start">
                    <MenuButton
                      as={IconButton}
                      icon={<HiDotsHorizontal />}
                      variant="ghost"
                      size="lg"
                      isRound
                    />
                    <MenuList>
                      {audio.user.id === currentUser?.id && (
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
                      {audio.visibility != Visibility.Private && (
                        <MenuItem icon={<FaShare />} onClick={onShareOpen}>
                          Share
                        </MenuItem>
                      )}
                    </MenuList>
                  </Menu>
                </Stack>
              </Box>
            </Flex>
          </Stack>
        </Box>
      </Flex>
      <AudioEditDrawer
        audio={audio}
        isOpen={isEditOpen}
        onClose={onEditClose}
      />
      <AudioShareModal
        audioId={audio.id}
        isOpen={isShareOpen}
        onClose={onShareClose}
      />
    </>
  );
};

export default AudioDetails;
