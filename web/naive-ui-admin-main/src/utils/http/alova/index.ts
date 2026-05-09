import { createAlova } from 'alova';
import VueHook from 'alova/vue';
import adapterFetch from 'alova/fetch';
import { createAlovaMockAdapter } from '@alova/mock';
import { isString } from 'lodash-es';
import mocks from './mocks';
import { useUser } from '@/store/modules/user';
import { storage } from '@/utils/Storage';
import { useGlobSetting, useLocalSetting } from '@/hooks/setting';
import { PageEnum } from '@/enums/pageEnum';
import { ResultEnum } from '@/enums/httpEnum';
import { isUrl } from '@/utils';

const { apiUrl, urlPrefix } = useGlobSetting();

const { useMock, loggerMock } = useLocalSetting();

const mockAdapter = createAlovaMockAdapter([...mocks], {
  // 全局控制是否启用mock接口，默认为true
  enable: useMock,

  // 非模拟请求适配器，用于未匹配mock接口时发送请求
  httpAdapter: adapterFetch(),

  // mock接口响应延迟，单位毫秒
  delay: 1000,

  // 自定义打印mock接口请求信息
  // mockRequestLogger: (res) => {
  //   loggerMock && console.log(`Mock Request ${res.url}`, res);
  // },
  mockRequestLogger: loggerMock,
  onMockError(error, currentMethod) {
    console.error('🚀 ~ onMockError ~ currentMethod:', currentMethod);
    console.error('🚀 ~ onMockError ~ error:', error);
  },
});

let refreshTokenPromise: Promise<string> | null = null;

async function parseResponseBody(response: Response) {
  try {
    return await response.clone().json();
  } catch {
    return response.body;
  }
}

async function ensureAccessTokenAsync() {
  const userStore = useUser();
  if (refreshTokenPromise) {
    return refreshTokenPromise;
  }

  refreshTokenPromise = (async () => {
    const tokenResult = await userStore.refreshTokenAction();
    return tokenResult.access_token;
  })();

  try {
    return await refreshTokenPromise;
  } finally {
    refreshTokenPromise = null;
  }
}

function handleAuthExpired() {
  const Message = window['$message'];
  Message?.warning('登录已过期，请重新登录');
  storage.clear();
  window.location.href = PageEnum.BASE_LOGIN;
}

async function replayRequestWithToken(method: any, token: string) {
  const requestInit: RequestInit = {
    method: method.type ?? method.config?.method ?? 'GET',
    headers: {
      ...(method.config?.headers ?? {}),
      Authorization: `Bearer ${token}`,
    },
    body: method.data ?? method.config?.data,
  };

  const retryResponse = await fetch(method.url as string, requestInit);
  if (retryResponse.status === 401) {
    throw new Error('Session refresh failed');
  }
  return parseResponseBody(retryResponse);
}

export const Alova = createAlova({
  baseURL: apiUrl,
  statesHook: VueHook,
  // 关闭全局请求缓存
  // cacheFor: null,
  // 全局缓存配置
  // cacheFor: {
  //   POST: {
  //     mode: 'memory',
  //     expire: 60 * 10 * 1000
  //   },
  //   GET: {
  //     mode: 'memory',
  //     expire: 60 * 10 * 1000
  //   },
  //   HEAD: 60 * 10 * 1000 // 统一设置HEAD请求的缓存模式
  // },
  // 在开发环境开启缓存命中日志
  cacheLogger: process.env.NODE_ENV === 'development',
  requestAdapter: mockAdapter,
  beforeRequest(method) {
    const userStore = useUser();
    const token = userStore.getToken;
    // 默认使用 Bearer 方案向后端传递访问令牌。
    if (!method.meta?.ignoreToken && token) {
      method.config.headers.Authorization = `Bearer ${token}`;
    }
    // 处理 api 请求前缀
    const isUrlStr = isUrl(method.url as string);
    if (!isUrlStr && urlPrefix) {
      method.url = `${urlPrefix}${method.url}`;
    }
    if (!isUrlStr && apiUrl && isString(apiUrl)) {
      method.url = `${apiUrl}${method.url}`;
    }
  },
  responded: {
    onSuccess: async (response, method) => {
      let res = await parseResponseBody(response);

      if (response.status === 401 && !method.meta?.ignoreAutoRefresh) {
        try {
          const token = await ensureAccessTokenAsync();
          res = await replayRequestWithToken(method, token);
        } catch {
          const userStore = useUser();
          await userStore.logout();
          handleAuthExpired();
          throw new Error('Authentication expired');
        }
      }

      // 是否返回原生响应头 比如：需要获取响应头时使用该属性
      if (method.meta?.isReturnNativeResponse) {
        return res;
      }
      // 请根据自身情况修改数据结构
      const { message, code, result } = res;

      // 不进行任何处理，直接返回
      // 用于需要直接获取 code、result、 message 这些信息时开启
      if (method.meta?.isTransformResponse === false) {
        return res;
      }

      // @ts-ignore
      const Message = window.$message;
      // @ts-ignore
      const Modal = window.$dialog;

      const LoginPath = PageEnum.BASE_LOGIN;
      if (ResultEnum.SUCCESS === code) {
        return result;
      }
      // 兼容后端返回 401/912 两种会话失效语义。
      if (code === 912 || code === 401 || response.status === 401) {
        Modal?.warning({
          title: '提示',
          content: '登录身份已失效，请重新登录!',
          okText: '确定',
          closable: false,
          maskClosable: false,
          onOk: async () => {
            storage.clear();
            window.location.href = LoginPath;
          },
        });
      } else {
        // 可按需处理错误 一般情况下不是 912 错误，不一定需要弹出 message
        Message?.error(message);
        throw new Error(message);
      }
    },
    onError: async (error, method) => {
      const status = error?.status ?? error?.response?.status;
      if (status === 401 && !method?.meta?.ignoreAutoRefresh) {
        try {
          await ensureAccessTokenAsync();
          return;
        } catch {
          const userStore = useUser();
          await userStore.logout();
          handleAuthExpired();
        }
      }
      throw error;
    },
  },
});

// 项目，多个不同 api 地址，可导出多个实例
// export const AlovaTwo = createAlova({
//   baseURL: 'http://localhost:9001',
// });
