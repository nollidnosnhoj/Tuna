import { AudioPlayerAction } from "../actions";
import { REPEAT_MODE, AudioPlayerItem } from '../types'
import { AudioPlayerState } from "../AudioPlayerContext";


function getCurrentAudio(list: AudioPlayerItem[], index?: number) {
  return index === undefined ? undefined : list[index];
}

export function audioPlayerReducer(
  state: AudioPlayerState,
  action: AudioPlayerAction
): AudioPlayerState {
  switch (action.type) {
    case "SET_AUDIO_REF": {
      return {
        ...state,
        audioRef: action.payload,
      };
    }
    case "SET_NEW_QUEUE": {
      const { payload, index } = action;
      if (payload.length === 0) return state;
      return {
        ...state,
        queue: payload,
        playIndex: index,
        isPlaying: true,
        currentAudio: getCurrentAudio(payload, index),
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
        currentAudio: getCurrentAudio(newQueue, newPlayIndex),
      };
    }
    case "CLEAR_QUEUE": {
      return {
        ...state,
        queue: [],
        playIndex: undefined,
        currentAudio: undefined,
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
    case "TOGGLE_PLAYING": {
      const { isPlaying } = state;
      return {
        ...state,
        isPlaying: !isPlaying,
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
        currentAudio: getCurrentAudio(queue, newIndex),
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
        currentAudio: getCurrentAudio(queue, newIndex),
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
        currentAudio: getCurrentAudio(queue, newIndex),
      };
    }
    default: {
      return state;
    }
  }
}