import H5AudioPlayer from "react-h5-audio-player";
import React from "react";
import useAudioPlayer from "~/hooks/useAudioPlayer";
import { Box } from "@chakra-ui/react";

export default function AudioPlayer() {
  const { audioList, playIndex } = useAudioPlayer();

  const handleClickPrevious = () => {};

  const handleClickNext = () => {};

  return (
    <React.Fragment>
      <Box
        borderTopWidth="1px"
        pos="fixed"
        bottom="0"
        left="0"
        width="100%"
        height="120px"
        zIndex="99"
        paddingY={4}
      >
        <H5AudioPlayer
          autoPlayAfterSrcChange={true}
          showSkipControls={true}
          showJumpControls={false}
          src={audioList[playIndex ?? 0]?.source ?? ""}
          onClickPrevious={handleClickPrevious}
          onClickNext={handleClickNext}
        />
      </Box>
    </React.Fragment>
  );
}
