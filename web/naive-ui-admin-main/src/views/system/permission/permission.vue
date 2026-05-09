<template>
  <div>
    <div class="n-layout-page-header">
      <n-card :bordered="false" title="权限配置">
        维护权限编码并绑定菜单。
      </n-card>
    </div>
    <n-card :bordered="false" class="mt-4">
      <n-space class="mb-4">
        <n-button type="primary" @click="openCreate">新增权限</n-button>
      </n-space>
      <n-data-table :columns="columns" :data="permissions" :loading="loading" :row-key="(row) => row.id" />
    </n-card>

    <n-modal v-model:show="showModal" preset="dialog" :title="isEdit ? '编辑权限' : '新增权限'">
      <n-form :model="formModel" label-placement="top">
        <n-form-item label="绑定菜单">
          <n-select v-model:value="formModel.menuId" clearable :options="menuOptions" />
        </n-form-item>
        <n-form-item label="权限编码">
          <n-input v-model:value="formModel.code" :disabled="isEdit" />
        </n-form-item>
        <n-form-item label="权限名称">
          <n-input v-model:value="formModel.name" />
        </n-form-item>
        <n-form-item label="资源">
          <n-input v-model:value="formModel.resource" />
        </n-form-item>
        <n-form-item label="动作">
          <n-input v-model:value="formModel.action" />
        </n-form-item>
        <n-form-item label="类型">
          <n-select v-model:value="formModel.permissionType" :options="permissionTypeOptions" />
        </n-form-item>
        <n-form-item label="作用域">
          <n-select v-model:value="formModel.scope" :options="scopeOptions" />
        </n-form-item>
        <n-form-item label="分组">
          <n-input v-model:value="formModel.groupName" placeholder="例如：orders/security" />
        </n-form-item>
        <n-form-item label="排序">
          <n-input-number v-model:value="formModel.sort" :min="0" />
        </n-form-item>
        <n-space>
          <n-checkbox v-model:checked="formModel.isSystem">系统权限</n-checkbox>
          <n-checkbox v-model:checked="formModel.isEnabled">启用</n-checkbox>
        </n-space>
      </n-form>
      <template #action>
        <n-space>
          <n-button @click="showModal = false">取消</n-button>
          <n-button type="primary" :loading="saving" @click="submit">保存</n-button>
        </n-space>
      </template>
    </n-modal>
  </div>
</template>

<script lang="ts" setup>
import { computed, h, onMounted, reactive, ref } from 'vue';
import { NButton, NPopconfirm, useMessage } from 'naive-ui';
import {
  createPermission,
  deletePermission,
  getAuthMenus,
  getPermissions,
  type AuthMenu,
  type AuthPermission,
  updatePermission,
} from '@/api/system/auth';
import {
  getPermissionScopeLabel,
  getPermissionTypeLabel,
  PermissionScopeOptions,
  PermissionTypeOptions,
} from '@/constants/authorization';

const message = useMessage();
const loading = ref(false);
const saving = ref(false);
const showModal = ref(false);
const isEdit = ref(false);
const editingPermissionId = ref('');
const menus = ref<AuthMenu[]>([]);
const permissions = ref<AuthPermission[]>([]);

const formModel = reactive({
  menuId: undefined as string | undefined,
  code: '',
  name: '',
  resource: '',
  action: '',
  permissionType: 'action',
  scope: 'api',
  groupName: '',
  sort: 0,
  isSystem: false,
  isEnabled: true,
});

const permissionTypeOptions = PermissionTypeOptions;
const scopeOptions = PermissionScopeOptions;

const menuOptions = computed(() => {
  const output: { label: string; value: string }[] = [];
  const walk = (nodes: AuthMenu[]) => {
    nodes.forEach((node) => {
      output.push({ label: `${node.name} (${node.code})`, value: node.id });
      walk(node.children ?? []);
    });
  };
  walk(menus.value);
  return output;
});

