import React, {
  createContext,
  PropsWithChildren,
  useCallback,
  useEffect,
  useMemo,
  useState,
} from "react";
import { AudioPlayerItem } from "~/features/audio/types";
import api from "~/utils/api";

type AudioQueueContexType = {
  audioList: AudioPlayerItem[];
  playIndex: number | undefined;
  setNewQueue: (
    newQueue: AudioPlayerItem[],
    index?: number | undefined
  ) => Promise<number>;
  addToQueue: (audios: AudioPlayerItem[]) => Promise<void>;
  clearQueue: () => Promise<void>;
  goToIndex: (index: number) => Promise<number>;
  removeFromQueue: (index: number) => Promise<number>;
};

export const AudioQueueContext = createContext<AudioQueueContexType>(
  {} as AudioQueueContexType
);

export default function AudioQueueProvider(props: PropsWithChildren<any>) {
  const [audioList, setAudioList] = useState<AudioPlayerItem[]>([]);
  const [playIndex, setPlayIndex] = useState<number | undefined>(undefined);

  const setNewQueue = (newQueue: AudioPlayerItem[], index?: number) => {
    return new Promise<number>((resolve) => {
      setAudioList(newQueue);
      setPlayIndex(index ?? 0);
      return resolve(index ?? 0);
    });
  };

  const addToQueue = (audios: AudioPlayerItem[]) => {
    return new Promise<void>((resolve) => {
      setAudioList((prev) => [...prev, ...audios]);
      return resolve();
    });
  };

  const clearQueue = () => {
    return new Promise<void>((resolve) => {
      setAudioList((prev) => prev.filter((_, i) => i === playIndex));
      setPlayIndex(0);
      return resolve();
    });
  };

  const goToIndex = (index: number) => {
    return new Promise<number>((resolve) => {
      let newIndex = Math.max(0, Math.min(audioList.length - 1, index));
      setPlayIndex(newIndex);
      return resolve(newIndex);
    });
  };

  const removeFromQueue = useCallback(
    (index: number) => {
      return new Promise<number>((resolve, reject) => {
        const boundedIndex = Math.max(0, Math.min(index, audioList.length - 1));
        // Do not remove audio that is currently playing
        if (playIndex === boundedIndex)
          return reject("Cannot remove audio that is now playing.");

        const nowQueueId = audioList.find((_, i) => i === playIndex)?.queueId;
        const newList = audioList.filter((_, i) => i !== boundedIndex);
        let newIndex = newList.findIndex((a) => a.queueId == nowQueueId);
        if (newIndex === -1) {
          return reject(
            "Cannot find the index of which the audio is currently playing. Check your algorithm haha."
          );
        }

        setPlayIndex(newIndex);
        setAudioList(newList);

        return resolve(newIndex);
      });
    },
    [audioList, playIndex]
  );

  // If there's nothing in the list, play index should be undefined
  useEffect(() => {
    if (audioList.length === 0) {
      setPlayIndex(undefined);
    }
  }, [audioList.length, setPlayIndex]);

  useEffect(() => {
    console.log(playIndex);
  }, [playIndex]);

  const values: AudioQueueContexType = useMemo(
    () => ({
      audioList: audioList,
      playIndex: playIndex,
      setNewQueue: setNewQueue,
      addToQueue: addToQueue,
      clearQueue: clearQueue,
      goToIndex: goToIndex,
      removeFromQueue: removeFromQueue,
    }),
    [
      audioList,
      playIndex,
      setNewQueue,
      addToQueue,
      clearQueue,
      goToIndex,
      removeFromQueue,
    ]
  );

  return (
    <AudioQueueContext.Provider value={values}>
      {props.children}
    </AudioQueueContext.Provider>
  );
}
