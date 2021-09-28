import axios from "axios";
import React, { createContext, ReactNode, useContext, useState } from "react";
import { useUser } from "~/features/user/hooks";
import request from "~/lib/http";

type UploaderContextValues = {
  isUploading: boolean;
  isUploaded: boolean;
  uploadProgress: number;
  uploadFile: (file: File) => Promise<string>;
  reset: () => void;
};

const UploaderContext = createContext<UploaderContextValues>(
  {} as UploaderContextValues
);

export const useUploaderContext = () => {
  const context = useContext(UploaderContext);
  if (!context) throw Error("Uploader context was not found.");
  return context;
};

export default function UploaderProvider({
  children,
}: {
  children: ReactNode | undefined;
}) {
  const { user } = useUser();
  const [uploading, setUploading] = useState(false);
  const [uploaded, setUploaded] = useState(false);
  const [progress, setProgress] = useState(0);
  const handleReset = () => {
    setUploading(false);
    setUploaded(false);
    setProgress(0);
  };
  const handleUploadFile = async (file: File): Promise<string> => {
    try {
      setUploading(true);
      const { data: response } = await request<{
        uploadId: string;
        uploadUrl: string;
      }>({
        method: "post",
        url: "upload",
        data: {
          fileName: file.name,
          fileSize: file.size,
        },
      });
      await axios.put(response.uploadUrl, file, {
        headers: {
          "Content-Type": file.type,
          "x-amz-meta-userId": `${user?.id}`,
        },
        onUploadProgress: (evt) => {
          const currentProgress = (evt.loaded / evt.total) * 100;
          setProgress(currentProgress);
        },
      });
      setUploaded(true);
      return response.uploadId;
    } finally {
      setUploading(false);
    }
  };

  return (
    <UploaderContext.Provider
      value={{
        isUploaded: uploaded,
        isUploading: uploading,
        uploadFile: handleUploadFile,
        uploadProgress: progress,
        reset: handleReset,
      }}
    >
      {children}
    </UploaderContext.Provider>
  );
}
