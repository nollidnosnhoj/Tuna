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
  Text,
} from "@chakra-ui/react";
import React, {
  PropsWithChildren,
  useCallback,
  useEffect,
  useState,
} from "react";
import ReactCropper from "react-cropper";
import Cropper from "cropperjs";
import "cropperjs/dist/cropper.css";

interface PictureCropModalProps {
  isOpen: boolean;
  onClose: () => void;
  onCropped: (croppedData: string) => void;
  file?: File;
}

export default function PictureCropModal({
  isOpen,
  onClose,
  file,
  onCropped,
}: PropsWithChildren<PictureCropModalProps>) {
  const [image, setImage] = useState<string>("");
  const [cropper, setCropper] = useState<Cropper | undefined>(undefined);

  /** Load image from File */
  useEffect(() => {
    if (file) {
      const reader = new FileReader();
      reader.addEventListener("load", () => {
        setImage(reader.result as string);
      });
      reader.readAsDataURL(file);
    } else {
      setImage("");
      setCropper(undefined);
    }
  }, [file, setCropper]);

  /** Crop the image and return canvas into Blob */
  const handleCrop = useCallback(() => {
    return new Promise<string>((resolve, reject) => {
      if (!cropper) {
        reject("Cropper was not initialized.");
        return;
      }

      const canvasData: HTMLCanvasElement = cropper.getCroppedCanvas({
        fillColor: "white",
      });

      resolve(canvasData.toDataURL(file?.type));

      onClose();
    });
  }, [cropper, setImage]);

  return (
    <Modal isOpen={isOpen} onClose={onClose} size="xl">
      <ModalOverlay />
      <ModalContent>
        <ModalHeader></ModalHeader>
        <ModalCloseButton />
        <ModalBody>
          <Box marginTop={4}>
            <ReactCropper
              src={image}
              aspectRatio={1}
              checkOrientation={false}
              initialAspectRatio={1}
              autoCrop
              onInitialized={(instance) => setCropper(instance)}
              viewMode={1}
              cropBoxMovable={false}
              cropBoxResizable={false}
              dragMode="move"
              wheelZoomRatio={0.4}
              autoCropArea={1}
              responsive
            />
          </Box>
          <Box>
            <Text>
              Drag image with mouse to move. Use mouse-wheel to zoom in and out.
            </Text>
          </Box>
        </ModalBody>
        <ModalFooter>
          <Button
            colorScheme="primary"
            onClick={() => {
              handleCrop()
                .then((imageData) => onCropped(imageData))
                .catch((err) => console.error(err));
            }}
          >
            Crop
          </Button>
        </ModalFooter>
      </ModalContent>
    </Modal>
  );
}
