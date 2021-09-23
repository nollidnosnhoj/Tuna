import { loginValidationSchema } from "./Login";

describe("login validation schema", () => {
  it("successfully validates", () => {
    const result = loginValidationSchema.safeParse({
      login: "testuser",
      password: "password1",
    });
    expect(result.success).toBeTruthy();
  });

  it("invalidates when login is empty", () => {
    expect(() => {
      loginValidationSchema.parse({ login: "", password: "password" });
    }).toThrow();
  });

  it("invalidates when password is empty.", () => {
    expect(() => {
      loginValidationSchema.parse({ login: "testuser", password: "" });
    }).toThrow();
  });
});
