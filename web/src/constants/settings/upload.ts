type UploadSettingType = {
  accept: string[],
  maxSize: number
}

export const audioUploadSetting: UploadSettingType = {
  accept: ["audio/mp3", "audio/mpeg"],
  maxSize: 262144000
}

export const imageUploadSetting: UploadSettingType = {
  accept: ["image/jpeg", "image/png", "image/gif"],
  maxSize: 2097152
}