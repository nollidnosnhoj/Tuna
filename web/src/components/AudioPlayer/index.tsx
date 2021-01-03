import React, { useState, useRef, useCallback, useEffect } from "react";
import { Box, Circle, Flex, Text } from "@chakra-ui/react";
import { IoMdPlay, IoMdPause } from "react-icons/io";

import WaveSurfer from "wavesurfer.js";
import WaveSurferComponent from "~/components/Wavesurfer";
import { formatDuration } from "~/utils/time";
import { useAudioPlayer } from "~/lib/contexts/audio_player_context";

const AudioPlayer: React.FC<{
  url: string;
  color?: string;
}> = ({ url, color = "#ED64A6", ...props }) => {
  const { volume, handleVolume } = useAudioPlayer();

  const [loop, setLoop] = useState(false);
  const [loaded, setLoaded] = useState(false);
  const [loading, setLoading] = useState(0);
  const [playing, setPlaying] = useState(false);
  const [seconds, setSeconds] = useState(0);
  const wavesurferRef = useRef<WaveSurfer>(null);
  const handleMount = useCallback(
    (ws: WaveSurfer) => {
      wavesurferRef.current = ws;
      if (wavesurferRef.current) {
        wavesurferRef.current.load(url);
        wavesurferRef.current.on("ready", () => {
          setLoaded(true);
        });
        wavesurferRef.current.on("volume", (level: number) => {
          handleVolume(level);
        });
        wavesurferRef.current.on("loading", (data: number) => {
          setLoading(data);
        });
        wavesurferRef.current.on("seek", () => {
          setSeconds(wavesurferRef.current.getCurrentTime());
        });
        wavesurferRef.current.on("audioprocess", () => {
          setSeconds(wavesurferRef.current.getCurrentTime());
        });
        wavesurferRef.current.on("finish", () => {
          if (!loop) {
            setPlaying(false);
            setSeconds(0);
          }
        });
      }
    },
    [url]
  );

  const handleUnmount = () => {
    wavesurferRef.current.unAll();
    wavesurferRef.current.destroy();
    wavesurferRef.current = null;
  };

  const onPlayPause = useCallback(() => {
    wavesurferRef.current.playPause();
    setPlaying(!playing);
  }, [playing]);

  useEffect(() => {
    if (wavesurferRef.current) {
      wavesurferRef.current.setVolume(volume);
    }
  }, [volume]);

  return (
    <WaveSurferComponent
      onMount={handleMount}
      onUnmount={handleUnmount}
      cursorWidth={1}
      cursorColor={color}
      height={150}
      responsive={true}
    >
      <Flex paddingY="0.2rem" paddingX="0.2rem" align="center">
        <Flex padding="1rem" width="10%" align="center">
          <Circle
            size="70px"
            bg={color}
            color="white"
            onClick={onPlayPause}
            as="button"
            disabled={loaded === false}
          >
            {playing ? <IoMdPause size="30px" /> : <IoMdPlay size="30px" />}
          </Circle>
        </Flex>
        <Box width="80%">
          <div id="wave-form"></div>
        </Box>
        <Box width="10%" textAlign="center">
          <Text fontSize="2xl">{formatDuration(seconds)}</Text>
        </Box>
      </Flex>
    </WaveSurferComponent>
  );
};

export default AudioPlayer;
