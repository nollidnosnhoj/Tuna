import { zodResolver } from "@hookform/resolvers/zod/dist/zod";
import React from "react";
import { Controller, useForm } from "react-hook-form";
import { z } from "zod";
import AudioDropzone from "./Dropzone";
import TagInput from "~/components/form-inputs/TagField";
import InputField from "~/components/form-inputs/InputField";
import { uploadAudioSchema } from "../../lib/validators/audio-schemas";
import { Button, Spacer, Stack } from "@chakra-ui/react";
import TextAreaField from "~/components/form-inputs/TextAreaField";

interface UploadFormProps {
  onSubmit?: (values: UploadAudioFormValues) => Promise<void>;
}

export type UploadAudioFormValues = z.infer<typeof uploadAudioSchema>;

export default function UploadForm({ onSubmit }: UploadFormProps) {
  const formMethods = useForm<UploadAudioFormValues>({
    defaultValues: {
      file: null,
      tags: [],
    },
    resolver: zodResolver(uploadAudioSchema),
  });

  const {
    control,
    handleSubmit,
    register,
    reset,
    setValue,
    formState: { errors },
  } = formMethods;

  const handleUploadSubmit = async (values: UploadAudioFormValues) => {
    try {
      await onSubmit?.(values);
      reset({});
    } catch {
      reset({ ...values });
    }
  };

  return (
    <form onSubmit={handleSubmit(handleUploadSubmit)}>
      <AudioDropzone
        onFileDropped={(file) => setValue("file", file)}
        onFileUploaded={(uploadId) => {
          setValue("uploadId", uploadId);
        }}
        onFileCleared={() => {
          setValue("file", null);
          setValue("uploadId", "");
        }}
      />
      <InputField
        {...register("title")}
        label="Title"
        error={errors.title?.message}
        helperText="Note: When you change the audio's title, the url (slug) will also change to correspond with the title."
      />
      <TextAreaField
        {...register("description")}
        label="Description"
        error={errors.description?.message}
      />
      <Controller
        name="tags"
        control={control}
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        render={({ field: { ref, ...restField }, fieldState: { error } }) => (
          <TagInput
            placeholder="Add Tag..."
            error={error?.message}
            {...restField}
          />
        )}
      />
      <Stack direction="row">
        <Spacer />
        <Button type="submit">Submit</Button>
      </Stack>
    </form>
  );
}
