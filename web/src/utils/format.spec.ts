import { formatDuration, formatFileSize } from "~/utils/format";

describe("formatFileSize", () => {
  it("Format bytes", () => {
    const expected = "500 Bytes";
    const result = formatFileSize(500);
    expect(result).toBe(expected);
  });

  it("Format kilobytes", () => {
    const expected = "20 kB";
    const result = formatFileSize(20_500);
    expect(result).toBe(expected);
  });

  it("Format megabytes", () => {
    const expected = "500 MB";
    const result = formatFileSize(524_288_000);
    expect(result).toBe(expected);
  });

  it("Format gigabytes", () => {
    const expected = "2 GB";
    const result = formatFileSize(2_147_483_648);
    expect(result).toBe(expected);
  });
});

describe("formatDuration", () => {
  it("Format zero seconds", () => {
    const expected = "0:00";
    const result = formatDuration(0) || "";
    expect(result).toBe(expected);
  });

  it("Format less than one minute", () => {
    const expected = "0:40";
    const result = formatDuration(40);
    expect(result).toBe(expected);
  });

  it("Format slightly more than one minute", () => {
    const expected = "1:20";
    const result = formatDuration(80);
    expect(result).toBe(expected);
  });

  it("Format more than 10 minutes", () => {
    const expected = "30:30";
    const result = formatDuration(1830);
    expect(result).toBe(expected);
  });

  it("Format more than a hour", () => {
    const expected = "1:00:30";
    const result = formatDuration(3630);
    expect(result).toBe(expected);
  });
});
