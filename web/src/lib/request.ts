import axios from 'axios'
import createAuthRefreshInterceptor, { AxiosAuthRefreshRequestConfig } from 'axios-auth-refresh'
import { IncomingMessage } from 'http'
import { ACCESS_TOKEN_KEY } from '~/constants'
import ENVIRONMENT from '~/constants/environment'
import { refreshAccessToken } from '~/lib/services/auth'
import { getCookie } from '~/utils/cookies'

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

const getBearer = (token: string) => !!token ? `Bearer ${token}` : '';

axios.defaults.baseURL = ENVIRONMENT.API_URL
axios.defaults.withCredentials = true;

const refreshAuthLogic = async (failedRequest: any) => refreshAccessToken().then(response => {
  failedRequest.response.config.headers['Authorization'] = getBearer(response.accessToken);
  axios.defaults.headers['Authorization'] = getBearer(response.accessToken);
  return Promise.resolve();
});

createAuthRefreshInterceptor(axios, refreshAuthLogic);

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

  return axios.request<TResponse>(requestConfig);
}