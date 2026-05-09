<template>
  <div>
    <div class="n-layout-page-header">
      <n-card :bordered="false" title="角色管理">
        维护角色并分配权限编码。
      </n-card>
    </div>
    <n-card :bordered="false" class="mt-4">
      <n-space class="mb-4">
        <n-button type="primary" @click="openCreate">新增角色</n-button>
      </n-space>
      <n-data-table :columns="columns" :data="roles" :loading="loading" :row-key="(row) => row.id" />
    </n-card>

    <n-modal v-model:show="showModal" preset="dialog" :title="isEdit ? '编辑角色权限' : '新增角色'">
      <n-form :model="formModel" label-placement="top">
        <n-form-item label="角色名称">
          <n-input v-model:value="formModel.name" :disabled="isEdit" placeholder="请输入角色名称" />
        </n-form-item>
        <n-form-item label="角色编码">
          <n-input v-model:value="formModel.code" :disabled="isEdit" placeholder="例如：admin" />
        </n-form-item>
        <n-form-item label="角色描述">
          <n-input v-model:value="formModel.description" placeholder="请输入角色描述" />
        </n-form-item>
        <n-form-item label="排序">
          <n-input-number v-model:value="formModel.sort" :min="0" />
        </n-form-item>
        <n-space>
          <n-checkbox v-model:checked="formModel.isEnabled">启用角色</n-checkbox>
        </n-space>
        <n-form-item label="权限">
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
  createRole,
  deleteRole,
  getPermissions,
  getRoles,
  type AuthPermission,
  type AuthRole,
  updateRole,
  updateRolePermissions,
} from '@/api/system/auth';

const message = useMessage();
const loading = ref(false);
const saving = ref(false);
const showModal = ref(false);
const isEdit = ref(false);
const editingRoleId = ref('');
const roles = ref<AuthRole[]>([]);
const permissions = ref<AuthPermission[]>([]);

const formModel = reactive({
  name: '',
  code: '',
  description: '',
  sort: 100,
  isEnabled: true,
  permissionCodes: [] as string[],
});

const permissionOptions = computed(() =>
  permissions.value
    .filter((permission) => permission.isEnabled)
    .sort((left, right) => left.sort - right.sort)
    .map((permission) => ({
      label: `[${permission.groupName ?? permission.resource} / ${permission.scope}] ${permission.name} (${permission.code})`,
      value: permission.code,
    }))
);

async function loadRoles() {
  loading.value = true;
  try {
    const [roleResult, permissionResult] = await Promise.all([getRoles(), getPermissions()]);
    roles.value = roleResult;
    permissions.value = permissionResult;
  } finally {
    loading.value = false;
  }
}

function openCreate() {
  isEdit.value = false;
  editingRoleId.value = '';
  formModel.name = '';
  formModel.code = '';
  formModel.description = '';
  formModel.sort = 100;
  formModel.isEnabled = true;
  formModel.permissionCodes = [];
  showModal.value = true;
}

function openEdit(role: AuthRole) {
  isEdit.value = true;
  editingRoleId.value = role.id;
  formModel.name = role.name;
  formModel.code = role.code;
  formModel.description = role.description ?? '';
  formModel.sort = role.sort;
  formModel.isEnabled = role.isEnabled;
  formModel.permissionCodes = [...role.permissionCodes];
  showModal.value = true;
}

async function removeRole(role: AuthRole) {
  await deleteRole(role.id);
  message.success('角色已删除');
  await loadRoles();
}

async function submit() {
  const permissionCodes = [...formModel.permissionCodes];
  saving.value = true;
  try {
    if (isEdit.value) {
      await updateRole(editingRoleId.value, {
        name: formModel.name.trim(),
        code: formModel.code.trim(),
        description: formModel.description.trim() || undefined,
        sort: formModel.sort ?? 100,
        isEnabled: formModel.isEnabled,
      });
      await updateRolePermissions(editingRoleId.value, permissionCodes);
      message.success('角色权限已更新');
    } else {
      await createRole({
        name: formModel.name.trim(),
        code: formModel.code.trim(),
        description: formModel.description.trim() || undefined,
        sort: formModel.sort ?? 100,
        isEnabled: formModel.isEnabled,
        permissionCodes,
      });
      message.success('角色已创建');
    }
    showModal.value = false;
    await loadRoles();
  } finally {
    saving.value = false;
  }
}

const columns = [
  { title: '角色ID', key: 'id' },
  { title: '角色名称', key: 'name' },
  { title: '编码', key: 'code' },
  { title: '描述', key: 'description' },
  { title: '排序', key: 'sort' },
  {
    title: '状态',
    key: 'isEnabled',
    render(row: AuthRole) {
      return row.isEnabled ? '启用' : '禁用';
    },
  },
  {
    title: '权限编码',
    key: 'permissionCodes',
    render(row: AuthRole) {
      return row.permissionCodes.join(', ');
    },
  },
  {
    title: '操作',
    key: 'action',
    render(row: AuthRole) {
      return h('div', { style: 'display:flex;gap:8px;' }, [
        h(
          NButton,
          { size: 'small', type: 'primary', ghost: true, onClick: () => openEdit(row) },
          { default: () => '编辑权限' }
        ),
        h(
          NPopconfirm,
          { onPositiveClick: () => removeRole(row) },
          {
            trigger: () =>
              h(
                NButton,
                { size: 'small', type: 'error', ghost: true },
                { default: () => '删除' }
              ),
            default: () => '确认删除该角色？',
          }
        ),
      ]);
    },
  },
];

onMounted(loadRoles);
</script>
