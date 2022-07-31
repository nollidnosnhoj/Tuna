import { Input } from "@chakra-ui/react";
import React from "react";
import { useAudioList } from ".";

export type AudioListViewLayout = "list" | "grid";

export default function AudioListFilterInput() {
  const { filterTerm, setFilterTerm } = useAudioList();

  return (
    <Input
      placeholder="Filter"
      value={filterTerm}
      onChange={(evt) => setFilterTerm(evt.target.value)}
    />
  );
}
