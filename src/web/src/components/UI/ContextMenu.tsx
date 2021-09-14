/* eslint-disable @typescript-eslint/no-explicit-any */
import {
  Menu,
  MenuButton,
  IconButton,
  MenuList,
  MenuItem,
  ButtonProps,
  MenuProps,
  MenuDivider,
  MenuGroup,
} from "@chakra-ui/react";
import React from "react";
import { HiDotsHorizontal } from "react-icons/hi";

type ButtonSizeType = Pick<ButtonProps, "size">;
type ButtonVariantType = Pick<ButtonProps, "variant">;
type MenuPlacementType = Pick<MenuProps, "placement">;

interface AudioMiscMenuProps
  extends ButtonSizeType,
    ButtonVariantType,
    MenuPlacementType {
  items: { title?: string; items: ContextMenuItem[] }[];
  icon?: JSX.Element;
}

type ContextMenuItem = {
  name: string;
  isVisible: boolean;
  icon?: React.ReactElement<any, string | React.JSXElementConstructor<any>>;
  onClick?: React.MouseEventHandler<HTMLButtonElement>;
};

export default function AudioMiscMenu({
  items,
  icon = <HiDotsHorizontal />,
  placement = "bottom-start",
  size = "md",
  variant = "ghost",
}: AudioMiscMenuProps) {
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
          {items.map((item, index) => {
            return (
              <React.Fragment key={index}>
                <MenuGroup title={item.title}>
                  {item.items.map((childItem, childIndex) =>
                    mapToMenuItem(childItem, childIndex)
                  )}
                </MenuGroup>
                {index !== items.length - 1 && <MenuDivider />}
              </React.Fragment>
            );
          })}
        </MenuList>
      </Menu>
    </>
  );
}

function mapToMenuItem(item: ContextMenuItem, index: number) {
  return (
    <MenuItem key={item.name + index} icon={item.icon} onClick={item.onClick}>
      {item.name}
    </MenuItem>
  );
}
