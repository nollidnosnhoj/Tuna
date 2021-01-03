import Axios from 'axios'
import { AuthResultResponse, LoginFormValues } from '../types';

const http = Axios.create({
  baseURL: 'http://localhost:3000',
  withCredentials: true
});

export async function login(request: LoginFormValues) {
  const { data } = await http.post<AuthResultResponse>('/api/auth/login', request);
  return data;
}

export async function refreshAccessToken() {
  const { data } = await http.post<AuthResultResponse>('/api/auth/refresh');
  return data;
}

export async function revokeRefreshToken() {
  return await http.post('/api/auth/revoke');
}