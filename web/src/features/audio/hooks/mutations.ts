import { useEffect, useState } from 'react';
import { useMutation, useQueryClient } from 'react-query'
import api from '~/utils/api';
import { apiErrorToast, successfulToast } from '~/utils/toast';
import { addAudioPicture } from '../services/addAudioPicture';
import { AudioDetail, CreateAudioRequest } from '../types'

export const useCreateAudio = () => {
  const queryClient = useQueryClient();
  const uploadAudio = async (request: CreateAudioRequest) => {
    const { data } = await api.post<AudioDetail>('audios', request);
    return data;
  }

  return useMutation(uploadAudio)
}

export const useEditAudio = (id: number) => {
  const queryClient = useQueryClient();
  const updateAudio = async (input: object) => {
    const { data } = await api.put<AudioDetail>(`audios/${id}`, input);
    return data;
  }

  return useMutation(updateAudio, {
    onSuccess: (data) => {
      queryClient.setQueryData<AudioDetail>([`audios`, id], data);
      queryClient.invalidateQueries(`audios`);
    }
  })
}

export const useRemoveAudio = (id: number) => {
  const queryClient = useQueryClient();
  const removeAudio = async () => await api.delete(`audios/${id}`);

  return useMutation(removeAudio, {
    onSuccess() {
      queryClient.invalidateQueries(`audios`);
      queryClient.invalidateQueries([`audios`, id], { exact: true })
    }
  })
}

export const useAddAudioPicture = (id: number) => {
  const queryClient = useQueryClient();
  const uploadArtwork = async (data: string) => {
    return await addAudioPicture(id, data);
  }

  return useMutation(uploadArtwork, {
    onSuccess(data) {
      queryClient.invalidateQueries(`audios`);
      queryClient.invalidateQueries([`audios`, id], { exact: true })
    }
  })
}