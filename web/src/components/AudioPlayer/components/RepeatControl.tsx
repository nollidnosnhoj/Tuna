import React, { useCallback } from "react";
import { chakra, Icon } from "@chakra-ui/react";
import { REPEAT_MODE } from "~/contexts/AudioPlayerContext";
import { MdRepeat, MdRepeatOne } from "react-icons/md";

const repeatLabels = {
  [REPEAT_MODE.DISABLE]: "No Repeat",
  [REPEAT_MODE.REPEAT]: "Repeat All",
  [REPEAT_MODE.REPEAT_SINGLE]: "Repeat One",
};

const repeatModeOrder = [
  REPEAT_MODE.DISABLE,
  REPEAT_MODE.REPEAT,
  REPEAT_MODE.REPEAT_SINGLE,
];

interface RepeatControlProps {
  repeat: REPEAT_MODE;
  onRepeatChange: (value: REPEAT_MODE) => void;
}

export default function RepeatControl(props: RepeatControlProps) {
  const { repeat, onRepeatChange } = props;

  const handleButtonClick = useCallback(() => {
    let newIndex = repeatModeOrder.findIndex((m) => m === repeat) + 1;
    if (newIndex === 0) throw new Error("Cannot find repeat mode");
    if (newIndex > repeatModeOrder.length - 1) newIndex = 0;
    onRepeatChange(repeatModeOrder[newIndex]);
  }, [repeat, onRepeatChange]);

  let icon: React.ReactNode;

  switch (repeat) {
    case REPEAT_MODE.DISABLE:
      icon = <Icon opacity="0.5" as={MdRepeat} />;
      break;
    case REPEAT_MODE.REPEAT:
      icon = <Icon as={MdRepeat} />;
      break;
    case REPEAT_MODE.REPEAT_SINGLE:
      icon = <Icon as={MdRepeatOne} />;
      break;
  }

  return (
    <chakra.button
      onClick={handleButtonClick}
      aria-label={repeatLabels[repeat]}
      title={repeatLabels[repeat]}
    >
      {icon}
    </chakra.button>
  );
}
