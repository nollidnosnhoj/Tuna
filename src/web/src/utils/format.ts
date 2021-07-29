export const formatFileSize = (size: number): string => {
  if (size === 0) return "0 Bytes";
  const units = ["Bytes", "kB", "MB", "GB"];

  // get exponential
  const e = Math.min(
    Math.floor(Math.log(size) / Math.log(1024)),
    units.length - 1
  );

  // conver the bytes into the exponential unit byte
  size = Number((size / Math.pow(1024, e)).toPrecision(3));
  return size.toLocaleString() + " " + units[e];
};

export const formatDuration = (seconds?: number): string | null => {
  const pad = (seconds: number): string => {
    return ("0" + seconds).slice(-2);
  };
  if (seconds === undefined || !isFinite(seconds)) return null;
  const date = new Date(seconds * 1000);
  const hh = date.getUTCHours();
  const mm = date.getUTCMinutes();
  const ss = pad(date.getUTCSeconds());
  if (hh) {
    return `${hh}:${pad(mm)}:${ss}`;
  }
  return `${mm}:${ss}`;
};
