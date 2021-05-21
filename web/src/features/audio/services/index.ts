import api from "~/lib/api";

type UploadResponse = {
  uploadId: string;
  uploadUrl: string;
};

export const getS3PresignedUrl = async (
  file: File
): Promise<UploadResponse> => {
  const { data } = await api.post<UploadResponse>("upload", {
    fileName: file.name,
    fileSize: file.size,
  });
  return data;
};
