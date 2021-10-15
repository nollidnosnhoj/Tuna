import {
  Box,
  Flex,
  Heading,
  Stack,
  useColorModeValue,
  chakra,
  useDisclosure,
} from "@chakra-ui/react";
import Router from "next/router";
import React, { useEffect } from "react";
import Link from "~/components/UI/Link";
import { AudioView } from "~/features/audio/api/types";
import { useUser } from "~/features/user/hooks";
import { relativeDate } from "~/utils/time";
import AudioPlayButton from "../Buttons/Play";
import AudioFavoriteButton from "../Buttons/Favorite";
import PictureController from "~/components/Picture";
import { useAddAudioPicture, useRemoveAudioPicture } from "../../api/hooks";
import AudioMiscMenu from "../../../../components/UI/ContextMenu";
import { useAudioPlayer } from "~/lib/stores";
import { MdQueueMusic } from "react-icons/md";
import { EditIcon } from "@chakra-ui/icons";
import AudioEditDrawer from "../Edit";

interface AudioDetailProps {
  audio: AudioView;
}

const AudioDetails: React.FC<AudioDetailProps> = ({ audio }) => {
  const secondaryColor = useColorModeValue("black.300", "gray.300");
  const { user: currentUser } = useUser();
  const addToQueue = useAudioPlayer((state) => state.addToQueue);

  const {
    isOpen: isEditOpen,
    onOpen: onEditOpen,
    onClose: onEditClose,
  } = useDisclosure();

  const { mutateAsync: addPictureAsync, isLoading: isAddingPicture } =
    useAddAudioPicture(audio.id);

  const { mutateAsync: removePictureAsync, isLoading: isRemovingPicture } =
    useRemoveAudioPicture(audio.id);

  useEffect(() => {
    Router.prefetch(`/users/${audio.user.userName}`);
  }, []);

  return (
    <Flex
      marginBottom={4}
      justifyContent="center"
      direction={{ base: "column", md: "row" }}
    >
      <Flex flex="1" marginRight={4} justify={{ base: "center", md: "normal" }}>
        <PictureController
          title={audio.title}
          src={audio.picture || ""}
          onChange={async (croppedData) => {
            await addPictureAsync(croppedData);
          }}
          onRemove={removePictureAsync}
          isMutating={isAddingPicture || isRemovingPicture}
          canEdit={currentUser?.id === audio.user.id}
        />
      </Flex>
      <Box flex="6">
        <chakra.div marginTop={{ base: 4, md: 0 }} marginBottom={4}>
          <Heading as="h1" fontSize={{ base: "3xl", md: "5xl" }}>
            {audio.title}
          </Heading>
          <chakra.div display="flex">
            <Link href={`/users/${audio.user.userName}`} fontWeight="500">
              {audio.user.userName}
            </Link>
            <chakra.span
              color={secondaryColor}
              _before={{ content: `"•"`, marginX: 2 }}
            >
              {relativeDate(audio.created)}
            </chakra.span>
          </chakra.div>
        </chakra.div>
        <Stack direction="row" alignItems="center">
          <AudioPlayButton audio={audio} size="lg" />
          <AudioFavoriteButton audioId={audio.id} size="lg" />
          <AudioMiscMenu
            size="lg"
            items={[
              {
                items: [
                  {
                    name: "Edit",
                    isVisible: audio.user.id === currentUser?.id,
                    onClick: onEditOpen,
                    icon: <EditIcon />,
                  },
                  {
                    name: "Add to Queue",
                    isVisible: true,
                    icon: <MdQueueMusic />,
                    onClick: async () => await addToQueue("custom", [audio]),
                  },
                ],
              },
            ]}
          />
        </Stack>
        <AudioEditDrawer
          audio={audio}
          isOpen={isEditOpen}
          onClose={onEditClose}
        />
      </Box>
    </Flex>
  );
};

export default AudioDetails;
