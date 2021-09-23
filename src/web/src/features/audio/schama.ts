import { z } from "zod";
import { validationMessages } from "~/utils";

export const audioSchema = z.object({
  title: z
    .string()
    .min(5, validationMessages.min("Title", 5))
    .max(30, validationMessages.max("Title", 30)),
  description: z
    .string()
    .max(500, validationMessages.max("Description", 500))
    .optional(),
  tags: z.string().array().max(10, validationMessages.max("Tags", 10)),
});

export const uploadAudioSchema = audioSchema.extend({
  uploadId: z.string().min(1, "Audio file has not been uploaded."),
  fileName: z.string().min(1, "File name is not valid."),
  fileSize: z.number().min(1, "File size is not valid."),
  duration: z.number().min(1, "Duration is not valid."),
});
