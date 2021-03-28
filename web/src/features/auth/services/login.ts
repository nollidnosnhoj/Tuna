import Axios from 'axios';
import { LoginFormValues } from '~/features/auth/components/LoginForm';
import { AuthResult } from '../types';


export async function authenticateUser(request: LoginFormValues) {
  const { data } = await Axios.post<AuthResult>('/api/auth/login', request, {
    withCredentials: true
  });
  return data;
}
