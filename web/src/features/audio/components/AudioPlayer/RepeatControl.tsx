import React, { useCallback } from "react";
import { chakra, Icon } from "@chakra-ui/react";
import { MdRepeat, MdRepeatOne } from "react-icons/md";
import { useAudioPlayer } from "~/lib/hooks";
import { REPEAT_MODE } from "~/lib/hooks/useAudioPlayer";

const repeatLabels = {
  [REPEAT_MODE.DISABLE]: "No Repeat",
  [REPEAT_MODE.REPEAT]: "Repeat All",
  [REPEAT_MODE.REPEAT_ONE]: "Repeat One",
};

const repeatModeOrder = [
  REPEAT_MODE.DISABLE,
  REPEAT_MODE.REPEAT,
  REPEAT_MODE.REPEAT_ONE,
];

export default function RepeatControl() {
  const { repeat, setRepeatMode } = useAudioPlayer();

  const handleButtonClick = useCallback(() => {
    let newIndex = repeatModeOrder.findIndex((m) => m === repeat) + 1;
    if (newIndex === 0) throw new Error("Cannot find repeat mode");
    if (newIndex > repeatModeOrder.length - 1) newIndex = 0;
    setRepeatMode(repeatModeOrder[newIndex]);
  }, [repeat]);

  let icon: React.ReactNode;

  switch (repeat) {
    case REPEAT_MODE.DISABLE:
      icon = <Icon opacity="0.5" as={MdRepeat} />;
      break;
    case REPEAT_MODE.REPEAT:
      icon = <Icon as={MdRepeat} />;
      break;
    case REPEAT_MODE.REPEAT_ONE:
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
