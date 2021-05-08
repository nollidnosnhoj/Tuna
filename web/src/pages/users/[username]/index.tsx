import { GetServerSideProps } from "next";
import { QueryClient } from "react-query";
import { dehydrate } from "react-query/hydration";
import { fetchUserProfile } from "~/features/user/services";
import ProfilePage from "~/features/user/components/Pages/ProfilePage";
import { getAccessToken } from "~/utils";

export const getServerSideProps: GetServerSideProps = async (context) => {
  const queryClient = new QueryClient();
  const username = context.params?.username as string;
  const accessToken = getAccessToken(context);
  try {
    await queryClient.fetchQuery(["users", username], () =>
      fetchUserProfile(username, { accessToken })
    );
    return {
      props: {
        dehydratedState: dehydrate(queryClient),
      },
    };
  } catch (err) {
    return {
      notFound: true,
    };
  }
};

export default function UserProfileNextPage() {
  return <ProfilePage />;
}
