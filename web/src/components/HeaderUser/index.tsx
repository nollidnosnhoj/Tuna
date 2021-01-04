import {
  Button,
  IconButton,
  Menu,
  MenuButton,
  MenuDivider,
  MenuGroup,
  MenuItem,
  MenuList,
  Popover,
  PopoverBody,
  PopoverContent,
  PopoverTrigger,
  Slider,
  SliderFilledTrack,
  SliderThumb,
  SliderTrack,
  Stack,
  Text,
  useColorMode,
} from "@chakra-ui/react";
import { MoonIcon, SunIcon } from "@chakra-ui/icons";
import {
  FaCloudUploadAlt,
  FaUserAlt,
  FaVolumeDown,
  FaVolumeMute,
  FaVolumeUp,
} from "react-icons/fa";
import React, { useMemo, useRef } from "react";
import NextLink from "next/link";
import { useRouter } from "next/router";
import useUser from "~/lib/contexts/user_context";
import { useAudioPlayer } from "~/lib/contexts/audio_player_context";
import LoginModal from "../Login/LoginModal";
import RegisterModal from "../Register/RegisterModal";

const HeaderUser: React.FC = () => {
  const router = useRouter();
  const { colorMode, toggleColorMode } = useColorMode();
  const { user, isLoading, isAuth, logout } = useUser();
  const { volume, handleVolume } = useAudioPlayer();
  const volumeRef = useRef();

  const volumeIcon = useMemo(() => {
    if (volume <= 0) {
      return <FaVolumeMute />;
    }

    if (volume >= 0.5) {
      return <FaVolumeUp />;
    }

    return <FaVolumeDown />;
  }, [volume]);

  if (isLoading) {
    return <Text>Loading...</Text>;
  }

  return (
    <>
      <Stack direction="row" spacing={4}>
        {colorMode === "light" ? (
          <IconButton
            aria-label="Dark mode"
            icon={<MoonIcon />}
            variant="ghost"
            onClick={toggleColorMode}
          />
        ) : (
          <IconButton
            aria-label="Light mode"
            icon={<SunIcon />}
            variant="ghost"
            onClick={toggleColorMode}
          />
        )}
        <div>
          <Popover initialFocusRef={volumeRef} placement="bottom">
            <PopoverTrigger>
              <IconButton
                aria-label="Set volume"
                icon={volumeIcon}
                variant="ghost"
              />
            </PopoverTrigger>
            <PopoverContent>
              <PopoverBody>
                <Slider
                  ref={volumeRef}
                  value={volume}
                  min={0}
                  max={1}
                  step={0.1}
                  onChange={(v) => handleVolume(v)}
                >
                  <SliderTrack>
                    <SliderFilledTrack />
                  </SliderTrack>
                  <SliderThumb />
                </Slider>
              </PopoverBody>
            </PopoverContent>
          </Popover>
        </div>
        {!isAuth ? (
          <>
            <LoginModal />
            <RegisterModal />
          </>
        ) : (
          <>
            <NextLink href="/upload">
              <Button leftIcon={<FaCloudUploadAlt />}>Upload</Button>
            </NextLink>
            <div>
              <Menu>
                <MenuButton
                  as={IconButton}
                  icon={<FaUserAlt />}
                  variant="ghost"
                  colorScheme="primary"
                >
                  Profile
                </MenuButton>
                <MenuList>
                  <MenuGroup title={user?.username}>
                    <MenuItem>Profile</MenuItem>
                    <MenuItem>Settings</MenuItem>
                  </MenuGroup>
                  <MenuDivider />
                  <MenuItem onClick={() => logout()}>Logout</MenuItem>
                </MenuList>
              </Menu>
            </div>
          </>
        )}
      </Stack>
    </>
  );
};

export default HeaderUser;
