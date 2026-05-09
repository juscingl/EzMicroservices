import { Alova } from '@/utils/http/alova/index';

export interface LoginParams {
  username: string;
  password: string;
}

export interface OidcTokenResult {
  access_token: string;
  token_type: string;
  expires_in: number;
  refresh_token?: string;
  scope?: string;
}

export interface RefreshTokenParams {
  refreshToken: string;
}

export interface CurrentUserProfile {
  id: string;
  userName: string;
  displayName: string;
  email: string;
  phoneNumber?: string;
  isEnabled: boolean;
  roles: string[];
  permissions: string[];
  directPermissions: string[];
  menus: unknown[];
}

export interface CreateUserParams {
  userName: string;
  displayName: string;
  email: string;
  phoneNumber?: string;
  password: string;
  isEnabled: boolean;
  roles: string[];
  directPermissionCodes: string[];
}

/**
 * @description: 获取当前登录用户信息
 */
export function getUserInfo() {
  return Alova.Get<CurrentUserProfile>('/auth/me', {
    meta: {
      isTransformResponse: false,
    },
  });
}

/**
 * @description: 用户登录（OpenIddict password grant）
 */
export function login(params: LoginParams) {
  const body = new URLSearchParams();
  body.append('grant_type', 'password');
  body.append('client_id', 'eztrade.cli');
  body.append('username', params.username);
  body.append('password', params.password);
  body.append(
    'scope',
    'openid profile email roles offline_access orders inventory payments identity'
  );

  return Alova.Post<OidcTokenResult>(
    '/connect/token',
    body.toString(),
    {
      meta: {
        ignoreToken: true,
        isTransformResponse: false,
      },
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
    }
  );
}

export function refreshAccessToken(params: RefreshTokenParams) {
  const body = new URLSearchParams();
  body.append('grant_type', 'refresh_token');
  body.append('client_id', 'eztrade.cli');
  body.append('refresh_token', params.refreshToken);
  body.append(
    'scope',
    'openid profile email roles offline_access orders inventory payments identity'
  );

  return Alova.Post<OidcTokenResult>('/connect/token', body.toString(), {
    meta: {
      ignoreToken: true,
      isTransformResponse: false,
      ignoreAutoRefresh: true,
    },
    headers: {
      'Content-Type': 'application/x-www-form-urlencoded',
    },
  });
}

/**
 * @description: 用户修改密码
 */
export function changePassword(params, uid) {
  return Alova.Post(`/user/u${uid}/changepw`, { params });
}

/**
 * @description: 用户登出
 */
export function logout(params) {
  return Alova.Post('/connect/revocation', {
    params,
  });
}

/**
 * @description: 管理员创建用户（作为注册能力）
 */
export function registerByAdmin(params: CreateUserParams) {
  return Alova.Post('/auth/users', params, {
    meta: {
      isTransformResponse: false,
    },
  });
}
