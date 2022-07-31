import { objectToFormData, stringifyQueryObject } from "~/utils/http";

describe("objectToFormData", () => {
  it("Successfully transform JS Object into FormData", () => {
    const obj = {
      test1: "hello",
      test2: "world",
    };
    const fd = objectToFormData(obj);
    expect(fd.has("test1")).toBeTruthy();
    expect(fd.has("test2")).toBeTruthy();
    expect(fd.get("test1")).toBe("hello");
    expect(fd.get("test2")).toBe("world");
  });
});

describe("stringifyQueryObject", () => {
  it("Successfully stringify object", () => {
    const mock = {
      paramOne: "hello",
      paramTwo: true,
      paramThree: 10,
    };
    const expected = "paramOne=hello&paramTwo=true&paramThree=10";
    const result = stringifyQueryObject(mock);
    expect(result).toBe(expected);
  });
});
