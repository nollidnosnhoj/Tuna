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

  useEffect(() => {
    const setting = JSON.parse(
      localStorage.getItem(AUDIO_PLAYER_SETTING) || ""
    );

    if (setting) {
      const parsedVolume = parseInt(setting.volume);
      if (!isNaN(parsedVolume)) {
        dispatch({ type: "SET_VOLUME", payload: parsedVolume });
      }
      if (Object.values(REPEAT_MODE).includes(setting.repeat)) {
        dispatch({
          type: "SET_REPEAT",
          payload: setting.repeat,
        });
      }
    }
  }, []);

  useEffect(() => {
    const saveTimer = setTimeout(() => {
      const settings = {
        volume: volume,
        repeat: repeat,
      };

      localStorage.setItem(AUDIO_PLAYER_SETTING, JSON.stringify(settings));
    }, 200);

    return () => {
      clearTimeout(saveTimer);
    };
  }, [repeat, volume]);

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
