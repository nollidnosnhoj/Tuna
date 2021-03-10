import { useColorMode } from "@chakra-ui/react";
import React, { useCallback, useEffect, useState } from "react";
import ReactJkMusicPlayer, {
  ReactJkMusicPlayerInstance,
  ReactJkMusicPlayerProps,
  ReactJkMusicPlayerTheme,
} from "react-jinke-music-player";
import "react-jinke-music-player/assets/index.css";
import { useAudioPlayer } from "~/contexts/audioPlayerContext";
import {
  AudioPlayerItemInfo,
  AudioPlayerListItem,
} from "../../features/audio/types";

export default function AudioPlayer() {
  const {
    audioList,
    playIndex,
    clearPriorAudioList,
    clearQueue,
    syncQueue,
    volume,
    volumeChange,
    setPlaying,
  } = useAudioPlayer();
  const { colorMode, setColorMode } = useColorMode();

  // const getAudioInstance = useCallback(
  //   (instance: ReactJkMusicPlayerInstance) => {},
  //   []
  // );

  const onAudioListsChange = useCallback(
    (
      currentPlayId: string,
      audioLists: AudioPlayerListItem[],
      audioInfo: AudioPlayerItemInfo
    ) => {
      syncQueue(audioLists);
    },
    []
  );

  const onAudioPlayTrackChange = useCallback(
    (
      currentPlayId: string,
      audioLists: AudioPlayerListItem[],
      audioInfo: AudioPlayerItemInfo
    ) => {},
    []
  );

  const onAudioPlay = useCallback(
    (audioInfo: AudioPlayerItemInfo) => {
      setPlaying(true);
    },
    [setPlaying]
  );

  const onAudioPause = useCallback(
    (audioInfo: AudioPlayerItemInfo) => {
      setPlaying(false);
    },
    [setPlaying]
  );

  const onThemeChange = useCallback(
    (theme: ReactJkMusicPlayerTheme) => {
      setColorMode(theme);
    },
    [setColorMode]
  );

  const onAudioVolumeChange = useCallback(
    (volume: number) => {
      volumeChange(Math.sqrt(volume));
    },
    [volumeChange]
  );

  const onBeforeDestroy = useCallback(
    (
      currentPlayId: string,
      audioLists: AudioPlayerListItem[],
      audioInfo: AudioPlayerItemInfo
    ) => {
      return new Promise<void>((resolve, reject) => {
        clearQueue();
        resolve();
      });
    },
    []
  );

  const onAudioError = useCallback(
    (
      error: any,
      currentPlayId: string,
      audioLists: AudioPlayerListItem[],
      audioInfo: AudioPlayerItemInfo
    ) => {
      console.log("error", error);
    },
    []
  );

  const defaultOptions: ReactJkMusicPlayerProps = {
    audioLists: [],
    toggleMode: false,
    mode: "full",
    quietUpdate: true,
    bounds: "body",
    preload: true,
    autoPlayInitLoadPlayList: true,
    loadAudioErrorPlayNext: false,
    clearPriorAudioLists: false,
    showDestroy: false,
    showDownload: false,
    showReload: false,
    glassBg: true,
    showThemeSwitch: false,
    showMediaSession: true,
    defaultPosition: {
      top: 300,
      left: 120,
    },
    volumeFade: {
      fadeIn: 200,
      fadeOut: 200,
    },
  };

  useEffect(() => {});

  if (audioList.length === 0) return null;

  return (
    <ReactJkMusicPlayer
      {...defaultOptions}
      audioLists={audioList}
      theme={colorMode}
      playIndex={playIndex}
      defaultVolume={volume}
      // getAudioInstance={getAudioInstance}
      onAudioListsChange={onAudioListsChange}
      onAudioPlayTrackChange={onAudioPlayTrackChange}
      onAudioPlay={onAudioPlay}
      onAudioPause={onAudioPause}
      onAudioError={onAudioError}
      onThemeChange={onThemeChange}
      onAudioVolumeChange={onAudioVolumeChange}
      onBeforeDestroy={onBeforeDestroy}
      clearPriorAudioLists={clearPriorAudioList}
      autoPlay={clearPriorAudioList || playIndex === 0}
      volumeFade={{ fadeIn: 0, fadeOut: 0 }}
    />
  );
}
