import { useEffect, useState } from 'react';
import { useQuery, useMutation, UseQueryOptions, useQueryClient } from 'react-query'
import queryString from 'query-string'
import { apiErrorToast } from '~/utils/toast';
import request from '../request'
import { ErrorResponse, PagedList, PaginatedOptions } from '../types';
import { AudioDetail, AudioListItem, AudioSearchType } from '../types/audio'
import usePagination from '../hooks/usePagination';
import useInfinitePagination from '../hooks/useInfinitePagination';

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

interface useAudiosPaginatedOptions extends PaginatedOptions {
  type: AudioSearchType
  username?: string
}

function generateUseAudiosKey(options: useAudiosPaginatedOptions) {
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

export const useAudiosInfiniteQuery = (options: useAudiosPaginatedOptions = { type: 'audios' }) => {
  const key = generateUseAudiosKey(options);

  const params = {
    ...options.params,
    size: options.size ?? 30
  };

  const fetchAudios = async (page: number = 1) => {
    const qs = `?page=${page}&${queryString.stringify(params)}`
    const { data } = await request<PagedList<AudioListItem>>(key + qs);
    return data;
  }

  return useInfinitePagination(key, fetchAudios, params);
}

export const useAudiosPaginatedQuery = (options: useAudiosPaginatedOptions = { type: 'audios' }) => {
  const key = generateUseAudiosKey(options);

  const params = {
    ...options.params,
    size: options.size = 30 
  };

  const fetchAudios = async (page: number) => {
    const qs = `?page=${page}&${queryString.stringify(params)}`
    const { data } = await request<PagedList<AudioListItem>>(key + qs);
    return data;
  }

  return usePagination(key, fetchAudios, params);
}

export const useFavorite = (audioId: string, initialData?: boolean) => {
  const [isFavorite, setIsFavorite] = useState<boolean | undefined>(initialData);

  useEffect(() => {
    if (isFavorite === undefined) {
      (async () => {
        try {
          await request(`favorites/audios/${audioId}`, { method: "head" });
          setIsFavorite(true);
        } catch (err) {
          setIsFavorite(false);
        }
      })();
    }
  }, []);

  const favoriteHandler = async () => {
    try {
      await request(`favorites/audios/${audioId}`, {
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