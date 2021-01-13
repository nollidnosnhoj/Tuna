import { Flex, Box, Button, Spacer, Checkbox } from "@chakra-ui/react";
import React from "react";
import Router from "next/router";
import { Controller, useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import AudioDropzone from "./Dropzone";
import InputCheckbox from "../Form/Checkbox";
import GenreSelect from "../Form/GenreSelect";
import TextInput from "../Form/TextInput";
import TagInput from "../Form/TagInput";
import { uploadAudio } from "~/lib/services/audio";
import { uploadAudioSchema } from "~/lib/validationSchemas";
import { apiErrorToast, successfulToast } from "~/utils/toast";
import { UploadAudioRequest } from "~/lib/types/audio";

const AudioUpload = () => {
  const onSubmit = async (values: UploadAudioRequest) => {
    var formData = new FormData();

    Object.entries(values).forEach(([key, value]) => {
      if (Array.isArray(value)) {
        value.forEach((val) => formData.append(key, val));
      } else if (value instanceof File) {
        formData.append(key, value);
      } else {
        formData.append(key, value.toString());
      }
    });

    try {
      const { id: audioId } = await uploadAudio(formData);
      successfulToast({
        title: "Audio uploaded!",
        message: "You will be redirected...",
      });
      Router.push(`audios/${audioId}`);
    } catch (err) {
      apiErrorToast(err);
    }
  };

  const {
    handleSubmit,
    register,
    errors,
    control,
    formState: { isSubmitting },
  } = useForm<UploadAudioRequest>({
    defaultValues: {
      file: undefined,
      title: "",
      description: "",
      tags: [],
      isPublic: true,
      genre: "",
      acceptTerms: false,
    },
    resolver: yupResolver(uploadAudioSchema),
  });

  return (
    <Flex justify="center">
      <Box width="100%">
        <form onSubmit={handleSubmit(onSubmit)}>
          <Controller
            name="file"
            control={control}
            render={({ name, onChange }) => (
              <AudioDropzone name={name} onChange={onChange} />
            )}
          />
          <TextInput
            name="title"
            type="text"
            ref={register}
            label="Title"
            error={errors.title}
            isRequired
            disabled={isSubmitting}
          />
          <TextInput
            name="description"
            ref={register}
            label="Description"
            error={errors.description}
            isTextArea
            disabled={isSubmitting}
          />
          <Controller
            name="genre"
            control={control}
            render={({ name, value, onChange }) => (
              <GenreSelect
                name={name}
                value={value}
                onChange={onChange}
                placeholder="Select Genre"
                isRequired
                isDisabled={isSubmitting}
              />
            )}
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
                disabled={isSubmitting}
              />
            )}
          />
          <InputCheckbox
            name="isPublic"
            label="Public?"
            ref={register}
            isInvalid={!!errors.isPublic}
            disabled={isSubmitting}
            isRequired={true}
            isSwitch={true}
          />
          <Flex marginY={4}>
            <Controller
              name="acceptTerms"
              control={control}
              render={({ name, onBlur, onChange, ref, value }) => (
                <Checkbox
                  name={name}
                  ref={ref}
                  onChange={(e) => onChange(e.target.checked)}
                  onBlur={onBlur}
                  value={value}
                  isInvalid={!!errors.acceptTerms}
                  disabled={isSubmitting}
                >
                  I agree to Audiochan's terms of service.
                </Checkbox>
              )}
            />
            <Spacer />
            <Button
              type="submit"
              isLoading={isSubmitting}
              loadingText="Uploading..."
            >
              Upload
            </Button>
          </Flex>
        </form>
      </Box>
    </Flex>
  );
};

export default AudioUpload;
