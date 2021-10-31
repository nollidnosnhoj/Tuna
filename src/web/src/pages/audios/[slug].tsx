import {
  Accordion,
  AccordionButton,
  AccordionIcon,
  AccordionItem,
  AccordionPanel,
  Box,
  chakra,
  Flex,
  Heading,
  MenuGroup,
  MenuItem,
  Stack,
  Table,
  Tag,
  TagLabel,
  TagLeftIcon,
  Tbody,
  Td,
  Tr,
  useColorModeValue,
  useDisclosure,
  Wrap,
  WrapItem,
} from "@chakra-ui/react";
import { GetServerSideProps } from "next";
import React, { useEffect } from "react";
import Page from "~/components/Page";
import request from "~/lib/http";
import Link from "~/components/ui/Link";
import { formatDuration, formatFileSize, relativeDate } from "~/utils";
import AudioPlayButton from "~/components/buttons/Play";
import AudioFavoriteButton from "~/components/buttons/Favorite";
import AudioMiscMenu from "~/components/buttons/Menu";
import { EditIcon } from "@chakra-ui/icons";
import AudioEditDrawer from "~/components/AudioEditDrawer";
import { useUser } from "~/components/providers/UserProvider";
import { useRouter } from "next/router";
import PictureController from "~/components/Picture";
import NextLink from "next/link";
import { FaHashtag } from "react-icons/fa";
import { Audio } from "~/lib/types";
import {
  useAddAudioPicture,
  useGetAudio,
  useRemoveAudioPicture,
} from "~/lib/hooks/api";

interface AudioPageProps {
  audio: Audio;
  slug: string;
}

function AudioDetailPicture(props: { audio: Audio }) {
  const { audio } = props;

  const { user: currentUser } = useUser();

  const { mutateAsync: addPictureAsync, isLoading: isAddingPicture } =
    useAddAudioPicture(audio.id);

  const { mutateAsync: removePictureAsync, isLoading: isRemovingPicture } =
    useRemoveAudioPicture(audio.id);
  return (
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
  );
}

export const getServerSideProps: GetServerSideProps<AudioPageProps> = async (
  context
) => {
  const slug = context.params?.slug as string;
  const { req, res } = context;
  try {
    const { data } = await request<Audio>({
      method: "get",
      url: `audios/${slug}`,
      req,
      res,
    });
    return {
      props: {
        audio: data,
        slug,
      },
    };
  } catch (err) {
    return {
      notFound: true,
    };
  }
};

export default function AudioPage({ audio: initAudio, slug }: AudioPageProps) {
  const router = useRouter();
  const secondaryColor = useColorModeValue("black.300", "gray.300");
  const { user: currentUser } = useUser();

  const { data: audio } = useGetAudio(slug, {
    staleTime: 1000,
    initialData: initAudio,
  });

  const {
    isOpen: isEditOpen,
    onOpen: onEditOpen,
    onClose: onEditClose,
  } = useDisclosure();

  useEffect(() => {
    if (audio) {
      router.prefetch(`/users/${audio.user.userName}`);
    }
  }, []);

  if (!audio) return null;

  return (
    <Page title={audio.title}>
      <Flex
        marginBottom={4}
        justifyContent="center"
        direction={{ base: "column", md: "row" }}
      >
        <Flex
          flex="1"
          marginRight={4}
          justify={{ base: "center", md: "normal" }}
        >
          <AudioDetailPicture audio={audio} />
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
            <AudioMiscMenu audio={audio} size="lg">
              {audio.user.id === currentUser?.id && (
                <MenuGroup>
                  <MenuItem icon={<EditIcon />} onClick={onEditOpen}>
                    Edit
                  </MenuItem>
                </MenuGroup>
              )}
            </AudioMiscMenu>
          </Stack>
        </Box>
      </Flex>
      <Accordion defaultIndex={[0]} allowMultiple>
        {audio.tags && audio.tags.length > 0 && (
          <AccordionItem>
            <h2>
              <AccordionButton>
                <Box flex="1" textAlign="left">
                  Tags
                </Box>
              </AccordionButton>
            </h2>
            <AccordionPanel pb={4}>
              <Flex alignItems="flex-end">
                <Wrap marginTop={2}>
                  {audio.tags.map((tag, idx) => (
                    <WrapItem key={idx}>
                      <NextLink href={`/tags/${tag}/`}>
                        <Tag size="md" cursor="pointer">
                          <TagLeftIcon as={FaHashtag} />
                          <TagLabel>{tag}</TagLabel>
                        </Tag>
                      </NextLink>
                    </WrapItem>
                  ))}
                </Wrap>
              </Flex>
            </AccordionPanel>
          </AccordionItem>
        )}
        <AccordionItem>
          <h2>
            <AccordionButton>
              <Box flex="1" textAlign="left">
                Description
              </Box>
              <AccordionIcon />
            </AccordionButton>
          </h2>
          <AccordionPanel pb={4}>
            {audio.description || "No information given."}
          </AccordionPanel>
        </AccordionItem>
        <AccordionItem>
          <h2>
            <AccordionButton>
              <Box flex="1" textAlign="left">
                File Info
              </Box>
              <AccordionIcon />
            </AccordionButton>
          </h2>
          <AccordionPanel pb={4}>
            <Table>
              <Tbody>
                <Tr>
                  <Td>Duration</Td>
                  <Td>{formatDuration(audio.duration)}</Td>
                </Tr>
                <Tr>
                  <Td>File Size</Td>
                  <Td>{formatFileSize(audio.size)}</Td>
                </Tr>
              </Tbody>
            </Table>
          </AccordionPanel>
        </AccordionItem>
      </Accordion>
      <AudioEditDrawer
        audio={audio}
        isOpen={isEditOpen}
        onClose={onEditClose}
      />
    </Page>
  );
}
