import { useContext } from "react";
import { AudioQueueContext } from "../contexts/audioQueueContext";

export default function useAudioQueue() {
  let context = useContext(AudioQueueContext);
  if (!context) throw new Error("Please implement AudioQueueContext.");
  return context;
}
