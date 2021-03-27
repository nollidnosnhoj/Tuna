import { Icon, chakra, Tooltip } from "@chakra-ui/react";
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
      icon = <Icon opacity="0.5" as={MdRepeat} aria-label="No Repeat" />;
      break;
    case REPEAT_MODE.REPEAT:
      icon = <Icon as={MdRepeat} aria-label="Repeat Loop" />;
      break;
    case REPEAT_MODE.REPEAT_SINGLE:
      icon = <Icon as={MdRepeatOne} aria-label="Repeat One" />;
      break;
  }

  return (
    <Tooltip label={`Click to ${label}`}>
      <span>
        <chakra.button onClick={onClick} className="rhap_repeat-button">
          {icon}
        </chakra.button>
      </span>
    </Tooltip>
  );
}
