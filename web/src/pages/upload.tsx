import React, { ReactElement } from "react";
import Router from "next/router";
import { useToast } from "@chakra-ui/react";
import { GetServerSideProps } from "next";
import AudioUpload from "~/components/AudioUpload";
import PageLayout from "~/components/Layout";
import { AudioRequest } from "~/lib/types";
import { uploadAudio } from "~/lib/services/audio";
import { toast } from "@chakra-ui/react";

import request from "~/lib/request";

export const getServerSideProps: GetServerSideProps = async (ctx) => {
  const { res } = ctx;
  try {
    await request("me", { method: "head", ctx });
  } catch (err) {
    res.writeHead(302, {
      Location: "/login",
    });
    res.end();
  }

  return { props: {} };
};

export default function UploadPage() {
  return (
    <PageLayout title="Upload Audio">
      <AudioUpload />
    </PageLayout>
  );
}
