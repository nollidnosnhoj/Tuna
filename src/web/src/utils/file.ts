/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
export function getFilenameWithoutExtension(filename: string): string {
  if (!filename.includes(".")) return filename;
  return filename.split(".").slice(0, -1).join(".");
}

export function getDurationFromAudioFile(file: any): Promise<number> {
  if (!(file instanceof File)) {
    throw Error("Input must be a file.");
  }
  return new Promise<number>((resolve, reject) => {
    if (typeof window === "undefined") return reject("Window is not defined.");
    const audioEle = new Audio();
    audioEle.src = window.URL.createObjectURL(file);
    audioEle.onloadedmetadata = () => {
      resolve(audioEle.duration);
    };
    audioEle.onerror = () => {
      reject("Unable to load audio's metadata.");
    };
  });
}
