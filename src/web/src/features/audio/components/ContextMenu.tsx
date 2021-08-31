import { EditIcon } from "@chakra-ui/icons";
import {
  Menu,
  MenuButton,
  IconButton,
  MenuList,
  MenuItem,
  useDisclosure,
  ButtonProps,
  MenuProps,
} from "@chakra-ui/react";
import React from "react";
import { FaShare } from "react-icons/fa";
import { HiDotsHorizontal } from "react-icons/hi";
import { MdQueueMusic } from "react-icons/md";
import { useUser } from "~/features/user/hooks";
import { useAudioQueue } from "~/lib/stores";
import { AudioView } from "../api/types";
import AudioEditDrawer from "./Edit";
import AudioShareModal from "./Share";

type ButtonSizeType = Pick<ButtonProps, "size">;
type MenuPlacementType = Pick<MenuProps, "placement">;

interface AudioMiscMenuProps extends ButtonSizeType, MenuPlacementType {
  audio: AudioView;
  context?: string;
}

export default function AudioMiscMenu({ audio, ...props }: AudioMiscMenuProps) {
  const { placement = "bottom-start", size = "md", context = "custom" } = props;
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
      <Menu placement={placement}>
        <MenuButton
          as={IconButton}
          icon={<HiDotsHorizontal />}
          variant="ghost"
          size={size}
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
            onClick={async () => await addToQueue(context, [audio])}
          >
            Add to queue
          </MenuItem>
          <MenuItem icon={<FaShare />} onClick={onShareOpen}>
            Share
          </MenuItem>
        </MenuList>
      </Menu>
      <AudioEditDrawer
        audio={audio}
        isOpen={isEditOpen}
        onClose={onEditClose}
      />
      <AudioShareModal
        slug={audio.slug}
        isOpen={isShareOpen}
        onClose={onShareClose}
      />
    </>
  );
}
