import WaveSurfer, { WaveSurferParams } from "wavesurfer.js";

interface CreateWavesurferParams extends Omit<WaveSurferParams, 'container'> {}

export const createWavesurfer = (ref: any, options?: CreateWavesurferParams): WaveSurfer => {
  return WaveSurfer.create({
    container: ref.current,
    ...options
  });
};
