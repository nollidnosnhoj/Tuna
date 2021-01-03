import { DropzoneOptions, useDropzone } from 'react-dropzone'

const defaultValidContentTypes = [
  "audio/mpeg",
  "audio/x-mpeg",
  "audio/mp3",
  "audio/x-mp3",
  "audio/mpeg3",
  "audio/x-mpeg3",
  "audio/mpg",
  "audio/x-mpg",
  "audio/x-mpegaudio"
]

const defaultMaxFileSize = 2000000000;

const useAudioDropzone = (options: DropzoneOptions = {}) => {
  return useDropzone({
    accept: defaultValidContentTypes,
    maxSize: defaultMaxFileSize,
    multiple: false,
    ...options
  });
}

export default useAudioDropzone;