import {
  InputGroup,
  InputLeftAddon,
  Input,
  useClipboard,
} from "@chakra-ui/react";
import React from "react";
import { toast } from "~/utils";

interface AudioShareItemProps {
  label: string;
  value: string;
}

export default function AudioShareItem(props: AudioShareItemProps) {
  const { onCopy } = useClipboard(props.value);

  return (
    <InputGroup>
      <InputLeftAddon>{props.label}</InputLeftAddon>
      <Input
        isReadOnly
        value={props.value}
        onClick={(event) => {
          event.currentTarget.select();
          onCopy();
          toast("info", {
            description: "Link copied.",
            duration: 3000,
            isClosable: false,
          });
        }}
      />
    </InputGroup>
  );
}
