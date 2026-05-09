export const PermissionTypeValues = ['menu', 'scope', 'page', 'action', 'button'] as const;
export const PermissionScopeValues = ['api', 'page', 'menu', 'button', 'scope', 'action'] as const;

export type PermissionTypeValue = (typeof PermissionTypeValues)[number];
export type PermissionScopeValue = (typeof PermissionScopeValues)[number];

export const PermissionTypeLabelMap: Record<PermissionTypeValue, string> = {
  menu: '菜单',
  scope: '范围',
  page: '页面',
  action: '动作',
  button: '按钮',
};

export const PermissionScopeLabelMap: Record<PermissionScopeValue, string> = {
  api: 'API',
  page: '页面',
  menu: '菜单',
  button: '按钮',
  scope: '范围',
  action: '动作',
};

export const PermissionTypeOptions = [
  { label: PermissionTypeLabelMap.menu, value: 'menu' },
  { label: PermissionTypeLabelMap.scope, value: 'scope' },
  { label: PermissionTypeLabelMap.page, value: 'page' },
  { label: PermissionTypeLabelMap.action, value: 'action' },
  { label: PermissionTypeLabelMap.button, value: 'button' },
] as const;

export const PermissionScopeOptions = [
  { label: PermissionScopeLabelMap.api, value: 'api' },
  { label: PermissionScopeLabelMap.page, value: 'page' },
  { label: PermissionScopeLabelMap.menu, value: 'menu' },
  { label: PermissionScopeLabelMap.button, value: 'button' },
  { label: PermissionScopeLabelMap.scope, value: 'scope' },
  { label: PermissionScopeLabelMap.action, value: 'action' },
] as const;

export function getPermissionTypeLabel(value: string): string {
  return PermissionTypeLabelMap[value as PermissionTypeValue] ?? value;
}

export function getPermissionScopeLabel(value: string): string {
  return PermissionScopeLabelMap[value as PermissionScopeValue] ?? value;
}
