import { Box, Button, useDisclosure } from "@chakra-ui/react";
import React from "react";
import { useDropzone } from "react-dropzone";
import ImageCropModal from "./ImageCropModal";
import { errorToast } from "~/utils/toast";

interface ImageDropzoneProps {
  name: string;
  onChange: (file: File) => void;
  image?: string;
  disabled?: boolean;
}

const ImageDropzone: React.FC<ImageDropzoneProps> = ({
  name,
  image,
  onChange,
  disabled = false,
}) => {
  const {
    isOpen: isImageCropModalOpen,
    onOpen: openImageCropModal,
    onClose: closeImageCropModal,
  } = useDisclosure();
  const { open, getInputProps, acceptedFiles } = useDropzone({
    accept: "image/*",
    maxSize: 2097152,
    multiple: false,
    onDropAccepted: () => {
      /** open crop modal */
      openImageCropModal();
    },
    onDropRejected: ([fileRejection]) => {
      /** Display error toasts */
      fileRejection.errors.forEach((err) => {
        errorToast({
          title: "Invalid Image",
          message: err.message,
        });
      });
    },
  });

  return (
    <Box>
      <input {...getInputProps({ name })} />
      <Button width="100%" onClick={open} disabled={disabled}>
        {image ? "Replace Image" : "Upload Image"}
      </Button>
      <ImageCropModal
        file={acceptedFiles[0]}
        isOpen={isImageCropModalOpen}
        onClose={closeImageCropModal}
        onCropped={(croppedFile) => {
          onChange(croppedFile);
        }}
      />
    </Box>
  );
};

export default ImageDropzone;
