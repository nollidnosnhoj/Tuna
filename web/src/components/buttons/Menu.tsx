/* eslint-disable @typescript-eslint/no-explicit-any */
import {
  ButtonProps,
  IconButton,
  Menu,
  MenuButton,
  MenuDivider,
  MenuGroup,
  MenuItem,
  MenuList,
  MenuProps,
} from "@chakra-ui/react";
import React, { PropsWithChildren } from "react";
import { HiDotsHorizontal } from "react-icons/hi";
import { MdQueueMusic } from "react-icons/md";
import { useAudioPlayer } from "~/lib/stores";
import { Audio } from "~/lib/types";

type ButtonSizeType = Pick<ButtonProps, "size">;
type ButtonVariantType = Pick<ButtonProps, "variant">;
type MenuPlacementType = Pick<MenuProps, "placement">;

interface IAudioMiscMenuProps
  extends ButtonSizeType,
    ButtonVariantType,
    MenuPlacementType {
  icon?: JSX.Element;
  audio: Audio;
}

export default function AudioMiscMenu({
  audio,
  icon = <HiDotsHorizontal />,
  placement = "bottom-start",
  size = "md",
  variant = "ghost",
  children,
}: PropsWithChildren<IAudioMiscMenuProps>) {
  const addToQueue = useAudioPlayer((state) => state.addToQueue);
  return (
    <>
      <Menu placement={placement}>
        <MenuButton
          as={IconButton}
          icon={icon}
          variant={variant}
          size={size}
          isRound
        />
        <MenuList>
          <MenuGroup>
            <MenuItem
              icon={<MdQueueMusic />}
              onClick={async () => await addToQueue("custom", [audio])}
            >
              Add to Queue
            </MenuItem>
          </MenuGroup>
          <MenuDivider />
          {children}
        </MenuList>
      </Menu>
    </>
  );
}
