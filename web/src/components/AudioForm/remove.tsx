import { DeleteIcon } from "@chakra-ui/icons";
import {
  Box,
  Popover,
  PopoverTrigger,
  IconButton,
  PopoverContent,
  PopoverArrow,
  PopoverCloseButton,
  PopoverHeader,
  PopoverBody,
  PopoverFooter,
  ButtonGroup,
  Button,
} from "@chakra-ui/react";
import React from "react";

interface AudioFormRemoveProps {
  isSubmitting: boolean;
  onDelete: () => void;
}

export default function ({ isSubmitting, onDelete }: AudioFormRemoveProps) {
  return (
    <Box>
      <Popover>
        <PopoverTrigger>
          <IconButton
            colorScheme="red"
            variant="outline"
            aria-label="Remove upload"
            icon={<DeleteIcon />}
            isLoading={isSubmitting}
          >
            Delete
          </IconButton>
        </PopoverTrigger>
        <PopoverContent>
          <PopoverArrow />
          <PopoverCloseButton />
          <PopoverHeader>Remove Confirmation</PopoverHeader>
          <PopoverBody>
            Are you sure you want to remove this upload? You cannot undo this
            action.
          </PopoverBody>
          <PopoverFooter d="flex" justifyContent="flex-end">
            <ButtonGroup size="sm">
              <Button
                colorScheme="red"
                onClick={onDelete}
                disabled={isSubmitting}
              >
                Remove
              </Button>
            </ButtonGroup>
          </PopoverFooter>
        </PopoverContent>
      </Popover>
    </Box>
  );
}
