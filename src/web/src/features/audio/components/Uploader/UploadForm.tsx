import { zodResolver } from "@hookform/resolvers/zod";
import React from "react";
import { Controller, useForm } from "react-hook-form";
import { z } from "zod";
import AudioDropzone from "./Dropzone";
import TagInput from "~/components/Forms/Inputs/Tags";
import TextInput from "~/components/Forms/Inputs/Text";
import { uploadAudioSchema } from "../../schama";

interface UploadFormProps {
  onFileDropped?: () => void;
  onFileCleared?: () => void;
  onSubmit?: (values: UploadAudioFormValues) => Promise<void>;
}

export type UploadAudioFormValues = z.infer<typeof uploadAudioSchema>;

export default function UploadForm({
  onSubmit,
  onFileCleared,
  onFileDropped,
}: UploadFormProps) {
  const formMethods = useForm<UploadAudioFormValues>({
    defaultValues: {
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
    await onSubmit?.(values);
    reset();
  };

  return (
    <form onSubmit={handleSubmit(handleUploadSubmit)}>
      <AudioDropzone
        onFileDropped={onFileDropped}
        onFileUploaded={(fileName, fileSize, uploadId, duration) => {
          setValue("fileName", fileName);
          setValue("fileSize", fileSize);
          setValue("uploadId", uploadId);
          setValue("duration", duration);
        }}
        onFileCleared={() => {
          onFileCleared?.();
          setValue("fileName", "");
          setValue("fileSize", -1);
          setValue("uploadId", "");
          setValue("duration", -1);
        }}
      />
      <TextInput
        {...register("title")}
        label="Title"
        error={errors.title?.message}
        helperText="Note: When you change the audio's title, the url (slug) will also change to correspond with the title."
      />
      <TextInput
        {...register("description")}
        isTextArea
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
            errors={error?.map((x) => x.message ?? "")}
            {...restField}
          />
        )}
      />
    </form>
  );
}
