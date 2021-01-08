import { useState, useEffect } from 'react';
import useSWR, { mutate } from 'swr'
import useInfiniteQuery, { PaginatedOptions } from '../hooks/useInfiniteQuery'
import { Audio, AudioListItem, AudioRequest, AudioSearchType, ErrorResponse } from '../types';
import request from '../request';
import { apiErrorToast } from '~/utils/toast';

export const useFavorite = (audioId: string) => {
  const [isFavorite, setIsFavorite] = useState<boolean | undefined>(undefined);

  useEffect(() => {
    const checkIsFollowing = async () => {
      try {
        await request(`me/audios/${audioId}/favorite`, { method: "head" });
        setIsFavorite(true);
      } catch (err) {
        setIsFavorite(false);
      }
    };
    checkIsFollowing();
  }, []);

  const favoriteHandler = async () => {
    try {
      await request(`me/audios/${audioId}/favorite`, {
        method: isFavorite ? "delete" : "put",
      });
      setIsFavorite(!isFavorite);
    } catch (err) {
      apiErrorToast(err);
    }
  }

  return { isFavorite, favorite: favoriteHandler };
}

export const useAudio = (id: string, initialData?: Audio) => {
  const { 
    data,
    isValidating: isLoading,
    error,
    mutate 
  } = useSWR<Audio, ErrorResponse>(`audios/${id}`, {
    initialData 
  });

  return { data, isLoading, error, mutate };
}

interface useAudiosInfiniteOptions extends PaginatedOptions {
  type: AudioSearchType
  username?: string
}

function generateUseAudiosKey(options: useAudiosInfiniteOptions) {
  let url = 'audios'

  if (options.type === 'feed') {
    url = `me/feed`
  } else if (options.type === 'favorites' && options.username) {
    url = `users/${options.username}/favorites`
  } else if (options.type === 'user' && options.username) {
    url = `users/${options.username}/audios`
  }

  return url;
}

export const useAudiosInfiniteQuery = (options: useAudiosInfiniteOptions = { type: 'audios' }) => {
  return useInfiniteQuery<AudioListItem, ErrorResponse>(generateUseAudiosKey(options), { 
    size: options.size,
    params: options.params
  });
}

export const uploadAudio = async (formData: FormData) => {
  const { data } = await request<Audio>('audios', {
    method: 'post',
    body: formData
  });

  return data;
}

export const deleteAudio = async (id: string | number) => {
  await request(`audios/${id}`, { method: 'delete' });
}

export const updateAudio = async (audio: Audio, inputs: AudioRequest) => {
  const { data } = await request<Audio>(`audios/${audio.id}`, {
    method: 'patch',
    body: inputs
  });
  mutate(`audios/${audio.id}`, data, false);
}