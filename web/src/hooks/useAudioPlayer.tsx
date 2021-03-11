import { useContext } from "react";
import { AudioPlayerContext } from "../contexts/audioPlayerContext";

export default function useAudioPlayer() {
  return useContext(AudioPlayerContext);
}
