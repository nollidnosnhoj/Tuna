import { stringifyQueryObject } from "./http";

describe("stringifyQueryObject", () => {
  it("Successfully stringify object", () => {
    const mock = {
      paramOne: "hello",
      paramTwo: true,
      paramThree: 10,
    };
    const expected = "?paramOne=hello&paramTwo=true&paramThree=10";
    const result = stringifyQueryObject(mock);
    expect(result).toBe(expected);
  });
});
