import { Box, Button, Flex, Spacer, Text, VStack } from "@chakra-ui/react";
import React, { useState } from "react";
import { useFormik } from "formik";
import AudioUploadDropzone from "./Dropzone";
import AudioUploading from "./Uploading";
import InputCheckbox from "~/components/Form/Checkbox";
import GenreSelect from "~/components/Form/GenreSelect";
import TagInput from "~/components/Form/TagInput";
import TextInput from "~/components/Form/TextInput";
import { AudioRequest } from "~/features/audio/types";
import { uploadAudioSchema } from "~/features/audio/schemas";
import Picture from "~/components/Picture";
import PictureDropzone from "~/components/Picture/PictureDropzone";

interface AudioUploadProps {
  maxFileSize?: number;
  validContentTypes?: string[];
}

export default function AudioUpload(props: AudioUploadProps) {
  const [file, setFile] = useState<File | undefined>(undefined);
  const [picture, setPicture] = useState("");
  const [value, setValue] = useState<AudioRequest | undefined>(undefined);
  const [submit, setSubmit] = useState(false);

  const formik = useFormik<AudioRequest>({
    initialValues: {
      title: "",
      description: "",
      tags: [],
      isPublic: true,
      genre: "",
    },
    onSubmit: (values) => {
      setValue(values);
      setSubmit(true);
    },
    validationSchema: uploadAudioSchema,
  });

  const { values, errors, handleChange, handleSubmit, setFieldValue } = formik;

  if (file && submit) {
    return <AudioUploading file={file} form={value} picture={picture} />;
  }

  return (
    <Box>
      <AudioUploadDropzone
        files={file ? [file] : []}
        onDropAccepted={(files) => {
          setFile(files[0]);
        }}
      />
      <form onSubmit={handleSubmit}>
        <Flex>
          <Flex flex="1" justifyContent="center">
            <Box padding={4} textAlign="center">
              <PictureDropzone
                onChange={async (data) => {
                  setPicture(data);
                }}
              >
                <Picture source={picture} imageSize={250} />
              </PictureDropzone>
            </Box>
          </Flex>
          <Box flex="3">
            <TextInput
              name="title"
              value={values.title}
              onChange={handleChange}
              error={errors.title}
              type="text"
              label="Title"
              placeholder={file?.name}
            />
            <TextInput
              name="description"
              value={values.description ?? ""}
              onChange={handleChange}
              error={errors.description}
              label="Description"
              textArea
            />
            <GenreSelect
              name="genre"
              label="Genre"
              value={values.genre ?? ""}
              onChange={handleChange}
              error={errors.genre}
              placeholder="No Genre Selected"
              paddingY={2}
            />
            <TagInput
              name="tags"
              value={values.tags}
              onAdd={(tag) => {
                setFieldValue("tags", [...values.tags, tag]);
              }}
              onRemove={(index) => {
                setFieldValue(
                  "tags",
                  values.tags.filter((_, i) => i !== index)
                );
              }}
              error={errors.tags}
            />
            <InputCheckbox
              name="isPublic"
              value={values.isPublic}
              onChange={() => setFieldValue("isPublic", !values.isPublic)}
              error={errors.isPublic}
              label="Public?"
              required
              toggleSwitch
            />
          </Box>
        </Flex>
        <Flex marginY={4} alignItems="center">
          <Text>By uploading, you agree to our terms and service.</Text>
          <Spacer />
          <Button type="submit" disabled={!file}>
            Upload
          </Button>
        </Flex>
      </form>
    </Box>
  );
}
