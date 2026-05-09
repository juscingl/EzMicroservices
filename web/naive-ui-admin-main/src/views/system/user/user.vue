<template>
  <div>
    <div class="n-layout-page-header">
      <n-card :bordered="false" title="用户管理">
        创建用户并分配角色、直授权限。
      </n-card>
    </div>
    <n-card :bordered="false" class="mt-4">
      <n-space class="mb-4">
        <n-button type="primary" @click="openCreate">新增用户</n-button>
      </n-space>
      <n-data-table :columns="columns" :data="users" :loading="loading" :row-key="(row) => row.id" />
    </n-card>

    <n-modal v-model:show="showModal" preset="dialog" :title="isEdit ? '编辑用户授权' : '新增用户'">
      <n-form :model="formModel" label-placement="top">
        <n-form-item label="用户名">
          <n-input v-model:value="formModel.userName" :disabled="isEdit" />
        </n-form-item>
        <n-form-item label="显示名">
          <n-input v-model:value="formModel.displayName" />
        </n-form-item>
        <n-form-item v-if="!isEdit" label="邮箱">
          <n-input v-model:value="formModel.email" />
        </n-form-item>
        <n-form-item v-if="!isEdit" label="手机号">
          <n-input v-model:value="formModel.phoneNumber" />
        </n-form-item>
        <n-form-item v-if="!isEdit" label="密码">
          <n-input v-model:value="formModel.password" type="password" />
        </n-form-item>
        <n-space>
          <n-checkbox v-model:checked="formModel.isEnabled">启用账号</n-checkbox>
        </n-space>
        <n-form-item label="角色">
          <n-select
            v-model:value="formModel.roles"
            :options="roleOptions"
            multiple
            filterable
            clearable
            placeholder="请选择启用角色"
          />
        </n-form-item>
        <n-form-item label="直授权限">
          <n-select
            v-model:value="formModel.permissionCodes"
            :options="permissionOptions"
            multiple
            filterable
            clearable
            placeholder="请选择启用权限"
          />
        </n-form-item>
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
  createUser,
  deleteUser,
  getPermissions,
  type AuthPermission,
  getRoles,
  type AuthRole,
  getUsers,
  type AuthUser,
  updateUserPermissions,
  updateUserRoles,
} from '@/api/system/auth';

const message = useMessage();
const loading = ref(false);
const saving = ref(false);
const showModal = ref(false);
const isEdit = ref(false);
const editingUserId = ref('');
const users = ref<AuthUser[]>([]);
const roles = ref<AuthRole[]>([]);
const permissions = ref<AuthPermission[]>([]);

const formModel = reactive({
  userName: '',
  displayName: '',
  email: '',
  phoneNumber: '',
  password: '',
  isEnabled: true,
  roles: [] as string[],
  permissionCodes: [] as string[],
});

const roleOptions = computed(() =>
  roles.value
    .filter((role) => role.isEnabled)
    .sort((left, right) => left.sort - right.sort)
    .map((role) => ({
      label: `${role.name} (${role.code})`,
      value: role.name,
    }))
);

const permissionOptions = computed(() =>
  permissions.value
    .filter((permission) => permission.isEnabled)
    .sort((left, right) => left.sort - right.sort)
    .map((permission) => ({
      label: `[${permission.groupName ?? permission.resource} / ${permission.scope}] ${permission.name} (${permission.code})`,
      value: permission.code,
    }))
);

async function loadUsers() {
  loading.value = true;
  try {
    const [userResult, roleResult, permissionResult] = await Promise.all([
      getUsers(),
      getRoles(),
      getPermissions(),
    ]);
    users.value = userResult;
    roles.value = roleResult;
    permissions.value = permissionResult;
  } finally {
    loading.value = false;
  }
}

function openCreate() {
  isEdit.value = false;
  editingUserId.value = '';
  formModel.userName = '';
  formModel.displayName = '';
  formModel.email = '';
  formModel.phoneNumber = '';
  formModel.password = '';
  formModel.isEnabled = true;
  formModel.roles = [];
  formModel.permissionCodes = [];
  showModal.value = true;
}

function openEdit(user: AuthUser) {
  isEdit.value = true;
  editingUserId.value = user.id;
  formModel.userName = user.userName;
  formModel.displayName = user.displayName;
  formModel.email = user.email;
  formModel.phoneNumber = user.phoneNumber ?? '';
  formModel.password = '';
  formModel.isEnabled = user.isEnabled;
  formModel.roles = user.roles;
  formModel.permissionCodes = user.directPermissions;
  showModal.value = true;
}

async function removeUser(user: AuthUser) {
  await deleteUser(user.id);
  message.success('用户已删除');
  await loadUsers();
}

async function submit() {
  saving.value = true;
  try {
    const roles = [...formModel.roles];
    const permissionCodes = [...formModel.permissionCodes];
    if (isEdit.value) {
      await updateUserRoles(editingUserId.value, roles);
      await updateUserPermissions(editingUserId.value, permissionCodes);
      message.success('用户授权已更新');
    } else {
      await createUser({
        userName: formModel.userName.trim(),
        displayName: formModel.displayName.trim(),
        email: formModel.email.trim(),
        phoneNumber: formModel.phoneNumber.trim() || undefined,
        password: formModel.password,
        isEnabled: formModel.isEnabled,
        roles,
        directPermissionCodes: permissionCodes,
      });
      message.success('用户已创建');
    }
    showModal.value = false;
    await loadUsers();
  } finally {
    saving.value = false;
  }
}

const columns = [
  { title: '用户名', key: 'userName' },
  { title: '显示名', key: 'displayName' },
  { title: '邮箱', key: 'email' },
  { title: '手机号', key: 'phoneNumber' },
  {
    title: '状态',
    key: 'isEnabled',
    render(row: AuthUser) {
      return row.isEnabled ? '启用' : '禁用';
    },
  },
  {
    title: '角色',
    key: 'roles',
    render(row: AuthUser) {
      return row.roles.join(', ');
    },
  },
  {
    title: '直授权限',
    key: 'directPermissions',
    render(row: AuthUser) {
      return row.directPermissions.join(', ');
    },
  },
  {
    title: '操作',
    key: 'action',
    render(row: AuthUser) {
      return h('div', { style: 'display:flex;gap:8px;' }, [
        h(
          NButton,
          { size: 'small', type: 'primary', ghost: true, onClick: () => openEdit(row) },
          { default: () => '编辑授权' }
        ),
        h(
          NPopconfirm,
          { onPositiveClick: () => removeUser(row) },
          {
            trigger: () =>
              h(
                NButton,
                { size: 'small', type: 'error', ghost: true },
                { default: () => '删除' }
              ),
            default: () => '确认删除该用户？',
          }
        ),
      ]);
    },
  },
];

onMounted(loadUsers);
</script>
