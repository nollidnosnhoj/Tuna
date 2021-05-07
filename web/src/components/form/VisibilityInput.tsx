import { AddIcon } from "@chakra-ui/icons";
import { ButtonGroup, Button, IconButton } from "@chakra-ui/react";
import React from "react";

interface VisibilityInputProps {}

const VisibilityInput: React.FC<VisibilityInputProps> = (props) => {
  return (
    <ButtonGroup size="sm" isAttached variant="outline">
      <Button>Public</Button>
      <Button>Unlisted</Button>
      <Button>Private</Button>
    </ButtonGroup>
  );
};
