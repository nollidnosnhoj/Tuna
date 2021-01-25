import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalCloseButton,
  ModalBody,
  ModalFooter,
  Button,
  ModalHeader,
  Box,
} from "@chakra-ui/react";
import React, { PropsWithChildren, useEffect, useState } from "react";
import ReactCropper from "react-cropper";
import Cropper from "cropperjs";
import "cropperjs/dist/cropper.css";

interface ImageCropModalProps {
  isOpen: boolean;
  onClose: () => void;
  onCropped: (croppedFile: File) => void;
  file?: File;
}

export default function ImageCropModal({
  isOpen,
  onClose,
  file,
  onCropped,
}: PropsWithChildren<ImageCropModalProps>) {
  const [image, setImage] = useState<string>(null);
  const [cropper, setCropper] = useState<Cropper>(null);

  useEffect(() => {
    if (file) {
      const reader = new FileReader();
      reader.addEventListener("load", () => {
        setImage(reader.result as string);
      });
      reader.readAsDataURL(file);
    } else {
      setImage(null);
      setCropper(null);
    }
  }, [file, cropper]);

  const handleCropped = () => {
    if (!cropper) return;

    const croppedCanvasOptions: Cropper.GetCroppedCanvasOptions = {
      maxWidth: 500,
      maxHeight: 500,
      minWidth: 100,
      minHeight: 100,
      imageSmoothingQuality: "medium",
    };

    const canvasData: HTMLCanvasElement = cropper.getCroppedCanvas(
      croppedCanvasOptions
    );

    canvasData.toBlob((blob) => {
      const croppedFile = new File([blob], file.name);
      onCropped(croppedFile);
      setImage(null);
      setCropper(null);
    });

    onClose();
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} size="xl">
      <ModalOverlay />
      <ModalContent>
        <ModalHeader></ModalHeader>
        <ModalCloseButton />
        <ModalBody>
          <Box marginTop={4}>
            <ReactCropper
              initialAspectRatio={1}
              src={image}
              aspectRatio={1}
              checkOrientation={false}
              onInitialized={(instance) => setCropper(instance)}
            />
          </Box>
        </ModalBody>
        <ModalFooter>
          <Button colorScheme="primary" onClick={handleCropped}>
            Crop
          </Button>
        </ModalFooter>
      </ModalContent>
    </Modal>
  );
}
