import axios from 'axios'
import createAuthRefreshInterceptor, { AxiosAuthRefreshRequestConfig } from 'axios-auth-refresh';
import { IncomingMessage } from 'http';
import Router from 'next/router'
import { ACCESS_TOKEN_KEY } from '~/constants';
import ENVIRONMENT from '~/constants/environment'
import { isAxiosError } from '~/utils';
import { getCookie } from '~/utils/cookies';
import { errorToast } from '~/utils/toast';
import { refreshAccessToken } from './services/auth';

type MethodType = 'get' | 'delete' | 'head' | 'post' | 'put' | 'patch'

export interface RequestOptions<TRequest = any> {
  method?: MethodType,
  body?: TRequest,
  accessToken?: string;
  skipAuthRefresh?: boolean;
}

function getBearer(token: string) {
  return token ? `Bearer ${token}` : ''
}

const requestClient = axios.create();

requestClient.defaults.baseURL = ENVIRONMENT.API_URL;
requestClient.defaults.withCredentials = true;

const refreshAuthLogic = async (failedRequest: any) => refreshAccessToken().then(response => {
  failedRequest.response.config.headers['Authorization'] = getBearer(response.accessToken);
  axios.defaults.headers['Authorization'] = getBearer(response.accessToken);
  return Promise.resolve();
});

createAuthRefreshInterceptor(requestClient, refreshAuthLogic);

requestClient.interceptors.request.use(request => {
  const accessToken = getCookie(ACCESS_TOKEN_KEY);
  if (!request.headers.Authorization) {
    request.headers.Authorization = getBearer(accessToken);
  }
  return request;
})

export default function request<TResponse = any>(url: string, options: RequestOptions = {}) {
  let { method = 'get', body, accessToken, skipAuthRefresh = false } = options;

  const requestConfig: AxiosAuthRefreshRequestConfig = {
    url: url,
    method: method,
    data: body,
    skipAuthRefresh: skipAuthRefresh
  }

  if (accessToken) {
    Object.assign(requestConfig, { headers: { Authorization: getBearer(accessToken) }})
  }

  return requestClient.request<TResponse>(requestConfig);
}