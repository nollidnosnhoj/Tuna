import { AspectRatio, Box, Flex, Image } from "@chakra-ui/react";
import { PropsWithChildren, useState } from "react";
import PictureDropzone from "~/components/Shared/Picture/PictureDropzone";
import { AudioDetailModel, AudioListModel } from "~/lib/types/audio";

interface AudioPictureProps {
  name: string;
  audio?: AudioDetailModel;
  disabled?: boolean;
  canReplace?: boolean;
  onChange?: (file: File) => Promise<void>;
}

export default function AudioPicture({
  audio,
  name = "image",
  disabled = false,
  canReplace = false,
  ...props
}: PropsWithChildren<AudioPictureProps>) {
  const [image, setImage] = useState<string>(audio?.pictureUrl);

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
      <Flex
        direction="column"
        justify="center"
        align="center"
        textAlign="center"
      >
        <AspectRatio boxSize="250px" ratio={1}>
          <Image
            src={image}
            objectFit="cover"
            marginBottom={4}
            bgGradient="linear(to-r, green.200, pink.500)"
          />
        </AspectRatio>
        {canReplace && (
          <PictureDropzone
            name="image"
            image={image}
            disabled={disabled}
            onChange={changeImage}
          />
        )}
      </Flex>
    </Box>
  );
}
