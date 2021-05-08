export * from "./audioplayer";
export * from "./cookies";
export * from "./format";
export * from "./http";
export * from "./string";
export * from "./time";
export * from "./toast";
export * from "./tokens";
export * from "./validation";

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function objectToFormData(values: Record<string, any>): FormData {
  const formData = new FormData();

  Object.entries(values).forEach(([key, value]) => {
    if (value) {
      if (Array.isArray(value)) {
        value.forEach((val) => formData.append(key, val));
      } else if (value instanceof File) {
        formData.append(key, value);
      } else {
        formData.append(key, value.toString());
      }
    }
  });

  return formData;
}
