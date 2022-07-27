<template>
  <div class="container">
    <h2>设置</h2>
    <el-form
      :model="userConfig"
      :rules="rules"
      ref="formRef"
      label-width="110px"
      label-position="left"
      v-loading="loading"
    >
      <el-form-item prop="email" label="邮箱地址">
        <el-input v-model="userConfig.email" maxlength="50"></el-input>
      </el-form-item>
      <el-form-item prop="smtpHost" label="SMTP服务器">
        <el-input v-model="userConfig.smtpHost" maxlength="50"></el-input>
      </el-form-item>
      <el-form-item prop="smtpPort" label="SMTP端口">
        <el-input-number v-model="userConfig.smtpPort" type="number" :min="1" :max="65535"></el-input-number>
      </el-form-item>
      <el-form-item label="SMTP使用SSL">
        <el-switch v-model="userConfig.smtpSSL"></el-switch>
      </el-form-item>
      <el-form-item prop="pop3Host" label="POP3服务器">
        <el-input v-model="userConfig.pop3Host" maxlength="50"></el-input>
      </el-form-item>
      <el-form-item prop="pop3Port" label="POP3端口">
        <el-input-number v-model="userConfig.pop3Port" type="number" :min="1" :max="65535"></el-input-number>
      </el-form-item>
      <el-form-item label="POP3使用SSL">
        <el-switch v-model="userConfig.pop3SSL"></el-switch>
      </el-form-item>
      <el-button type="primary" @click="handleSave" :loading="saving">保存</el-button>
    </el-form>
  </div>
</template>

<script setup>
import { config, getConfig } from "@/api/user";
import { showSuccessPrompt, showErrorPrompt } from "@/utils/MyPrompt";

const loading = ref(true);
const saving = ref(false);

const formRef = ref(null);
const rules = ref({
  email: [{ required: true, trigger: "blur" }],
  smtpHost: [{ required: true, trigger: "blur" }],
  smtpPort: [{ required: true, trigger: "blur" }],
  pop3Host: [{ required: true, trigger: "blur" }],
  pop3Port: [{ required: true, trigger: "blur" }],
});

const userConfig = ref({
  email: "",
  smtpHost: "",
  smtpPort: 465,
  smtpSSL: true,
  pop3Host: "",
  pop3Port: 995,
  pop3SSL: true,
});

const handleSave = () => {
  saving.value = true;
  formRef.value.validate(valid => {
    if (valid) {
      config(
        userConfig.value.smtpHost,
        userConfig.value.smtpPort,
        userConfig.value.smtpSSL,
        userConfig.value.pop3Host,
        userConfig.value.pop3Port,
        userConfig.value.pop3SSL
      )
        .then(() => {
          showSuccessPrompt("保存成功");
        })
        .catch(err => {
          showErrorPrompt("保存失败", err);
        })
        .finally(() => {
          saving.value = false;
        });
    } else {
      saving.value = false;
      showErrorPrompt("保存失败", "信息未填写完整");
    }
  });
};

onMounted(() => {
  getConfig()
    .then(data => {
      userConfig.value = {
        email: data.email,
        smtpHost: data.smtp_host,
        smtpPort: data.smtp_port,
        smtpSSL: data.smtp_ssl,
        pop3Host: data.pop3_host,
        pop3Port: data.pop3_port,
        pop3SSL: data.pop3_ssl,
      };
    })
    .catch(err => {
      showErrorPrompt("获取配置失败", err);
    })
    .finally(() => {
      loading.value = false;
    });
});
</script>

<style lang="less" scoped>
.container {
  padding: 20px;
}
</style>
