import React, {
  PropsWithChildren,
  useContext,
  useEffect,
  useMemo,
  useReducer,
} from "react";
import { AudioPlayerItem } from "~/components/AudioPlayer/types";
import { AudioPlayerAction } from "./actions/audioPlayerActions";
import { audioPlayerReducer } from "./reducers/audioPlayerReducer";

export enum REPEAT_MODE {
  DISABLE = "disable",
  REPEAT = "repeat",
  REPEAT_SINGLE = "repeat-one",
}

export interface AudioPlayerState {
  audioRef: HTMLAudioElement | null;
  currentAudio?: AudioPlayerItem;
  queue: AudioPlayerItem[];
  playIndex?: number;
  isPlaying: boolean;
  repeat: REPEAT_MODE;
  currentTime?: number;
  volume: number;
}

const defaultState: AudioPlayerState = {
  audioRef: null,
  currentAudio: undefined,
  queue: [],
  playIndex: undefined,
  isPlaying: false,
  repeat: REPEAT_MODE.DISABLE,
  currentTime: undefined,
  volume: 25,
};

type AudioPlayerContextType = {
  state: AudioPlayerState;
  dispatch: React.Dispatch<AudioPlayerAction>;
};

export const AudioPlayerContext = React.createContext<AudioPlayerContextType>(
  {} as AudioPlayerContextType
);

const AUDIO_PLAYER_SETTING = "audiochan_player_setting";

export default function AudioPlayerProvider(props: PropsWithChildren<any>) {
  const [state, dispatch] = useReducer(audioPlayerReducer, defaultState);

  const contextType = useMemo(() => {
    return { state, dispatch };
  }, [state, dispatch]);

  const { volume, repeat, currentAudio } = state;

  /**
   * When the audio player component loads, load the settings from local storage
   */
  useEffect(() => {
    let settingString = localStorage.getItem(AUDIO_PLAYER_SETTING) || "";
    if (settingString) {
      var setting = JSON.parse(settingString);
      const { volume, repeat } = setting;
      const parsedVolume = parseInt(volume);
      if (!isNaN(parsedVolume)) {
        dispatch({ type: "SET_VOLUME", payload: parsedVolume });
      }
      if (Object.values(REPEAT_MODE).includes(repeat)) {
        dispatch({
          type: "SET_REPEAT",
          payload: repeat,
        });
      }
    }
  }, []);

  /**
   * Whenever the volume and repeat changes, save it into the localstorage for persistence
   */
  useEffect(() => {
    const saveTimer = setTimeout(() => {
      localStorage.setItem(
        AUDIO_PLAYER_SETTING,
        JSON.stringify({
          volume,
          repeat,
        })
      );
    }, 200);

    return () => {
      clearTimeout(saveTimer);
    };
  }, [repeat, volume]);

  /**
   * When a new song is queued, set the time at the beginning.
   */
  useEffect(() => {
    dispatch({ type: "SET_CURRENT_TIME", payload: 0 });
  }, [currentAudio?.queueId]);

  return (
    <AudioPlayerContext.Provider value={contextType}>
      {props.children}
    </AudioPlayerContext.Provider>
  );
}

export const useAudioPlayer = () => {
  const context = useContext(AudioPlayerContext);
  if (!context) throw new Error("Cannot find AudioPlayerContext.");
  return context;
};
