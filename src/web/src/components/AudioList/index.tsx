import { List, ListItem, Stack } from "@chakra-ui/layout";
import { chakra } from "@chakra-ui/system";
import React, {
  createContext,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import { Audio } from "~/lib/types";
import { AudioListViewLayout } from "./FilterListInput";

interface IAudioListProps {
  audios: Audio[];
  renderItem: (audio: Audio) => React.ReactNode;
  children?: React.ReactNode;
  customNoAudiosFoundMessage?: string;
}

interface IAudioListContext {
  audios: Audio[];
  viewLayout: AudioListViewLayout;
  filterTerm: string;

  setViewLayout: (newView: AudioListViewLayout) => void;
  setFilterTerm: (newTerm: string) => void;
}

export const AudioListContext = createContext<IAudioListContext>(
  {} as IAudioListContext
);

export function useAudioList(): IAudioListContext {
  const context = useContext(AudioListContext);
  if (!context) throw Error("Unable to find Audio List Context.");
  return context;
}

export default function AudioList(props: IAudioListProps) {
  const {
    audios: initAudios,
    renderItem,
    children,
    customNoAudiosFoundMessage = "No audios found.",
  } = props;
  const [layout, setLayout] = useState<AudioListViewLayout>("list");
  const [filterTerm, setFilterTerm] = useState("");
  const [audios, setAudios] = useState<Audio[]>(initAudios);

  const handleFilterChange = (input: string) => {
    setFilterTerm(input);
  };

  const handleViewChange = (view: AudioListViewLayout) => {
    setLayout(view);
  };

  useEffect(() => {
    if (!filterTerm) {
      setAudios(audios);
      return;
    }
    const transformedTerm = filterTerm.toLowerCase();
    const filtered = audios.filter(
      (x) =>
        x.title.toLowerCase().includes(transformedTerm) ||
        x.user.userName.toLowerCase().includes(transformedTerm)
    );
    setAudios(filtered);
  }, [audios, filterTerm]);

  const values: IAudioListContext = useMemo(
    () => ({
      audios: audios,
      filterTerm: filterTerm,
      setFilterTerm: handleFilterChange,
      viewLayout: layout,
      setViewLayout: handleViewChange,
    }),
    [audios, layout, filterTerm, handleFilterChange, handleViewChange]
  );

  return (
    <AudioListContext.Provider value={values}>
      <chakra.div>
        <Stack direction="column">
          <Stack direction="row" spacing={4}>
            {children}
          </Stack>
          <chakra.div>
            {audios.length === 0 ? (
              <chakra.div
                display="flex"
                justifyContent="center"
                alignItems="center"
              >
                {customNoAudiosFoundMessage}
              </chakra.div>
            ) : (
              <List>
                {audios.map((item) => (
                  <ListItem key={item.id}>{renderItem(item)}</ListItem>
                ))}
              </List>
            )}
          </chakra.div>
        </Stack>
      </chakra.div>
    </AudioListContext.Provider>
  );
}
