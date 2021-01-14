import React from "react";
import { Box, Heading, Stack } from "@chakra-ui/react";
import UpdateUsername from "~/components/User/UpdateUsername";
import UpdateEmail from "~/components/User/UpdateEmail";
import UpdatePassword from "~/components/User/UpdatePassword";
import Page from "~/components/Shared/Page";
import useUser from "~/lib/contexts/user_context";

export default function SettingPage() {
  const { user } = useUser();
  return (
    <Page title="Settings" loginRequired>
      <Stack direction="column" spacing={4}>
        <Box>
          <Heading>Account</Heading>
          <UpdateUsername />
          <UpdateEmail />
        </Box>
        <Box>
          <Heading>Password</Heading>
          <UpdatePassword />
        </Box>
      </Stack>
    </Page>
  );
}
