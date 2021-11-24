import { ButtonGroup, IconButton } from "@chakra-ui/button";
import { Input } from "@chakra-ui/input";
import { List, ListItem, Spacer, Stack } from "@chakra-ui/layout";
import { chakra } from "@chakra-ui/system";
import React, { useEffect, useState } from "react";
import { HiViewGrid, HiViewList } from "react-icons/hi";
import { Audio } from "~/lib/types";

type LayoutTypes = "stack" | "grid";

interface IAudioListProps {
  audios: Audio[];
  children: (audio: Audio) => React.ReactNode;
  defaultLayout?: LayoutTypes;
  customNoAudiosFoundMessage?: string;
  hideLayoutButtons?: boolean;
  hideSearchBar?: boolean;
}

export default function AudioList(props: IAudioListProps) {
  const {
    audios,
    children,
    defaultLayout = "stack",
    customNoAudiosFoundMessage = "No audios found.",
    hideLayoutButtons = false,
    hideSearchBar = false,
  } = props;
  const [layout, setLayout] = useState<LayoutTypes>(defaultLayout);
  const [filterTerm, setFilterTerm] = useState("");
  const [filteredAudios, setFilteredAudios] = useState<Audio[]>(audios);

  useEffect(() => {
    if (!filterTerm) {
      setFilteredAudios(audios);
      return;
    }
    const transformedTerm = filterTerm.toLowerCase();
    const filtered = audios.filter(
      (x) =>
        x.title.toLowerCase().includes(transformedTerm) ||
        x.user.userName.toLowerCase().includes(transformedTerm)
    );
    setFilteredAudios(filtered);
  }, [audios, filterTerm]);

  return (
    <chakra.div>
      <Stack direction="column">
        <Stack direction="row" spacing={4}>
          <Spacer />
          {!hideLayoutButtons && (
            <ButtonGroup variant="ghost" spacing="2">
              <IconButton
                aria-label="Stack"
                icon={<HiViewList opacity={layout === "stack" ? 1 : 0.5} />}
                onClick={() => setLayout("stack")}
              />
              <IconButton
                aria-label="Grid"
                icon={<HiViewGrid opacity={layout === "grid" ? 1 : 0.5} />}
                onClick={() => setLayout("grid")}
              />
            </ButtonGroup>
          )}
          {!hideSearchBar && (
            <chakra.div>
              <Input
                value={filterTerm}
                onChange={(evt) => setFilterTerm(evt.target.value)}
                variant="outline"
                placeholder="Outline"
              />
            </chakra.div>
          )}
        </Stack>
        <chakra.div>
          {filteredAudios.length === 0 ? (
            <chakra.div
              display="flex"
              justifyContent="center"
              alignItems="center"
            >
              {customNoAudiosFoundMessage}
            </chakra.div>
          ) : (
            <List>
              {filteredAudios.map((item) => (
                <ListItem key={item.id}>{children(item)}</ListItem>
              ))}
            </List>
          )}
        </chakra.div>
      </Stack>
    </chakra.div>
  );
}
