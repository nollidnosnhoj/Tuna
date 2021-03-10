import { Box, chakra, Tooltip, useDisclosure } from "@chakra-ui/react";
import React, { ReactElement, useCallback } from "react";
import { useDropzone } from "react-dropzone";
import { PictureProps } from "~/components/Picture";
import PictureCropModal from "./PictureCropModal";
import SETTINGS from "~/constants/settings";
import { errorToast, successfulToast } from "~/utils/toast";
import { isAxiosError } from "~/utils/axios";
import { ErrorResponse } from "~/lib/types";

interface PictureDropzoneProps {
  children: ReactElement<PictureProps>;
  onChange: (imageData: string) => Promise<void>;
  disabled?: boolean;
}

const PictureDropzone: React.FC<PictureDropzoneProps> = ({
  onChange,
  disabled = false,
  children,
}) => {
  const {
    isOpen: isImageCropModalOpen,
    onOpen: openImageCropModal,
    onClose: closeImageCropModal,
  } = useDisclosure();
  const { open, getInputProps, acceptedFiles } = useDropzone({
    accept: SETTINGS.UPLOAD.IMAGE.accept,
    maxSize: SETTINGS.UPLOAD.IMAGE.maxSize,
    multiple: false,
    onDropAccepted: () => {
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

  const openUploadBox = useCallback(() => {
    !disabled && open();
  }, [disabled, open]);

  return (
    <Box>
      <input {...getInputProps()} />
      <Tooltip label={children.props.source ? "Replace Image" : "Upload Image"}>
        <chakra.span onClick={openUploadBox} cursor="pointer">
          {children}
        </chakra.span>
      </Tooltip>
      <PictureCropModal
        file={acceptedFiles[0]}
        isOpen={isImageCropModalOpen}
        onClose={closeImageCropModal}
        onCropped={(croppedData) => {
          onChange(croppedData)
            .then(() => {
              successfulToast({
                title: "Image have successfully changed.",
              });
            })
            .catch((err) => {
              if (
                isAxiosError<ErrorResponse>(err) &&
                err.response?.status === 429
              ) {
                errorToast({
                  title: "Too many requests.",
                  message: "Try again later.",
                });
              }
            });
        }}
      />
    </Box>
  );
};

export default PictureDropzone;
