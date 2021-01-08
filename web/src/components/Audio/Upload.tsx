import { AttachmentIcon, DeleteIcon } from "@chakra-ui/icons";
import {
  Flex,
  Box,
  Icon,
  Text,
  IconButton,
  Button,
  Spacer,
  Divider,
  Checkbox,
  useToast,
} from "@chakra-ui/react";
import React, { useState } from "react";
import Router from "next/router";
import { Controller, useForm } from "react-hook-form";
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import { FaCloudUploadAlt } from "react-icons/fa";
import useAudioDropzone from "~/lib/hooks/useAudioDropzone";
import { apiErrorToast, errorToast } from "~/utils/toast";
import InputField from "../InputField";
import TagInput from "../TagInput";
import { uploadAudio } from "~/lib/services/audio";

type FormInputs = {
  title: string;
  description?: string;
  tags?: string[];
  isPublic: boolean;
  acceptTerms: boolean;
};

const AudioUpload = () => {
  const toast = useToast();
  const [file, setFile] = useState<File>(undefined);
  const [uploaded, setUploaded] = useState(false);

  const onSubmit = async (values: FormInputs) => {
    var formData = new FormData();
    formData.append("file", file);

    Object.entries(values).forEach(([key, value]) => {
      if (Array.isArray(value)) {
        value.forEach((val) => formData.append(key, val));
      } else {
        formData.append(key, value.toString());
      }
    });

    try {
      const { id: audioId } = await uploadAudio(formData);
      setUploaded(true);
      toast({
        title: "Audio uploaded!",
        description: "You will be redirected to the audio page.",
        status: "success",
        duration: 500,
        onCloseComplete: () => {
          Router.push(`audios/${audioId}`);
        },
      });
    } catch (err) {
      apiErrorToast(err);
    }
  };

  const { getRootProps, getInputProps } = useAudioDropzone({
    onDropAccepted: (files, event) => {
      setFile(files[0]);
    },
    onDropRejected: (fileRejections) => {
      fileRejections[0].errors
        .map((err) => err.message)
        .forEach((message) => errorToast({ message }));
    },
  });

  const {
    handleSubmit,
    register,
    errors,
    control,
    formState: { isSubmitting },
  } = useForm<FormInputs>({
    defaultValues: {
      title: file?.name ?? "",
      description: "",
      tags: [],
      isPublic: true,
      acceptTerms: false,
    },
    resolver: yupResolver(
      yup.object().shape({
        title: yup.string().required().max(30),
        description: yup.string().max(500),
        tags: yup.array(yup.string()).max(10).ensure(),
        isPublic: yup.boolean(),
        acceptTerms: yup
          .boolean()
          .required()
          .oneOf([true], "You must accept terms of service."),
      })
    ),
  });

  return (
    <div>
      {!file ? (
        <Flex justify="center" align="center" height="70vh">
          <Box {...getRootProps({ className: "dropzone" })} textAlign="center">
            <input {...getInputProps()} />
            <Icon as={FaCloudUploadAlt} boxSize={50} />
            <Text>Drop in an audio file or click to upload.</Text>
          </Box>
        </Flex>
      ) : (
        <Flex justify="center">
          <Box width="100%">
            <Flex align="center">
              <Icon as={AttachmentIcon} marginRight={4} />
              <Text isTruncated>{file.name}</Text>
              <IconButton
                aria-label="Remove file"
                icon={<DeleteIcon />}
                colorScheme="red"
                variant="ghost"
                marginLeft={4}
                disabled={isSubmitting || uploaded}
                onClick={() => setFile(undefined)}
              />
            </Flex>
            <Divider marginY={4} />
            <Box>
              <form onSubmit={handleSubmit(onSubmit)}>
                <InputField
                  name="title"
                  type="text"
                  ref={register}
                  label="Title"
                  error={errors.title}
                  isRequired
                  disabled={isSubmitting || uploaded}
                />
                <InputField
                  name="description"
                  ref={register}
                  label="Description"
                  error={errors.description}
                  isTextArea
                  disabled={isSubmitting || uploaded}
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
                      disabled={isSubmitting || uploaded}
                    />
                  )}
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
                        disabled={isSubmitting || uploaded}
                      >
                        I agree to Audiochan's terms of service.
                      </Checkbox>
                    )}
                  />
                  <Spacer />
                  <Button
                    type="submit"
                    isLoading={isSubmitting}
                    disabled={uploaded}
                    loadingText="Uploading..."
                  >
                    Upload
                  </Button>
                </Flex>
              </form>
            </Box>
          </Box>
        </Flex>
      )}
    </div>
  );
};

export default AudioUpload;
