import { Flex, Box, Button, Spacer } from "@chakra-ui/react";
import React from "react";
import Router from "next/router";
import { Controller, FormProvider, useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import { DevTool } from "@hookform/devtools";
import AudioDropzone from "./Dropzone";
import InputCheckbox from "../Form/Checkbox";
import GenreSelect from "../Form/GenreSelect";
import TextInput from "../Form/TextInput";
import TagInput from "../Form/TagInput";
import { uploadAudio } from "~/lib/services/audio";
import { UploadAudioRequest } from "~/lib/types/audio";
import { uploadAudioSchema } from "~/lib/validationSchemas";
import { apiErrorToast, successfulToast } from "~/utils/toast";

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

  const methods = useForm<UploadAudioRequest>({
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

  const {
    handleSubmit,
    errors,
    control,
    formState: { isSubmitting },
  } = methods;

  return (
    <Flex justify="center">
      <Box width="100%">
        <FormProvider {...methods}>
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
              label="Title"
              required
              disabled={isSubmitting}
            />
            <TextInput
              name="description"
              label="Description"
              textArea
              disabled={isSubmitting}
            />
            <GenreSelect
              name="genre"
              placeholder="Select Genre"
              required
              disabled={isSubmitting}
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
              disabled={isSubmitting}
              required
              toggleSwitch
            />
            <Flex marginY={4}>
              <InputCheckbox
                name="acceptTerms"
                disabled={isSubmitting}
                required
              >
                I agree to Audiochan's terms of service.
              </InputCheckbox>
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
        </FormProvider>
        <DevTool control={control} />
      </Box>
    </Flex>
  );
};

export default AudioUpload;
