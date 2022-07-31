import { getFilenameWithoutExtension } from "~/utils/file";

describe("getFilenameWithoutExtension", () => {
  test("Successfully obtain filename without extension", () => {
    const filename = "test.mp3";
    const fileNameWithoutExt = getFilenameWithoutExtension(filename);
    expect(fileNameWithoutExt).toBe("test");
  });

  test("Returns input if it does not have extension", () => {
    const filename = "testmp3";
    const fileNameWithoutExt = getFilenameWithoutExtension(filename);
    expect(fileNameWithoutExt).toBe(filename);
  });
});
