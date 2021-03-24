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

  if (file) {
    return <AudioUploading file={file} />;
  }

  return (
    <Box>
      <AudioUploadDropzone
        files={file ? [file] : []}
        onDropAccepted={(files) => {
          setFile(files[0]);
        }}
      />
    </Box>
  );
}
