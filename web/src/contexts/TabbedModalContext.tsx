import React, { createContext, useMemo, useState } from "react";

interface TabbedModalContextProps {
  index: number;
  setIndex: (i: number) => Promise<void>;
}

interface TabbedModalProviderProps {
  initialIndex?: number;
}

export const TabbedModalContext = createContext<TabbedModalContextProps>({
  index: 0,
  setIndex: async () => {},
});

export const TabbedModalProvider: React.FC<TabbedModalProviderProps> = ({
  initialIndex = 0,
  children,
}) => {
  const [idx, setIdx] = useState(initialIndex);

  const updateIndex = (i: number) => {
    return new Promise<void>((resolve) => {
      setIdx(() => Math.max(0, i));
      return resolve();
    });
  };

  const values = useMemo<TabbedModalContextProps>(
    () => ({
      index: idx,
      setIndex: updateIndex,
    }),
    [idx, updateIndex]
  );

  return (
    <TabbedModalContext.Provider value={values}>
      {children}
    </TabbedModalContext.Provider>
  );
};
