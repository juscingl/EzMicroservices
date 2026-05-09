import { Alova } from '@/utils/http/alova/index';

export interface AuthMenu {
  id: string;
  code: string;
  name: string;
  route: string;
  icon?: string;
  component?: string;
  sort: number;
  isVisible: boolean;
  isEnabled: boolean;
  isExternal: boolean;
  linkUrl?: string;
  keepAlive: boolean;
  hideInBreadcrumb: boolean;
  children: AuthMenu[];
}

export interface AuthRole {
  id: string;
  name: string;
  code: string;
  description?: string;
  sort: number;
  isEnabled: boolean;
  permissionCodes: string[];
}

export interface AuthPermission {
  id: string;
  menuId?: string;
  code: string;
  name: string;
  resource: string;
  action: string;
  permissionType: string;
  scope: string;
  groupName?: string;
  sort: number;
  isSystem: boolean;
  isEnabled: boolean;
}

export interface AuthUser {
  id: string;
  userName: string;
  displayName: string;
  email: string;
  phoneNumber?: string;
  isEnabled: boolean;
  roles: string[];
  permissions: string[];
  directPermissions: string[];
  menus: AuthMenu[];
}

export interface SaveMenuPayload {
  code: string;
  name: string;
  parentId?: string;
  route: string;
  icon?: string;
  component?: string;
  sort: number;
  isVisible: boolean;
  isEnabled: boolean;
  isExternal: boolean;
  linkUrl?: string;
  keepAlive: boolean;
  hideInBreadcrumb: boolean;
  description?: string;
}

export interface SavePermissionPayload {
  menuId?: string;
  code: string;
  name: string;
  resource: string;
  action: string;
  permissionType: string;
  scope: string;
  groupName?: string;
  sort: number;
  isSystem: boolean;
  isEnabled: boolean;
  description?: string;
}

export interface CreateUserPayload {
  userName: string;
  displayName: string;
  email: string;
  phoneNumber?: string;
  password: string;
  isEnabled: boolean;
  roles: string[];
  directPermissionCodes: string[];
}

export function getAuthMenus() {
  return Alova.Get<AuthMenu[]>('/auth/menus', { meta: { isTransformResponse: false } });
}

export function createMenu(payload: SaveMenuPayload) {
  return Alova.Post<AuthMenu>('/auth/menus', payload, { meta: { isTransformResponse: false } });
}

export function updateMenu(id: string, payload: SaveMenuPayload) {
  return Alova.Put<AuthMenu>(`/auth/menus/${id}`, payload, { meta: { isTransformResponse: false } });
}

export function deleteMenu(id: string) {
  return Alova.Delete(`/auth/menus/${id}`, { meta: { isTransformResponse: false } });
}

export function getPermissions() {
  return Alova.Get<AuthPermission[]>('/auth/permissions', { meta: { isTransformResponse: false } });
}

export function createPermission(payload: SavePermissionPayload) {
  return Alova.Post<AuthPermission>('/auth/permissions', payload, { meta: { isTransformResponse: false } });
}

export function updatePermission(id: string, payload: SavePermissionPayload) {
  return Alova.Put<AuthPermission>(`/auth/permissions/${id}`, payload, { meta: { isTransformResponse: false } });
}

export function deletePermission(id: string) {
  return Alova.Delete(`/auth/permissions/${id}`, { meta: { isTransformResponse: false } });
}

export function getRoles() {
  return Alova.Get<AuthRole[]>('/auth/roles', { meta: { isTransformResponse: false } });
}

export function createRole(payload: {
  name: string;
  code: string;
  description?: string;
  sort: number;
  isEnabled: boolean;
  permissionCodes: string[];
}) {
  return Alova.Post<AuthRole>('/auth/roles', payload, { meta: { isTransformResponse: false } });
}

export function updateRole(
  roleId: string,
  payload: { name: string; code: string; description?: string; sort: number; isEnabled: boolean }
) {
  return Alova.Put<AuthRole>(`/auth/roles/${roleId}`, payload, { meta: { isTransformResponse: false } });
}

export function updateRolePermissions(roleId: string, permissionCodes: string[]) {
  return Alova.Put<AuthRole>(`/auth/roles/${roleId}/permissions`, { permissionCodes }, { meta: { isTransformResponse: false } });
}

export function deleteRole(roleId: string) {
  return Alova.Delete(`/auth/roles/${roleId}`, { meta: { isTransformResponse: false } });
}

export function getUsers() {
  return Alova.Get<AuthUser[]>('/auth/users', { meta: { isTransformResponse: false } });
}

export function createUser(payload: CreateUserPayload) {
  return Alova.Post<AuthUser>('/auth/users', payload, { meta: { isTransformResponse: false } });
}

export function updateUserRoles(userId: string, roles: string[]) {
  return Alova.Put<AuthUser>(`/auth/users/${userId}/roles`, { roles }, { meta: { isTransformResponse: false } });
}

export function updateUserPermissions(userId: string, permissionCodes: string[]) {
  return Alova.Put<AuthUser>(
    `/auth/users/${userId}/permissions`,
    { permissionCodes },
    { meta: { isTransformResponse: false } }
  );
}

export function deleteUser(userId: string) {
  return Alova.Delete(`/auth/users/${userId}`, { meta: { isTransformResponse: false } });
}
