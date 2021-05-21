export function getFilenameWithoutExtension(filename: string): string {
  if (!filename.includes(".")) return filename;
  return filename.split(".").slice(0, -1).join(".");
}

export function getDurationFromAudioFile(file: File): Promise<number> {
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
