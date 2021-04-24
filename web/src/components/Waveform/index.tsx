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
import { AudioDetail } from "~/features/audio/types";
import { mapAudioForAudioQueue } from "../AudioPlayer/utils";

interface WaveformProps {
  audio: AudioDetail;
  progressColor?: string;
  cursorColor?: string;
  barGap?: number;
  barWidth?: number;
}

const Waveform: React.FC<WaveformProps> = (props) => {
  const {
    audio,
    progressColor = "#b92c84",
    cursorColor = "#b92c84",
    barGap,
    barWidth,
  } = props;
  const { audioUrl } = audio;

  const { state, dispatch } = useAudioPlayer();
  const { currentTime, isPlaying, audioRef, currentAudio } = state;
  const { duration } = currentAudio || { duration: audio.duration };
  const [ready, setReady] = useState(false);

  const wavesurfer = useRef<Wavesurfer | null>(null);
  const wavesurferRef = useRef<HTMLDivElement | null>(null);

  const isActivelyPlaying = useMemo(() => {
    if (!currentAudio) return false;
    return currentAudio.audioId === audio.id;
  }, [currentAudio?.audioId, audio]);

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
      if (isActivelyPlaying) {
        ws?.seekTo(currentPercentage);
      }
    };

    ws?.on("ready", ready);

    return () => {
      ws?.un("ready", ready);
    };
  }, [isActivelyPlaying, audioUrl]);

  /**
   * Change current time based on the mouseclick on the waveform canvas
   */
  useEffect(() => {
    const ws = wavesurfer.current;

    const seek = (e: React.MouseEvent<HTMLElement>) => {
      const { left, width } = e.currentTarget.getBoundingClientRect();
      const newCurrentTime = ((e.clientX - left) / width) * duration;
      if (audioRef) audioRef.currentTime = newCurrentTime;
      dispatch({ type: "SET_CURRENT_TIME", payload: newCurrentTime });
    };

    if (ready && isActivelyPlaying) {
      ws?.drawer.on("click", seek);
    }

    return () => {
      ws?.drawer.un("click", seek);
    };
  }, [duration, ready, isActivelyPlaying]);

  /**
   * Update the waveform progress based on current Time
   */
  useEffect(() => {
    const ws = wavesurfer.current;

    if (ready && isActivelyPlaying) {
      ws?.seekTo(currentPercentage);
    }
  }, [ready, isActivelyPlaying, currentPercentage]);

  const clickPlayButton = useCallback(() => {
    if (isActivelyPlaying) {
      dispatch({ type: "SET_PLAYING", payload: !isPlaying });
    } else {
      dispatch({
        type: "SET_NEW_QUEUE",
        payload: mapAudioForAudioQueue(audio),
        index: 0,
      });
    }
  }, [isActivelyPlaying, isPlaying, audio.id]);

  return (
    <Flex>
      <Flex justifyContent="center" alignItems="center" width="100px">
        <Tooltip label="Play" placement="top">
          <span>
            <IconButton
              isRound
              colorScheme="pink"
              size="lg"
              icon={isPlaying && isActivelyPlaying ? <FaPause /> : <FaPlay />}
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
