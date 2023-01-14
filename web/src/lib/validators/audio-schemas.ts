import { z } from "zod";
import SETTINGS from "~/lib/config";

export const audioSchema = z.object({
  title: z.string().min(5).max(30),
  description: z.string().max(500).optional(),
  tags: z.string().array().max(10),
});

export const uploadAudioSchema = audioSchema.extend({
  file: z.any().superRefine((arg, ctx) => {
    if (typeof window === undefined || !(arg instanceof File)) {
      ctx.addIssue({
        code: "custom",
        message: "Unable to read file.",
      });
      return;
    }

    if (!SETTINGS.UPLOAD.AUDIO.accept[arg.type]) {
      const fileExts = Object.values(SETTINGS.UPLOAD.AUDIO.accept).flatMap(
        (x) => x
      );
      if (!fileExts.some((x) => arg.name.endsWith(x))) {
        ctx.addIssue({
          code: "custom",
          message: "File is not a valid audio type.",
        });
      }
    }

    if (SETTINGS.UPLOAD.AUDIO.maxSize <= arg.size) {
      ctx.addIssue({
        code: "custom",
        message: "File exceeded the file size limit",
      });
    }
  }),
  uploadId: z.string().min(1, "Audio file has not been uploaded."),
});
