import React, { useCallback } from "react";
import { chakra, Icon } from "@chakra-ui/react";
import { REPEAT_MODE, useAudioPlayer } from "~/contexts/AudioPlayerContext";
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

export default function RepeatControl() {
  const { state, dispatch } = useAudioPlayer();
  const { repeat } = state;

  const handleButtonClick = useCallback(() => {
    let newIndex = repeatModeOrder.findIndex((m) => m === repeat) + 1;
    if (newIndex === 0) throw new Error("Cannot find repeat mode");
    if (newIndex > repeatModeOrder.length - 1) newIndex = 0;
    dispatch({ type: "SET_REPEAT", payload: repeatModeOrder[newIndex] });
  }, [repeat]);

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
