import { Stack, Box, Heading } from "@chakra-ui/react";
import React from "react";
import Page from "~/components/Page";
import UpdateEmail from "~/features/user/components/UpdateEmail";
import UpdatePassword from "~/features/user/components/UpdatePassword";
import UpdateUsername from "~/features/user/components/UpdateUsername";

const SettingPage: React.FC = () => {
  return (
    <Page title="Settings" requiresAuth>
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
};

export default SettingPage;
