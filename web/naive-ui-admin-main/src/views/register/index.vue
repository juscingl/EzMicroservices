<template>
  <div class="register-page">
    <div class="register-card">
      <h2>创建账号</h2>
      <p class="desc">该页面用于管理员创建用户账号。</p>
      <n-form ref="formRef" :model="formModel" :rules="rules" label-placement="top">
        <n-form-item label="用户名" path="userName">
          <n-input v-model:value="formModel.userName" placeholder="请输入用户名" />
        </n-form-item>
        <n-form-item label="邮箱" path="email">
          <n-input v-model:value="formModel.email" placeholder="请输入邮箱" />
        </n-form-item>
        <n-form-item label="密码" path="password">
          <n-input v-model:value="formModel.password" type="password" placeholder="请输入密码" />
        </n-form-item>
        <n-form-item label="角色（逗号分隔）" path="rolesText">
          <n-input v-model:value="formModel.rolesText" placeholder="例如：admin,operator" />
        </n-form-item>
        <n-space justify="end">
          <n-button @click="goLogin">返回登录</n-button>
          <n-button type="primary" :loading="submitting" @click="submit">创建用户</n-button>
        </n-space>
      </n-form>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { reactive, ref } from 'vue';
import { useMessage } from 'naive-ui';
import { useRouter } from 'vue-router';
import { registerByAdmin } from '@/api/system/user';
import { useUserStore } from '@/store/modules/user';
import { PageEnum } from '@/enums/pageEnum';

const router = useRouter();
const message = useMessage();
const userStore = useUserStore();
const formRef = ref();
const submitting = ref(false);

const formModel = reactive({
  userName: '',
  email: '',
  password: '',
  rolesText: 'admin',
});

const rules = {
  userName: { required: true, message: '请输入用户名', trigger: 'blur' },
  email: { required: true, message: '请输入邮箱', trigger: 'blur' },
  password: { required: true, message: '请输入密码', trigger: 'blur' },
};

function goLogin() {
  router.push(PageEnum.BASE_LOGIN);
}

function parseRoles(value: string): string[] {
  return value
    .split(',')
    .map((item) => item.trim())
    .filter((item) => item.length > 0);
}

function submit(event: Event) {
  event.preventDefault();
  formRef.value?.validate(async (errors) => {
    if (errors) {
      return;
    }
    if (!userStore.getToken) {
      message.error('当前未登录管理员账号，无法创建用户。');
      return;
    }
    submitting.value = true;
    try {
      await registerByAdmin({
        userName: formModel.userName,
        displayName: formModel.userName,
        email: formModel.email,
        phoneNumber: undefined,
        password: formModel.password,
        isEnabled: true,
        roles: parseRoles(formModel.rolesText),
        directPermissionCodes: [],
      });
      message.success('用户创建成功');
      router.push(PageEnum.BASE_LOGIN);
    } finally {
      submitting.value = false;
    }
  });
}
</script>

<style scoped>
.register-page {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: #f5f7fa;
}
.register-card {
  width: 460px;
  background: #fff;
  border-radius: 8px;
  padding: 24px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.08);
}
.desc {
  color: #666;
  margin-bottom: 16px;
}
</style>
