import { useContext } from "react";
import { AudioPlayerContext } from "~/contexts/AudioPlayerContext";

export default function useAudioPlayer() {
  const context = useContext(AudioPlayerContext);
  if (!context) throw new Error("Cannot find AudioPlayerContext.");
  return context;
}