import Axios from 'axios'
import { LoginFormValues } from '~/features/auth/components/LoginForm'
import { AuthResult } from '../types';

export async function login(request: LoginFormValues) {
  const { data } = await Axios.post<AuthResult>('/api/auth/login', request, {
    withCredentials: true
  });
  return data;
}

export async function refreshAccessToken() {
  const { data } = await Axios.post<AuthResult>('/api/auth/refresh', undefined, {
    withCredentials: true
  });
  return data;
}

export async function revokeRefreshToken() {
  return await Axios.post('/api/auth/revoke', undefined, {
    withCredentials: true,
    validateStatus: (status) => status < 500
  });
}