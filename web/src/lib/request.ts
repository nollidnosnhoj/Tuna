import axios from 'axios'
import createAuthRefreshInterceptor, { AxiosAuthRefreshRequestConfig } from 'axios-auth-refresh';
import { IncomingMessage } from 'http';
import { ACCESS_TOKEN_KEY } from '~/constants';
import ENVIRONMENT from '~/constants/environment'
import { getCookie } from '~/utils/cookies';
import { refreshAccessToken } from './services/auth';

type MethodType = 'get' | 'delete' | 'head' | 'post' | 'put' | 'patch'

interface NextContext {
  req?: IncomingMessage;
  ctx?: { req: IncomingMessage };
}

export interface RequestOptions<TRequest = any> {
  method?: MethodType,
  body?: TRequest,
  skipAuthRefresh?: boolean;
  ctx?: NextContext,
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

export default function request<TResponse = any>(url: string, options: RequestOptions = {}) {
  let { method = 'get', body, ctx, skipAuthRefresh = false } = options;

  const accessToken = getCookie(ACCESS_TOKEN_KEY, ctx);

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