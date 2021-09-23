import { Stack, Box, Heading } from "@chakra-ui/react";
import React from "react";
import Page from "~/components/Page";
import {
  useUpdateEmail,
  useUpdatePassword,
  useUpdateUsername,
} from "~/features/user/api/hooks";
import UpdateEmailForm from "~/features/user/components/UpdateEmail";
import UpdatePasswordForm from "~/features/user/components/UpdatePassword";
import UpdateUsernameForm from "~/features/user/components/UpdateUsername";

const SettingPage: React.FC = () => {
  const { mutateAsync: updateUsernameAsync } = useUpdateUsername();
  const { mutateAsync: updatePasswordAsync } = useUpdatePassword();
  const { mutateAsync: updateEmailAsync } = useUpdateEmail();

  return (
    <Page title="Settings" requiresAuth>
      <Stack direction="column" spacing={4}>
        <Box>
          <Heading>Account</Heading>
          <UpdateUsernameForm
            onSubmit={(values) =>
              updateUsernameAsync({ newUsername: values.username })
            }
          />
          <UpdateEmailForm
            onSubmit={(email) => updateEmailAsync({ newEmail: email })}
          />
        </Box>
        <Box>
          <Heading>Password</Heading>
          <UpdatePasswordForm
            onSubmit={(curr, newPass) =>
              updatePasswordAsync({
                currentPassword: curr,
                newPassword: newPass,
              })
            }
          />
        </Box>
      </Stack>
    </Page>
  );
};

export default SettingPage;
