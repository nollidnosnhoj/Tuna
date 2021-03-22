import {
  Box,
  Button,
  Flex,
  FormControl,
  FormHelperText,
  FormLabel,
  ListItem,
  Select,
  Spacer,
  Text,
  UnorderedList,
} from "@chakra-ui/react";
import React, { useState } from "react";
import { useFormik } from "formik";
import AudioUploadDropzone from "./Dropzone";
import AudioUploading from "./Uploading";
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
      publicity: "unlisted",
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
            <FormControl id="publicity">
              <FormLabel>Publicity</FormLabel>
              <Select
                name="publicity"
                value={values.publicity}
                onChange={handleChange}
              >
                <option value="unlisted">Unlisted</option>
                <option value="public">Public</option>
                <option value="private">Private</option>
              </Select>
              <FormHelperText>
                <UnorderedList>
                  <ListItem>
                    <strong>Public</strong> - Audio will be shown in lists and
                    searches.
                  </ListItem>
                  <ListItem>
                    <strong>Unlisted</strong> - Audio will not be in lists or
                    searches.
                  </ListItem>
                  <ListItem>
                    <strong>Private</strong> - Audio can only be seen if a
                    private key is provided.
                  </ListItem>
                </UnorderedList>
              </FormHelperText>
            </FormControl>
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
