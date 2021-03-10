import { identitySetting } from "./identity";
import { audioUploadSetting, imageUploadSetting } from "./upload";

export default {
  IDENTITY: identitySetting,
  UPLOAD: {
    AUDIO: audioUploadSetting,
    IMAGE: imageUploadSetting
  }
}