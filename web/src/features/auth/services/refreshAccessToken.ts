import Axios from 'axios';
import { AuthResult } from '../types';


export async function refreshAccessToken() {
  const { data } = await Axios.post<AuthResult>('/api/auth/refresh', undefined, {
    withCredentials: true
  });
  return data;
}
