import axios from 'axios'
import createAuthRefreshInterceptor from 'axios-auth-refresh';
import CONSTANTS from '~/constants'
import { getAccessToken } from '~/utils/cookies';
import { refreshAccessToken } from './services/auth';

export function getBearer(token: string) {
  return token ? `Bearer ${token}` : ''
}

const apiAxios = axios.create({
  baseURL: CONSTANTS.API_URL,
  withCredentials: true
});

createAuthRefreshInterceptor(apiAxios, async (failedRequest: any) => {
  return refreshAccessToken().then(response => {
    failedRequest.response.config.headers['Authorization'] = getBearer(response.accessToken);
    apiAxios.defaults.headers['Authorization'] = getBearer(response.accessToken);
    return Promise.resolve();
  });
});

apiAxios.interceptors.request.use(request => {
  if (!request.headers.Authorization) {
    const accessToken = getAccessToken();
    request.headers.Authorization = getBearer(accessToken);
  }
  return request;
})

export default apiAxios;