import { Box, Heading, Stack } from "@chakra-ui/react";
import React from "react";
import Page from "~/components/Page";
import UpdateEmailForm from "~/components/forms/UpdateEmailForm";
import UpdatePasswordForm from "~/components/forms/UpdatePasswordForm";
import UpdateUsernameForm from "~/components/forms/UpdateUsernameForm";
import {
  useUpdateEmail,
  useUpdatePassword,
  useUpdateUsername,
} from "~/lib/hooks/api";

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
