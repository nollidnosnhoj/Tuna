import React, { useEffect } from "react";
import {
  Box,
  Button,
  ButtonGroup,
  Flex,
  IconButton,
  Popover,
  PopoverArrow,
  PopoverBody,
  PopoverCloseButton,
  PopoverContent,
  PopoverFooter,
  PopoverHeader,
  PopoverTrigger,
  Spacer,
} from "@chakra-ui/react";
import { Controller, FieldValues, useForm } from "react-hook-form";
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import InputField from "~/components/InputField";
import TagInput from "~/components/TagInput";
import { AudioRequest } from "~/lib/types";
import { DeleteIcon } from "@chakra-ui/icons";

interface AudioFormProps {
  type: "create" | "edit";
  currentValues?: AudioRequest;
  onSubmit: (values: FieldValues) => Promise<void>;
  onDelete?: () => Promise<void>;
}

const AudioForm = ({ type, currentValues, ...props }: AudioFormProps) => {
  const {
    register,
    reset,
    handleSubmit,
    control,
    errors,
    formState: { isSubmitting },
  } = useForm<AudioRequest>({
    defaultValues: currentValues,
    resolver: yupResolver(
      yup.object().shape({
        title: yup.string().required().max(30),
        description: yup.string().max(500),
        tags: yup.array(yup.string()).max(10).ensure(),
        isPublic: yup.boolean(),
      })
    ),
  });

  useEffect(() => {
    reset(currentValues);
  }, [reset, currentValues]);

  const onSubmit = async (values: AudioRequest) => {
    await props.onSubmit(values);
  };

  const onDelete = async () => {
    await props.onDelete();
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <InputField
        name="title"
        type="text"
        ref={register}
        label="Title"
        error={errors.title}
        isRequired
      />
      <InputField
        name="description"
        ref={register}
        label="Description"
        error={errors.description}
        isTextArea
      />
      <Controller
        name="tags"
        control={control}
        render={({ name, value, onChange }) => (
          <TagInput
            name={name}
            value={value}
            onChange={onChange}
            error={errors.tags && errors.tags[0]}
          />
        )}
      />
      <Flex marginY={4}>
        <Box>
          {type === "edit" && (
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
                  Are you sure you want to remove this upload? You cannot undo
                  this action.
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
          )}
        </Box>
        <Spacer />
        <Box>
          <Button
            colorScheme="blue"
            type="submit"
            isLoading={isSubmitting}
            loadingText="Submitting"
          >
            {type === "create" ? "Upload" : "Modify"}
          </Button>
        </Box>
      </Flex>
    </form>
  );
};

export default AudioForm;
