import { Box, chakra, Flex, IconButton, Tooltip } from "@chakra-ui/react";
import React, {
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState,
} from "react";
import { FaPause, FaPlay } from "react-icons/fa";
import Wavesurfer from "wavesurfer.js";
import { useAudioPlayer } from "~/contexts/AudioPlayerContext";
import { Audio, AudioDetail } from "~/features/audio/types";
import { mapAudioForAudioQueue } from "../AudioPlayer/utils";

interface WaveformProps {
  isActive: boolean;
  audio: AudioDetail;
  progressColor?: string;
  cursorColor?: string;
  barGap?: number;
  barWidth?: number;
}

const Waveform: React.FC<WaveformProps> = (props) => {
  const {
    audio,
    isActive,
    progressColor = "#b92c84",
    cursorColor = "#b92c84",
    barGap,
    barWidth,
  } = props;
  const { audioUrl, duration } = audio;

  const { state, dispatch } = useAudioPlayer();
  const { currentTime, isPlaying } = state;
  const [ready, setReady] = useState(false);

  const wavesurfer = useRef<Wavesurfer | null>(null);
  const wavesurferRef = useRef<HTMLDivElement | null>(null);

  /**
   * Current the current percentage (current time / duration)
   */
  const currentPercentage = useMemo(() => {
    return Math.max(0, Math.min(1, (currentTime || 0) / duration));
  }, [currentTime, duration]);

  /**
   * Create wavesurfer and update state
   */
  useEffect(() => {
    if (wavesurferRef.current && !wavesurfer.current) {
      wavesurfer.current = Wavesurfer.create({
        container: wavesurferRef.current,
        progressColor,
        cursorColor,
        barGap,
        barWidth,
      });
      wavesurfer.current?.load(audioUrl);
    }

    return () => {
      wavesurfer.current?.destroy();
      wavesurfer.current = null;
    };
  }, []);

  /**
   * Load the audio url to create waveform
   */
  useEffect(() => {
    const ws = wavesurfer.current;

    const ready = () => {
      setReady(true);
      if (isActive) {
        ws?.seekTo(currentPercentage);
      }
    };

    ws?.on("ready", ready);

    return () => {
      ws?.un("ready", ready);
    };
  }, [isActive, audioUrl]);

  /**
   * Change current time based on the mouseclick on the waveform canvas
   */
  useEffect(() => {
    const ws = wavesurfer.current;

    const seek = (e: React.MouseEvent<HTMLElement>) => {
      const { left, width } = e.currentTarget.getBoundingClientRect();
      const newCurrentTime = ((e.clientX - left) / width) * duration;
      dispatch({ type: "SET_CURRENT_TIME", payload: newCurrentTime });
    };

    if (ready && isActive) {
      ws?.drawer.on("click", seek);
    }

    return () => {
      ws?.drawer.un("click", seek);
    };
  }, [duration, ready, isActive]);

  /**
   * Update the waveform progress based on current Time
   */
  useEffect(() => {
    const ws = wavesurfer.current;

    if (ready && isActive) {
      ws?.seekTo(currentPercentage);
    }
  }, [ready, isActive, currentPercentage]);

  const clickPlayButton = useCallback(() => {
    if (isActive) {
      dispatch({ type: "SET_PLAYING", payload: !isPlaying });
    } else {
      dispatch({
        type: "SET_NEW_QUEUE",
        payload: mapAudioForAudioQueue(audio),
        index: 0,
      });
    }
  }, [isActive, isPlaying, audio.id]);

  return (
    <Flex>
      <Flex justifyContent="center" alignItems="center" width="100px">
        <Tooltip label="Play" placement="top">
          <span>
            <IconButton
              isRound
              colorScheme="pink"
              size="lg"
              icon={isPlaying && isActive ? <FaPause /> : <FaPlay />}
              aria-label="Play"
              onClick={clickPlayButton}
            />
          </span>
        </Tooltip>
      </Flex>
      <chakra.div width="100%" ref={wavesurferRef}></chakra.div>
    </Flex>
  );
};

export default Waveform;
