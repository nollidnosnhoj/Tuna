import { useEffect, useState } from 'react';
import { useQuery, useInfiniteQuery, useMutation, QueryClient, QueryFunctionContext } from 'react-query'
import queryString from 'query-string'
import { apiErrorToast } from '~/utils/toast';
import request from '../request'
import { ErrorResponse, PaginatedOptions } from '../types';
import { AudioDetail, AudioListItem, AudioSearchType } from '../types/audio'

const queryClient = new QueryClient();

export const fetchAudioById = async (id: string) => {
  const { data } = await request<AudioDetail>(`audios/${id}`, { method: 'get' });
  return data;
}

export const useAudio = (id: string, initialData?: AudioDetail) => {
  return useQuery<AudioDetail, ErrorResponse>(['audios', id], () => fetchAudioById(id), {
    initialData: initialData
  });
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
  const key = generateUseAudiosKey(options);
  const fetchAudios = async (params?: Record<string, any>, page: number = 1) => {
    const qs = `?page=${page}&${queryString.stringify(params)}`
    const { data } = await request<AudioListItem[]>(key + qs);
    return data;
  }
  const params = {...options.params, size: options.size = 15 };
  return useInfiniteQuery<AudioListItem[], ErrorResponse>([key, params], ({ pageParam = 1 }) => 
    fetchAudios(params, pageParam), {
      getNextPageParam: (lastPage) => lastPage.length > 0,
    });
}

export const useFavorite = (audioId: string, initialData?: boolean) => {
  const [isFavorite, setIsFavorite] = useState<boolean | undefined>(initialData);

  useEffect(() => {
    if (isFavorite === undefined) {
      (async () => {
        try {
          await request(`me/audios/${audioId}/favorite`, { method: "head" });
          setIsFavorite(true);
        } catch (err) {
          setIsFavorite(false);
        }
      })();
    }
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

export const useCreateAudio = () => {
  const uploadAudio = async (formData: FormData) => {
    const { data } = await request<AudioDetail>('audios', {
      method: 'post',
      body: formData
    });
  
    return data;
  }

  return useMutation(uploadAudio, {
    onSuccess() {
      queryClient.invalidateQueries(`audios`);
    }
  })
}

export const useEditAudio = (id: string) => {
  const updateAudio = async (input: object) => {
    const { data } = await request<AudioDetail>(`audios/${id}`, { method: 'patch', body: input });
    return data;
  }

  return useMutation(updateAudio, {
    onSuccess() {
      queryClient.invalidateQueries(`audios/${id}`);
      queryClient.invalidateQueries(`audios`);
      queryClient.invalidateQueries(`me`);
    }
  })
}

export const useRemoveAudio = (id: string) => {
  const removeAudio = async () => {
    return await request(`audios/${id}`, { method: 'delete' });
  }

  return useMutation(removeAudio, {
    onSuccess() {
      queryClient.invalidateQueries(`audios/${id}`);
      queryClient.invalidateQueries(`audios`);
    }
  })
}