import { Box, Flex, Image } from "@chakra-ui/react";
import { PropsWithChildren, useState } from "react";
import ImageDropzone from "~/components/Shared/ImageDropzone";

interface AudioImageProps {
  name: string;
  disabled?: boolean;
  imageData?: string;
  canReplace?: boolean;
  onChange?: (file: File) => Promise<void>;
}

export default function AudioImage({
  imageData,
  disabled = false,
  canReplace = false,
  ...props
}: PropsWithChildren<AudioImageProps>) {
  const [image, setImage] = useState<string>(imageData);

  /** When image changes (after submitting crop) */
  const changeImage = async (file: File) => {
    if (props.onChange) {
      props
        .onChange(file)
        .then(() => setImage(window.URL.createObjectURL(file)));
    } else {
      setImage(window.URL.createObjectURL(file));
    }
  };

  return (
    <Box>
      <Flex justify="center" marginBottom={4}>
        <Image src={image} boxSize="250px" />
      </Flex>
      {canReplace && (
        <ImageDropzone
          name="image"
          image={image}
          disabled={disabled}
          onChange={changeImage}
        />
      )}
    </Box>
  );
}
