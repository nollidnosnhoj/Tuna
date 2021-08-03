import { EditIcon } from "@chakra-ui/icons";
import {
  Menu,
  MenuButton,
  IconButton,
  MenuList,
  MenuItem,
  useDisclosure,
} from "@chakra-ui/react";
import React from "react";
import { FaShare } from "react-icons/fa";
import { HiDotsHorizontal } from "react-icons/hi";
import { MdQueueMusic } from "react-icons/md";
import { useUser } from "~/features/user/hooks";
import { useAudioQueue } from "~/lib/stores";
import { mapAudioForAudioQueue } from "~/utils";
import { AudioDetailData, Visibility } from "../../types";
import AudioEditDrawer from "./AudioEditDrawer";
import AudioShareModal from "./AudioShareModal";

interface AudioMiscMenuProps {
  audio: AudioDetailData;
}

export default function AudioMiscMenu({ audio }: AudioMiscMenuProps) {
  const { user } = useUser();
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
  return (
    <>
      <Menu placement="bottom-start">
        <MenuButton
          as={IconButton}
          icon={<HiDotsHorizontal />}
          variant="ghost"
          size="lg"
          isRound
        />
        <MenuList>
          {audio.user.id === user?.id && (
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
}
