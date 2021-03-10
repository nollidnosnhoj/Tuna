import { useEffect, useState } from 'react';
import { useMutation, useQueryClient } from 'react-query'
import api from '~/utils/api';
import { apiErrorToast, successfulToast } from '~/utils/toast';
import { addAudioPicture } from '../services/addAudioPicture';
import { Audio, CreateAudioRequest } from '../types'

export const useAudioFavorite = (audioId: number, initialData?: boolean) => {
  const [isLoading, setLoading] = useState(false);
  const [isFavorite, setIsFavorite] = useState<boolean | undefined>(initialData);

  useEffect(() => {
    if (isFavorite === undefined) {
      (async () => {
        try {
          setLoading(true);
          await api.head(`favorites/audios/${audioId}`)
          setIsFavorite(true);
        } catch (err) {
          setIsFavorite(false);
        } finally {
          setLoading(false);
        }
      })();
    }
  }, []);

  const onFavorite = async () => {
    try {
      setLoading(true);
      const method = isFavorite ? "delete" : "put"
      await api.request(`favorites/audios/${audioId}`, method);
      successfulToast({
        message: isFavorite ? 'You have unfavorited this audio' : 'You have favorited this audio.'
      })
      setIsFavorite(!isFavorite);
    } catch (err) {
      apiErrorToast(err);
    } finally {
      setLoading(false);
    }
  }

  return { isFavorite, onFavorite, isLoading };
}

export const useCreateAudio = () => {
  const queryClient = useQueryClient();
  const uploadAudio = async (request: CreateAudioRequest) => {
    const { data } = await api.post<Audio>('audios', request);
    return data;
  }

  return useMutation(uploadAudio)
}

export const useEditAudio = (id: number) => {
  const queryClient = useQueryClient();
  const updateAudio = async (input: object) => {
    const { data } = await api.put<Audio>(`audios/${id}`, input);
    return data;
  }

  return useMutation(updateAudio, {
    onSuccess: (data) => {
      queryClient.setQueryData<Audio>([`audios`, id], data);
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