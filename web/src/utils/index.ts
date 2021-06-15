export * from "./audioplayer";
export * from "./file";
export * from "./format";
export * from "./http";
export * from "./string";
export * from "./time";
export * from "./toast";
export * from "./validation";

export function dedupePromise<T extends unknown[], K>(
  func: (...args: T) => Promise<K>
): (...args: T) => Promise<K> {
  let dedupe: Promise<K> | null = null;
  return async (...args: T) => {
    if (dedupe) return dedupe;
    dedupe = func(...args);
    try {
      return await dedupe;
    } finally {
      dedupe = null;
    }
  };
}
