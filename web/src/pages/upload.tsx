import React from "react";
import { GetServerSideProps } from "next";
import AudioUpload from "~/components/AudioUpload";
import PageLayout from "~/components/Layout";

import request from "~/lib/request";

export const getServerSideProps: GetServerSideProps = async (ctx) => {
  const { res } = ctx;
  try {
    const { data } = await request("me", { ctx });
    return { props: { user: data } };
  } catch (err) {
    return {
      props: {},
      redirect: {
        destination: "/login",
        permanent: false,
      },
    };
  }
};

export default function UploadPage() {
  return (
    <PageLayout title="Upload Audio">
      <AudioUpload />
    </PageLayout>
  );
}
