import React, {
  createContext,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";

interface AudioPlayerContextProps {
  volume: number;
  handleVolume: (level: number) => void;
}

const AudioPlayerContext = createContext<AudioPlayerContextProps>(null);

export function AudioPlayerProvider(props) {
  const [volume, setVolume] = useState<number>(0);

  useEffect(() => {
    setVolume(parseFloat(window.localStorage.getItem("playerVolume") || "0.7"));
  }, []);

  const onVolumeChange = (level: number) => {
    level = Math.min(level, 1);
    level = Math.max(0, level);
    setVolume(level);
    if (typeof window !== "undefined") {
      window.localStorage.setItem("playerVolume", JSON.stringify(level));
    }
  };

  const values = useMemo<AudioPlayerContextProps>(
    () => ({
      volume,
      handleVolume: onVolumeChange,
    }),
    [volume]
  );

  return (
    <AudioPlayerContext.Provider value={values}>
      {props.children}
    </AudioPlayerContext.Provider>
  );
}

export function useAudioPlayer() {
  return useContext(AudioPlayerContext);
}
