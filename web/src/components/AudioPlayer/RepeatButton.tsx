import { IconButton, Tooltip } from "@chakra-ui/react";
import React from "react";
import { MdRepeat, MdRepeatOne } from "react-icons/md";
import { REPEAT_MODE } from "~/contexts/audioPlayerContext";

interface RepeatButtonProps {
  mode: REPEAT_MODE;
  label: string;
  onClick?: () => void;
}

export default function RepeatButton(props: RepeatButtonProps) {
  const { mode, label, onClick } = props;

  let icon: React.ReactNode;

  switch (mode) {
    case REPEAT_MODE.DISABLE:
      icon = (
        <IconButton
          opacity="0.5"
          icon={<MdRepeat />}
          aria-label={label}
          borderRadius="full"
          variant="ghost"
          onClick={onClick}
        />
      );
      break;
    case REPEAT_MODE.REPEAT:
      icon = (
        <IconButton
          icon={<MdRepeat />}
          aria-label={label}
          borderRadius="full"
          variant="ghost"
          onClick={onClick}
        />
      );
      break;
    case REPEAT_MODE.REPEAT_SINGLE:
      icon = (
        <IconButton
          icon={<MdRepeatOne />}
          aria-label={label}
          borderRadius="full"
          variant="ghost"
          onClick={onClick}
        />
      );
      break;
  }

  return <Tooltip label={label}>{icon}</Tooltip>;
}
