import { REPEAT_MODE, AudioPlayerItem } from "../types";

export type AudioPlayerAction =
  | {
      type: "PLAY_PREVIOUS" | "PLAY_NEXT" | "CLEAR_QUEUE" | "TOGGLE_PLAYING";
    }
  | {
      type: "REMOVE_AUDIO_ID_FROM_QUEUE";
      payload: string;
    }
  | {
      type: "SET_PLAYING";
      payload: boolean;
    }
  | {
      type:
        | "SET_VOLUME"
        | "SET_CURRENT_TIME"
        | "SET_PLAY_INDEX"
        | "REMOVE_FROM_QUEUE";
      payload: number;
    }
  | {
      type: "SET_REPEAT";
      payload: REPEAT_MODE;
    }
  | {
      type: "SET_AUDIO_REF";
      payload: HTMLAudioElement | null;
    }
  | {
      type: "SET_NEW_QUEUE";
      payload: AudioPlayerItem[];
      index?: number;
    }
  | {
      type: "ADD_TO_QUEUE";
      payload: AudioPlayerItem[];
    };
