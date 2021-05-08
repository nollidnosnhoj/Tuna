import { useContext } from "react";
import { AudioPlayerContext } from "../contexts/AudioPlayerContext";

export function useAudioPlayer() {
  const context = useContext(AudioPlayerContext);
  if (!context) throw new Error("Cannot find AudioPlayerContext.");
  return context;
}
