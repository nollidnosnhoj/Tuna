import React, {
  createContext,
  PropsWithChildren,
  useEffect,
  useMemo,
  useState,
} from "react";
import { useMutex } from "react-context-mutex";
import { AudioPlayerItem } from "~/features/audio/types";

export enum REPEAT_MODE {
  DISABLE,
  REPEAT,
  REPEAT_SINGLE,
}

interface IAudioPlayerContext {
  nowPlaying: AudioPlayerItem | undefined;
  isPlaying: boolean;
  audioList: AudioPlayerItem[];
  playIndex: number | undefined;
  volume: number;
  repeatMode: REPEAT_MODE;
  addToQueue: (newAudiosInQueue: AudioPlayerItem[]) => void;
  clearQueue: () => void;
  startPlay: (newRelatedAudios: AudioPlayerItem[], index: number) => void;
  playPrevious: () => void;
  playNext: (onEnded: boolean) => void;
  changePlaying: (state?: boolean) => void;
  changeVolume: (volumeLevel: number) => void;
  changePlayIndex: (index: number) => void;
  changeRepeatMode: (mode: REPEAT_MODE) => void;
}

export const AudioPlayerContext = createContext<IAudioPlayerContext>(
  {} as IAudioPlayerContext
);

export default function AudioPlayerProvider(props: PropsWithChildren<any>) {
  const MutexRunner = useMutex();
  const mutex = new MutexRunner("audio_player");
  const [repeatMode, setRepeatMode] = useState<REPEAT_MODE>(
    REPEAT_MODE.DISABLE
  );
  const [audioList, setAudioList] = useState<AudioPlayerItem[]>([]);

  const [isPlaying, setIsPlaying] = useState(false);
  const [playIndex, setPlayIndex] = useState<number | undefined>(undefined);
  const [volume, setVolume] = useState<number>(() => {
    if (typeof window !== "undefined") {
      return (
        parseInt(window.localStorage.getItem("playerVolume") || "0.5") || 0.5
      );
    }

    return 0.5;
  });

  const nowPlaying = useMemo(() => {
    if (playIndex === undefined) return undefined;
    return audioList[playIndex];
  }, [audioList, playIndex]);

  const startPlay = (audios: AudioPlayerItem[], index: number) => {
    mutex.run(() => {
      mutex.lock();
      const boundedIndex = Math.max(0, Math.min(index, audios.length - 1));
      setAudioList(audios);
      setPlayIndex(boundedIndex);
      mutex.unlock();
    });
  };

  const addToQueue = (audios: AudioPlayerItem[]) => {
    mutex.run(() => {
      mutex.lock();
      setAudioList((prev) => [...prev, ...audios]);
      mutex.unlock();
    });
  };

  const changeVolume = (level: number) => {
    setVolume(level);
    if (typeof window !== "undefined") {
      localStorage.setItem("playerVolume", level + "");
    }
  };

  const clearQueue = () => {
    mutex.run(() => {
      mutex.lock();
      setAudioList((prev) => prev.filter((_, i) => i === playIndex));
      setPlayIndex(0);
      mutex.unlock();
    });
  };

  const changePlayingState = (state?: boolean) => {
    if (state === true) {
      setIsPlaying(() => true);
    } else if (state === false) {
      setIsPlaying(() => false);
    } else {
      setIsPlaying((prev) => !prev);
    }
  };

  const playPrevious = () => {
    if (playIndex === undefined) return;
    mutex.run(() => {
      mutex.lock();
      let newIndex = Math.max(0, Math.min(audioList.length - 1, playIndex - 1));
      setPlayIndex(newIndex);
      mutex.unlock();
    });
  };

  const playNext = (onEnded: boolean) => {
    if (playIndex === undefined) return;
    mutex.run(() => {
      mutex.lock();
      let newIndex: number | undefined = playIndex;
      switch (repeatMode) {
        case REPEAT_MODE.REPEAT_SINGLE:
        case REPEAT_MODE.DISABLE:
          if (!onEnded) {
            newIndex = playIndex + 1;
          }
          break;
        case REPEAT_MODE.REPEAT:
          newIndex = playIndex + 1;
          break;
      }

      if (
        newIndex > audioList.length - 1 &&
        repeatMode === REPEAT_MODE.REPEAT
      ) {
        newIndex = 0;
      }

      setPlayIndex(newIndex);
      mutex.unlock();
    });
  };

  const changePlayIndex = (index: number) => {
    if (playIndex === undefined) return;
    let newIndex = Math.max(0, Math.min(audioList.length - 1, index));
    setPlayIndex(newIndex);
  };

  const removeAudioFromList = (index: number) => {
    const boundedIndex = Math.max(0, Math.min(index, audioList.length - 1));
    // Do not remove audio that is currently playing
    if (playIndex === boundedIndex) return;

    mutex.run(() => {
      mutex.lock();
      // Get the queue Id of the audio that is currently playing, so we can get the index in the new filtered list
      const currentPlayingQueueId =
        audioList.find((_, i) => i === playIndex)?.queueId ?? "";
      const newList = audioList.filter((_, i) => i !== boundedIndex);
      let newIndex = newList.findIndex(
        (a) => a.queueId == currentPlayingQueueId
      );
      if (newIndex === -1) {
        throw new Error(
          "Cannot find the index of which the audio is currently playing. Check your algorithm haha."
        );
      } else {
        setPlayIndex(newIndex);
        setAudioList(newList);
      }
      mutex.unlock();
    });
  };

  const changeRepeat = (mode: REPEAT_MODE) => {
    setRepeatMode(mode);
  };

  // If there's nothing in the list, play index should be undefined
  useEffect(() => {
    if (audioList.length === 0) {
      setPlayIndex(undefined);
    }
  }, [audioList.length, setPlayIndex]);

  const values: IAudioPlayerContext = useMemo(
    () => ({
      nowPlaying: nowPlaying,
      isPlaying: isPlaying,
      audioList: audioList,
      playIndex: playIndex,
      volume: volume,
      repeatMode: repeatMode,
      addToQueue: addToQueue,
      clearQueue: clearQueue,
      startPlay: startPlay,
      playPrevious: playPrevious,
      playNext: playNext,
      changePlaying: changePlayingState,
      changeVolume: changeVolume,
      changePlayIndex: changePlayIndex,
      changeRepeatMode: changeRepeat,
    }),
    [
      isPlaying,
      audioList,
      playIndex,
      volume,
      repeatMode,
      addToQueue,
      clearQueue,
      startPlay,
      playPrevious,
      playNext,
      changePlayingState,
      changeVolume,
      changePlayIndex,
      changeRepeat,
      nowPlaying,
    ]
  );

  return (
    <AudioPlayerContext.Provider value={values}>
      {props.children}
    </AudioPlayerContext.Provider>
  );
}
