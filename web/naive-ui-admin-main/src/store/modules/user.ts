import { defineStore } from 'pinia';
import { store } from '@/store';
import { ACCESS_TOKEN, CURRENT_USER, IS_SCREENLOCKED, REFRESH_TOKEN } from '@/store/mutation-types';
import {
  getUserInfo as getUserInfoApi,
  login,
  refreshAccessToken,
  type CurrentUserProfile,
  type LoginParams,
} from '@/api/system/user';
import { storage } from '@/utils/Storage';

export type UserInfoType = {
  username: string;
  userName: string;
  email: string;
  roles?: string[];
  permissions?: string[];
  menus?: unknown[];
};

export interface IUserState {
  token: string;
  refreshToken: string;
  username: string;
  welcome: string;
  avatar: string;
  permissions: string[];
  info: UserInfoType;
}

export const useUserStore = defineStore({
  id: 'app-user',
  state: (): IUserState => ({
    token: storage.get(ACCESS_TOKEN, ''),
    refreshToken: storage.get(REFRESH_TOKEN, ''),
    username: '',
    welcome: '',
    avatar: '',
    permissions: [],
    info: storage.get(CURRENT_USER, { username: '', userName: '', email: '' }),
  }),
  getters: {
    getToken(): string {
      return this.token;
    },
    getRefreshToken(): string {
      return this.refreshToken;
    },
    getAvatar(): string {
      return this.avatar;
    },
    getNickname(): string {
      return this.username;
    },
    getPermissions(): string[] {
      return this.permissions;
    },
    getUserInfo(): UserInfoType {
      return this.info;
    },
  },
  actions: {
    setToken(token: string) {
      this.token = token;
    },
    setRefreshToken(refreshToken: string) {
      this.refreshToken = refreshToken;
    },
    setAvatar(avatar: string) {
      this.avatar = avatar;
    },
    setPermissions(permissions: string[]) {
      this.permissions = permissions;
    },
    setUserInfo(info: UserInfoType) {
      this.info = info;
      this.username = info.username || info.userName || '';
    },
    // 登录
    async login(params: LoginParams) {
      const tokenResult = await login(params);
      const token = tokenResult.access_token;
      const ex = Math.max(tokenResult.expires_in ?? 3600, 300);
      storage.set(ACCESS_TOKEN, token, ex);
      storage.set(REFRESH_TOKEN, tokenResult.refresh_token ?? '', 7 * 24 * 60 * 60);
      storage.set(IS_SCREENLOCKED, false);
      this.setToken(token);
      this.setRefreshToken(tokenResult.refresh_token ?? '');
      return tokenResult;
    },

    async refreshTokenAction() {
      if (!this.refreshToken) {
        throw new Error('No refresh token');
      }
      const tokenResult = await refreshAccessToken({ refreshToken: this.refreshToken });
      const token = tokenResult.access_token;
      const ex = Math.max(tokenResult.expires_in ?? 3600, 300);
      storage.set(ACCESS_TOKEN, token, ex);
      storage.set(REFRESH_TOKEN, tokenResult.refresh_token ?? this.refreshToken, 7 * 24 * 60 * 60);
      this.setToken(token);
      this.setRefreshToken(tokenResult.refresh_token ?? this.refreshToken);
      return tokenResult;
    },

    // 获取用户信息
    async getInfo() {
      const result = (await getUserInfoApi()) as CurrentUserProfile;
      const permissionsList = Array.isArray(result.permissions) ? result.permissions : [];
      this.setPermissions(permissionsList);
      const normalizedInfo: UserInfoType = {
        ...result,
        username: result.userName ?? '',
      };
      this.setUserInfo(normalizedInfo);
      storage.set(CURRENT_USER, normalizedInfo, 7 * 24 * 60 * 60);
      this.setAvatar('');
      return normalizedInfo;
    },

    // 登出
    async logout() {
      this.setPermissions([]);
      this.setUserInfo({ username: '', userName: '', email: '' });
      storage.remove(ACCESS_TOKEN);
      storage.remove(REFRESH_TOKEN);
      storage.remove(CURRENT_USER);
      this.setToken('');
      this.setRefreshToken('');
    },
  },
});

// Need to be used outside the setup
export function useUser() {
  return useUserStore(store);
}
