import { useEffect, useState } from 'react';
import { useQuery, useInfiniteQuery, useMutation, QueryClient, UseQueryOptions, useQueryClient } from 'react-query'
import queryString from 'query-string'
import { apiErrorToast } from '~/utils/toast';
import request from '../request'
import { ErrorResponse, PaginatedOptions } from '../types';
import { AudioDetail, AudioListItem, AudioSearchType } from '../types/audio'

interface FetchAudioByIdOptions {
  accessToken?: string;
}

export const fetchAudioById = async (id: string, options: FetchAudioByIdOptions = {}) => {
  const { data } = await request<AudioDetail>(`audios/${id}`, { 
    method: 'get',
    accessToken: options.accessToken
  });
  return data;
}

export const useAudio = (id: string, options: UseQueryOptions<AudioDetail, ErrorResponse> = {}) => {
  return useQuery<AudioDetail, ErrorResponse>(['audios', id], () => fetchAudioById(id), options);
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
      getNextPageParam: (lastPage, allPages) => {
        return lastPage.length > 0 ? allPages.length + 1 : undefined
      },
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
  const queryClient = useQueryClient();
  const uploadAudio = async (formData: FormData) => {
    const { data } = await request<AudioDetail>('audios', {
      method: 'post',
      body: formData
    });
  
    return data;
  }

  return useMutation(uploadAudio, {
    onSuccess() {
      queryClient.invalidateQueries(`audios`, { exact: true });
    }
  })
}

export const useEditAudio = (id: string) => {
  const queryClient = useQueryClient();
  const updateAudio = async (input: object) => {
    const { data } = await request<AudioDetail>(`audios/${id}`, { method: 'patch', body: input });
    return data;
  }

  return useMutation(updateAudio, {
    onSuccess: (data) => {
      queryClient.setQueryData<AudioDetail>([`audios`, id], data);
      queryClient.invalidateQueries(`audios`, { exact: true });
    }
  })
}

export const useRemoveAudio = (id: string) => {
  const queryClient = useQueryClient();
  const removeAudio = async () => {
    return await request(`audios/${id}`, { method: 'delete' });
  }

  return useMutation(removeAudio, {
    onSuccess() {
      queryClient.invalidateQueries(`audios`);
      queryClient.invalidateQueries([`audios`, id], { exact: true })
    }
  })
}