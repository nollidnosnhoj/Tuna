import React, { createContext, useEffect, useState } from "react";
import WaveSurfer, { WaveSurferParams } from "wavesurfer.js";

const createWavesurfer = (options: WaveSurferParams) => {
  return WaveSurfer.create(options);
};

const WavesurferContext = createContext<WaveSurfer>(null);

const WaveSurferComponent: React.FC<
  {
    container?: string;
    waveColor?: string;
    progressColor?: string;
    onMount?: (ws: WaveSurfer) => void;
    onUnmount?: () => void;
  } & any
> = ({
  container = "#wave-form",
  waveColor = "#EDF2F7",
  progressColor = "#ED64A6",
  onMount,
  onUnmount,
  children,
  ...props
}) => {
  const [waveSurfer, setWaveSurfer] = useState<WaveSurfer>(null);

  useEffect(() => {
    if (waveSurfer) {
      waveSurfer.unAll();
      waveSurfer.destroy();
      setWaveSurfer(null);
    }

    let instance = createWavesurfer({
      container,
      waveColor,
      progressColor,
      backend: "MediaElement",
      ...props,
    });

    setWaveSurfer(instance);

    if (onMount) onMount(instance);

    return () => {
      onUnmount();
      setWaveSurfer(null);
    };
  }, []);

  return (
    <WavesurferContext.Provider value={waveSurfer}>
      {children}
    </WavesurferContext.Provider>
  );
};

export default WaveSurferComponent;