async function loadData() {
  loading.value = true;
  try {
    const [menuResult, permissionResult] = await Promise.all([getAuthMenus(), getPermissions()]);
    menus.value = menuResult;
    permissions.value = permissionResult;
  } finally {
    loading.value = false;
  }
}

function openCreate() {
  isEdit.value = false;
  editingPermissionId.value = '';
  formModel.menuId = undefined;
  formModel.code = '';
  formModel.name = '';
  formModel.resource = '';
  formModel.action = '';
  formModel.permissionType = 'action';
  formModel.scope = 'api';
  formModel.groupName = '';
  formModel.sort = 0;
  formModel.isSystem = false;
  formModel.isEnabled = true;
  showModal.value = true;
}

function openEdit(item: AuthPermission) {
  isEdit.value = true;
  editingPermissionId.value = item.id;
  formModel.menuId = item.menuId;
  formModel.code = item.code;
  formModel.name = item.name;
  formModel.resource = item.resource;
  formModel.action = item.action;
  formModel.permissionType = item.permissionType;
  formModel.scope = item.scope;
  formModel.groupName = item.groupName ?? '';
  formModel.sort = item.sort;
  formModel.isSystem = item.isSystem;
  formModel.isEnabled = item.isEnabled;
  showModal.value = true;
}

async function removeItem(item: AuthPermission) {
  await deletePermission(item.id);
  message.success('权限已删除');
  await loadData();
}

async function submit() {
  const code = formModel.code.trim();
  const name = formModel.name.trim();
  const resource = formModel.resource.trim();
  const action = formModel.action.trim();
  const groupName = formModel.groupName.trim();
  if (!code) {
    message.error('权限编码不能为空');
    return;
  }
  if (!name) {
    message.error('权限名称不能为空');
    return;
  }
  if (!resource) {
    message.error('资源不能为空');
    return;
  }
  if (!action) {
    message.error('动作不能为空');
    return;
  }
  if (!formModel.permissionType) {
    message.error('请选择权限类型');
    return;
  }
  if (!formModel.scope) {
    message.error('请选择权限作用域');
    return;
  }
  if (!groupName) {
    message.error('分组不能为空');
    return;
  }

  saving.value = true;
  try {
    const payload = {
      menuId: formModel.menuId,
      code,
      name,
      resource,
      action,
      permissionType: formModel.permissionType,
      scope: formModel.scope,
      groupName,
      sort: formModel.sort ?? 0,
      isSystem: formModel.isSystem,
      isEnabled: formModel.isEnabled,
      description: undefined,
    };
    if (isEdit.value) {
      await updatePermission(editingPermissionId.value, payload);
      message.success('权限已更新');
    } else {
      await createPermission(payload);
      message.success('权限已创建');
    }
    showModal.value = false;
    await loadData();
  } finally {
    saving.value = false;
  }
}

const columns = [
  { title: '权限编码', key: 'code' },
  { title: '名称', key: 'name' },
  { title: '资源', key: 'resource' },
  { title: '动作', key: 'action' },
  {
    title: '类型',
    key: 'permissionType',
    render(row: AuthPermission) {
      return getPermissionTypeLabel(row.permissionType);
    },
  },
  {
    title: '作用域',
    key: 'scope',
    render(row: AuthPermission) {
      return getPermissionScopeLabel(row.scope);
    },
  },
  { title: '分组', key: 'groupName' },
  { title: '排序', key: 'sort' },
  {
    title: '操作',
    key: 'action',
    render(row: AuthPermission) {
      return h('div', { style: 'display:flex;gap:8px;' }, [
        h(
          NButton,
          { size: 'small', type: 'primary', ghost: true, onClick: () => openEdit(row) },
          { default: () => '编辑' }
        ),
        h(
          NPopconfirm,
          { onPositiveClick: () => removeItem(row) },
          {
            trigger: () =>
              h(
                NButton,
                { size: 'small', type: 'error', ghost: true },
                { default: () => '删除' }
              ),
            default: () => '确认删除该权限？',
          }
        ),
      ]);
    },
  },
];

onMounted(loadData);
</script>
