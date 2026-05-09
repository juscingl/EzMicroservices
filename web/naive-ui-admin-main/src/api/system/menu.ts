import { Alova } from '@/utils/http/alova/index';
export interface ListDate {
  label: string;
  key: string;
  type: number;
  subtitle: string;
  openType: number;
  auth: string;
  path: string;
  children?: ListDate[];
}

export interface BackendMenuNode {
  id: string;
  code: string;
  name: string;
  route: string;
  icon?: string;
  component?: string;
  sort: number;
  isVisible: boolean;
  isEnabled: boolean;
  children?: BackendMenuNode[];
}

/**
 * @description: 获取当前登录用户菜单
 */
export function adminMenus() {
  return Alova.Get<BackendMenuNode[]>('/auth/me/menus', {
    meta: {
      isTransformResponse: false,
    },
  });
}

/**
 * 获取tree菜单列表
 * @param params
 */
export function getMenuList(params?) {
  return Alova.Get<{ list: ListDate[] }>('/menu/list', {
    params,
  });
}
