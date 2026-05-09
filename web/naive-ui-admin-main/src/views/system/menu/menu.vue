<template>
  <div>
    <div class="n-layout-page-header">
      <n-card :bordered="false" title="菜单管理">
        维护系统菜单与路由映射。
      </n-card>
    </div>
    <n-card :bordered="false" class="mt-4">
      <n-space class="mb-4">
        <n-button type="primary" @click="openCreate">新增菜单</n-button>
      </n-space>
      <n-data-table :columns="columns" :data="menusFlat" :loading="loading" :row-key="(row) => row.id" />
    </n-card>

    <n-modal v-model:show="showModal" preset="dialog" :title="isEdit ? '编辑菜单' : '新增菜单'">
      <n-form :model="formModel" label-placement="top">
        <n-form-item label="编码">
          <n-input v-model:value="formModel.code" :disabled="isEdit" />
        </n-form-item>
        <n-form-item label="名称">
          <n-input v-model:value="formModel.name" />
        </n-form-item>
        <n-form-item label="父级菜单">
          <n-select v-model:value="formModel.parentId" clearable :options="menuOptions" />
        </n-form-item>
        <n-form-item label="路由">
          <n-input v-model:value="formModel.route" />
        </n-form-item>
        <n-form-item label="组件路径">
          <n-input v-model:value="formModel.component" />
        </n-form-item>
        <n-form-item label="图标">
          <n-input v-model:value="formModel.icon" />
        </n-form-item>
        <n-form-item label="排序">
          <n-input-number v-model:value="formModel.sort" :min="0" />
        </n-form-item>
        <n-form-item label="外链地址">
          <n-input v-model:value="formModel.linkUrl" placeholder="外链菜单时填写" />
        </n-form-item>
        <n-space>
          <n-checkbox v-model:checked="formModel.isVisible">显示</n-checkbox>
          <n-checkbox v-model:checked="formModel.isEnabled">启用</n-checkbox>
          <n-checkbox v-model:checked="formModel.isExternal">外链</n-checkbox>
          <n-checkbox v-model:checked="formModel.keepAlive">缓存页面</n-checkbox>
          <n-checkbox v-model:checked="formModel.hideInBreadcrumb">隐藏面包屑</n-checkbox>
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
import { createMenu, deleteMenu, getAuthMenus, type AuthMenu, updateMenu } from '@/api/system/auth';

const message = useMessage();
const loading = ref(false);
const saving = ref(false);
const showModal = ref(false);
const isEdit = ref(false);
const editingMenuId = ref('');
const menus = ref<AuthMenu[]>([]);

const formModel = reactive({
  code: '',
  name: '',
  parentId: undefined as string | undefined,
  route: '',
  icon: '',
  component: '',
  sort: 0,
  isVisible: true,
  isEnabled: true,
  isExternal: false,
  linkUrl: '',
  keepAlive: true,
  hideInBreadcrumb: false,
});

const menusFlat = computed(() => {
  const output: AuthMenu[] = [];
  const walk = (nodes: AuthMenu[]) => {
    nodes.forEach((node) => {
      output.push(node);
      walk(node.children ?? []);
    });
  };
  walk(menus.value);
  return output;
});

const menuOptions = computed(() =>
  menusFlat.value.map((item) => ({
    label: `${item.name} (${item.code})`,
    value: item.id,
  }))
);

async function loadMenus() {
  loading.value = true;
  try {
    menus.value = await getAuthMenus();
  } finally {
    loading.value = false;
  }
}

function openCreate() {
  isEdit.value = false;
  editingMenuId.value = '';
  formModel.code = '';
  formModel.name = '';
  formModel.parentId = undefined;
  formModel.route = '';
  formModel.icon = '';
  formModel.component = '';
  formModel.sort = 0;
  formModel.isVisible = true;
  formModel.isEnabled = true;
  formModel.isExternal = false;
  formModel.linkUrl = '';
  formModel.keepAlive = true;
  formModel.hideInBreadcrumb = false;
  showModal.value = true;
}

function openEdit(menu: AuthMenu) {
  isEdit.value = true;
  editingMenuId.value = menu.id;
  formModel.code = menu.code;
  formModel.name = menu.name;
  formModel.parentId = undefined;
  formModel.route = menu.route;
  formModel.icon = menu.icon ?? '';
  formModel.component = menu.component ?? '';
  formModel.sort = menu.sort;
  formModel.isVisible = menu.isVisible;
  formModel.isEnabled = menu.isEnabled;
  formModel.isExternal = menu.isExternal;
  formModel.linkUrl = menu.linkUrl ?? '';
  formModel.keepAlive = menu.keepAlive;
  formModel.hideInBreadcrumb = menu.hideInBreadcrumb;
  showModal.value = true;
}

async function removeMenu(menu: AuthMenu) {
  await deleteMenu(menu.id);
  message.success('菜单已删除');
  await loadMenus();
}

async function submit() {
  const code = formModel.code.trim();
  const name = formModel.name.trim();
  const route = formModel.route.trim();
  const linkUrl = formModel.linkUrl.trim();

  if (!code) {
    message.error('菜单编码不能为空');
    return;
  }

  if (!name) {
    message.error('菜单名称不能为空');
    return;
  }

  if (!route) {
    message.error('菜单路由不能为空');
    return;
  }

  if (formModel.isExternal && !linkUrl) {
    message.error('外链菜单必须填写外链地址');
    return;
  }

  if (!formModel.isExternal && linkUrl) {
    message.error('内部菜单不允许填写外链地址');
    return;
  }

  saving.value = true;
  try {
    const payload = {
      code,
      name,
      parentId: formModel.parentId,
      route,
      icon: formModel.icon.trim() || undefined,
      component: formModel.component.trim() || undefined,
      sort: formModel.sort ?? 0,
      isVisible: formModel.isVisible,
      isEnabled: formModel.isEnabled,
      isExternal: formModel.isExternal,
      linkUrl: linkUrl || undefined,
      keepAlive: formModel.keepAlive,
      hideInBreadcrumb: formModel.hideInBreadcrumb,
      description: undefined,
    };
    if (isEdit.value) {
      await updateMenu(editingMenuId.value, payload);
      message.success('菜单已更新');
    } else {
      await createMenu(payload);
      message.success('菜单已创建');
    }
    showModal.value = false;
    await loadMenus();
  } finally {
    saving.value = false;
  }
}

const columns = [
  { title: '名称', key: 'name' },
  { title: '编码', key: 'code' },
  { title: '路由', key: 'route' },
  { title: '组件', key: 'component' },
  { title: '排序', key: 'sort' },
  {
    title: '外链',
    key: 'isExternal',
    render(row: AuthMenu) {
      return row.isExternal ? '是' : '否';
    },
  },
  {
    title: '操作',
    key: 'action',
    render(row: AuthMenu) {
      return h('div', { style: 'display:flex;gap:8px;' }, [
        h(
          NButton,
          { size: 'small', type: 'primary', ghost: true, onClick: () => openEdit(row) },
          { default: () => '编辑' }
        ),
        h(
          NPopconfirm,
          { onPositiveClick: () => removeMenu(row) },
          {
            trigger: () =>
              h(
                NButton,
                { size: 'small', type: 'error', ghost: true },
                { default: () => '删除' }
              ),
            default: () => '确认删除该菜单？',
          }
        ),
      ]);
    },
  },
];

onMounted(loadMenus);
</script>
