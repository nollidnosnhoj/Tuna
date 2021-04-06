import React, {
  PropsWithChildren,
  useEffect,
  useMemo,
  useReducer,
} from "react";
import { AudioPlayerItem } from "~/components/AudioPlayer/types";

export enum REPEAT_MODE {
  DISABLE = "disable",
  REPEAT = "repeat",
  REPEAT_SINGLE = "repeat-one",
}

export interface AudioPlayerState {
  currentPlaying?: AudioPlayerItem;
  queue: AudioPlayerItem[];
  playIndex?: number;
  isPlaying: boolean;
  repeat: REPEAT_MODE;
  currentTime?: number;
  volume: number;
}

const defaultState: AudioPlayerState = {
  currentPlaying: undefined,
  queue: [],
  playIndex: undefined,
  isPlaying: false,
  repeat: REPEAT_MODE.DISABLE,
  currentTime: undefined,
  volume: 50,
};

type AudioPlayerAction =
  | {
      type: "PLAY_PREVIOUS" | "PLAY_NEXT" | "CLEAR_QUEUE" | "UPDATE_CURRENT";
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
      type: "SET_NEW_QUEUE";
      payload: AudioPlayerItem[];
      index?: number;
    }
  | {
      type: "ADD_TO_QUEUE";
      payload: AudioPlayerItem[];
    };

type AudioPlayerContextType = {
  state: AudioPlayerState;
  dispatch: React.Dispatch<AudioPlayerAction>;
};

export function audioPlayerReducer(
  state: AudioPlayerState,
  action: AudioPlayerAction
) {
  switch (action.type) {
    case "SET_NEW_QUEUE": {
      const { payload, index } = action;
      if (payload.length === 0) return state;
      return {
        ...state,
        queue: payload,
        playIndex: index ?? 0,
        currentPlaying: payload[index ?? 0],
        isPlaying: true,
      };
    }
    case "ADD_TO_QUEUE": {
      const { queue } = state;
      const { payload } = action;
      return {
        ...state,
        queue: [...queue, ...payload],
      };
    }
    case "REMOVE_FROM_QUEUE": {
      const { queue, playIndex } = state;
      const { payload } = action;
      let newPlayIndex = playIndex;
      if (playIndex !== undefined && payload < playIndex) {
        newPlayIndex = playIndex - 1;
      }
      const newQueue = [...queue].filter((_, i) => i !== payload);

      if (newQueue.length === 0) {
        newPlayIndex = undefined;
      }

      return {
        ...state,
        queue: newQueue,
        playIndex: newPlayIndex,
        currentPlaying:
          newPlayIndex === undefined ? undefined : newQueue[newPlayIndex],
      };
    }
    case "CLEAR_QUEUE": {
      return {
        ...state,
        queue: [],
        playIndex: undefined,
        currentPlaying: undefined,
      };
    }
    case "SET_VOLUME": {
      return {
        ...state,
        volume: action.payload,
      };
    }
    case "SET_PLAYING": {
      return {
        ...state,
        isPlaying: action.payload,
      };
    }
    case "SET_CURRENT_TIME": {
      return {
        ...state,
        currentTime: action.payload,
      };
    }
    case "SET_PLAY_INDEX": {
      const { queue } = state;
      const { payload } = action;
      const newIndex = Math.max(0, Math.min(payload, queue.length - 1));
      return {
        ...state,
        isPlaying: true,
        playIndex: newIndex,
        currentPlaying: queue[newIndex],
      };
    }
    case "SET_REPEAT": {
      return {
        ...state,
        repeat: action.payload,
      };
    }
    case "PLAY_PREVIOUS": {
      const { playIndex, queue } = state;
      if (playIndex === undefined) return state;
      const newIndex = Math.max(0, Math.min(queue.length - 1, playIndex - 1));
      return {
        ...state,
        isPlaying: true,
        playIndex: newIndex,
        currentPlaying: queue[newIndex],
      };
    }
    case "PLAY_NEXT": {
      const { playIndex, queue, repeat } = state;
      if (playIndex === undefined) return state;
      let newIndex = playIndex + 1;
      if (newIndex > queue.length - 1) {
        newIndex = repeat === REPEAT_MODE.REPEAT ? 0 : queue.length - 1;
      }
      return {
        ...state,
        isPlaying: true,
        playIndex: newIndex,
        currentPlaying: queue[newIndex],
      };
    }
    case "UPDATE_CURRENT": {
      const { queue, playIndex } = state;
      return {
        ...state,
        currentPlaying: playIndex ? queue[playIndex] : undefined,
      };
    }
    default: {
      return state;
    }
  }
}

export const AudioPlayerContext = React.createContext<AudioPlayerContextType>(
  {} as AudioPlayerContextType
);

export default function AudioPlayerProvider(props: PropsWithChildren<any>) {
  const [state, dispatch] = useReducer(audioPlayerReducer, defaultState);

  const contextType = useMemo(() => {
    return { state, dispatch };
  }, [state, dispatch]);

  return (
    <AudioPlayerContext.Provider value={contextType}>
      {props.children}
    </AudioPlayerContext.Provider>
  );
}
