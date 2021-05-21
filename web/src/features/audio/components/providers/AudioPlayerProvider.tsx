import React, {
  PropsWithChildren,
  useEffect,
  useMemo,
  useReducer,
} from "react";
import { AudioPlayerContext } from "../../contexts";
import { audioPlayerReducer } from "../../contexts/reducers";
import { REPEAT_MODE } from "../../contexts/types";

const AUDIO_PLAYER_SETTING = "audiochan_player_setting";

export function AudioPlayerProvider(props: PropsWithChildren<unknown>) {
  const [state, dispatch] = useReducer(audioPlayerReducer, {
    audioRef: null,
    currentAudio: undefined,
    queue: [],
    playIndex: undefined,
    isPlaying: false,
    repeat: REPEAT_MODE.DISABLE,
    currentTime: undefined,
    volume: 25,
  });

  const contextType = useMemo(() => {
    return { state, dispatch };
  }, [state, dispatch]);

  const { volume, repeat, currentAudio } = state;

  /**
   * When the audio player component loads, load the settings from local storage
   */
  useEffect(() => {
    const settingString = localStorage.getItem(AUDIO_PLAYER_SETTING) || "";
    if (settingString) {
      const setting = JSON.parse(settingString);
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
