import {
  passwordRule,
  PasswordRulesType,
  usernameRule,
  UsernameRulesType,
} from "./schemas";

describe("username update schema validation", () => {
  const label = "Username";
  const rules: UsernameRulesType = {
    allowedCharacters: "0123456789abcdefghijklmnopqrstuvwxyz_-",
    maxLength: 30,
    minLength: 5,
  };
  it("successfully validate", () => {
    const result = usernameRule(label, rules).safeParse("testuser_123");
    expect(result.success).toBeTruthy();
  });

  it("invalid when username has invalid characters", () => {
    const testFunc = (): void => {
      usernameRule(label, rules).parse("testuser!");
    };
    expect(testFunc).toThrow();
  });

  it("invalid when username is less than minLength.", () => {
    const testFunc = (): void => {
      usernameRule(label, rules).parse("hi");
    };
    expect(testFunc).toThrow();
  });

  it("invalid when username is more than maxLength.", () => {
    const testFunc = (): void => {
      usernameRule(label, { ...rules, maxLength: 10 }).parse(
        "morethan10charactershaha"
      );
    };
    expect(testFunc).toThrow();
  });
});

describe("password update schema validation", () => {
  const label = "Password";
  const rules: PasswordRulesType = {
    minLength: 5,
    requiresDigit: false,
    requiresLowercase: false,
    requiresNonAlphanumeric: false,
    requiresUppercase: false,
  };

  it("successfully validate", () => {
    const result = passwordRule(label, {
      ...rules,
      requiresDigit: true,
      requiresLowercase: true,
      requiresNonAlphanumeric: true,
      requiresUppercase: true,
    }).safeParse("Password1!");
    expect(result.success).toBeTruthy();
  });

  it("invalid when password is less than minLength.", () => {
    expect((): void => {
      passwordRule(label, rules).parse("hi");
    }).toThrow();
  });

  it("invalid when password requires digits", () => {
    expect((): void => {
      passwordRule(label, { ...rules, requiresDigit: true }).parse("password");
    }).toThrow();
  });

  it("invalid when password requires lowercase", () => {
    expect(() => {
      passwordRule(label, { ...rules, requiresLowercase: true }).parse(
        "PASSWORD"
      );
    }).toThrow();
  });

  it("invalid when password requires uppercase", () => {
    expect(() => {
      passwordRule(label, { ...rules, requiresUppercase: true }).parse(
        "password"
      );
    }).toThrow();
  });

  it("invalid when password requires nonalphanumeric", () => {
    expect(() => {
      passwordRule(label, { ...rules, requiresNonAlphanumeric: true }).parse(
        "password"
      );
    }).toThrow();
  });
});
