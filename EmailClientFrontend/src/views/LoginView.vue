<template>
  <div class="container">
    <div class="login-container" :style="{ width: reg ? '285px' : '225px' }">
      <h2>{{ reg ? "配置邮件服务器" : "登录" }}</h2>
      <el-form
        class="login-form"
        :model="userInfo"
        label-width="80px"
        label-position="top"
        :rules="rules"
        ref="formRef"
      >
        <el-form-item prop="email">
          <el-input v-model="userInfo.email" placeholder="邮箱地址" maxlength="50">
            <template #prefix
              ><el-icon class="el-input__icon"><i-ep-message /></el-icon
            ></template>
          </el-input>
        </el-form-item>
        <el-form-item prop="password">
          <el-input v-model="userInfo.password" type="password" placeholder="密码" show-password maxlength="40">
            <template #prefix
              ><el-icon class="el-input__icon"><i-ep-lock /></el-icon
            ></template>
          </el-input>
        </el-form-item>
        <template v-if="reg">
          <el-form-item prop="smtpHost">
            <el-input v-model="userInfo.smtpHost" placeholder="SMTP服务器" maxlength="50">
              <template #prefix
                ><el-icon class="el-input__icon"><i-ep-setUp /></el-icon
              ></template>
              <template #append>SSL&nbsp;&nbsp;<el-switch v-model="userInfo.smtpSsl"></el-switch></template>
            </el-input>
          </el-form-item>
          <el-form-item prop="pop3Host">
            <el-input v-model="userInfo.pop3Host" placeholder="POP3服务器" maxlength="50">
              <template #prefix
                ><el-icon class="el-input__icon"><i-ep-setUp /></el-icon
              ></template>
              <template #append>SSL&nbsp;&nbsp;<el-switch v-model="userInfo.pop3Ssl"></el-switch></template>
            </el-input>
          </el-form-item>
        </template>
        <el-button type="primary" @click="handleAction" id="action-btn" :loading="loading">{{
          reg ? "配置服务器信息" : "登录"
        }}</el-button>
      </el-form>
    </div>
  </div>
</template>

<script setup>
import * as user from "@/api/user";
import { showErrorPrompt } from "@/utils/MyPrompt";

const router = useRouter();

const userInfo = reactive({
  email: "",
  password: "",
  smtpHost: "",
  smtpPort: 0,
  smtpSsl: true,
  pop3Host: "",
  pop3Ssl: true,
  pop3Port: 0,
});

const reg = ref(false);

const loading = ref(false);

const formRef = ref(null);
const rules = reactive({
  email: [{ required: true, trigger: "blur" }],
  password: [{ required: true, trigger: "blur" }],
  smtpHost: [{ required: true, trigger: "blur" }],
  pop3Host: [{ required: true, trigger: "blur" }],
});

const handleAction = () => {
  loading.value = true;
  if (reg.value) {
    formRef.value.validate(valid => {
      if (valid) {
        let smtp = userInfo.smtpHost.split(":");
        let pop3 = userInfo.pop3Host.split(":");
        user
          .config(
            smtp[0],
            smtp[1] || (userInfo.smtpSsl ? 465 : 25),
            userInfo.smtpSsl,
            pop3[0],
            pop3[1] || (userInfo.pop3Ssl ? 995 : 110),
            userInfo.pop3Ssl
          )
          .then(() => {
            router.replace("/home/inbox");
          })
          .catch(err => {
            showErrorPrompt("配置失败", err);
          })
          .finally(() => {
            loading.value = false;
          });
      } else {
        showErrorPrompt("配置失败", "信息未填写完整");
        loading.value = false;
        return false;
      }
    });
  } else {
    user
      .login(userInfo.email, userInfo.password)
      .then(res => {
        if (res.is_active) {
          router.replace("/home/inbox");
        } else {
          reg.value = true;
          userInfo.smtpHost = userInfo.email.replace(/.+@/, "smtp.");
          userInfo.pop3Host = userInfo.email.replace(/.+@/, "pop.");
        }
      })
      .catch(err => {
        showErrorPrompt("登录失败", err);
      })
      .finally(() => {
        loading.value = false;
      });
  }
};
</script>

<style lang="less">
.container {
  background-color: var(--primary-bg);
}

.login-container {
  position: fixed;
  padding: 32px;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  background-color: #fff;
  backdrop-filter: blur(10px);
  -webkit-backdrop-filter: blur(10px);
  border-radius: 20px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.05);
  transition: all 0.3s ease;

  @media (prefers-color-scheme: dark) {
    background-color: var(--secondary-bg);
  }

  h2 {
    font-size: 32px;
    font-weight: 500;
    margin-bottom: 16px;
  }

  .el-form-item {
    width: 100%;
  }

  #action-btn {
    width: 75%;
    display: block;
    margin: 0 auto;
  }
}
</style>
