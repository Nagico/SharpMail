<template>
  <div class="container">
    <h2>发送邮件</h2>
    <el-form :model="mail" label-width="5em" ref="formRef" :rules="formRules">
      <el-form-item label="收件人" prop="to">
        <ReceiverList v-model="mail.to" />
      </el-form-item>
      <el-form-item label="抄送">
        <ReceiverList v-model="mail.cc" type="success" />
      </el-form-item>
      <el-form-item label="密送">
        <ReceiverList v-model="mail.bcc" type="danger" />
      </el-form-item>
      <el-form-item label="主题" prop="subject">
        <el-input v-model="mail.subject" prop="subject" />
      </el-form-item>
      <el-form-item label="正文" prop="html_body">
        <div class="editor-container">
          <quill-editor :options="editorOption" v-model:value="mail.html_body"></quill-editor>
        </div>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleSend" :loading="sending">发送</el-button>
        <el-button @click="handleSaveDraft">保存草稿</el-button>
        <el-popconfirm title="确认要删除已保存的草稿吗？此操作不可撤销" @confirm="handleRmDraft" v-if="loadedFromDraft">
          <template #reference>
            <el-button>删除草稿</el-button>
          </template>
        </el-popconfirm>
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup>
// eslint-disable-next-line no-unused-vars
import Quill from "quill";
import { quillEditor } from "vue3-quill";
import { showSuccessPrompt, showErrorPrompt } from "@/utils/MyPrompt";
import ReceiverList from "@/components/ReceiverList.vue";
import { sendMail } from "@/api/mail";

const router = useRouter();

const formRef = ref(null);
const formRules = {
  to: [{ type: "array", required: true, message: "未填写收件人" }],
  subject: [{ required: true, message: "请输入主题" }],
  html_body: [{ required: true, message: "请输入正文" }],
};
const editorOption = ref({
  placeholder: "在这里撰写邮件正文",
  modules: {
    toolbar: [
      ["bold", "italic", "underline", "strike"],
      ["blockquote"],
      [{ header: 1 }, { header: 2 }],
      [{ list: "ordered" }, { list: "bullet" }],
      [{ script: "sub" }, { script: "super" }],
      [{ header: [1, 2, 3, 4, 5, 6, false] }],
      [{ color: [] }, { background: [] }],
      ["clean"],
      ["link", "image", "video"],
    ],
  },
  scrollingContainer: ".ql-container",
});

const mail = ref({
  to: [], // 收件人
  cc: [], // 抄送
  bcc: [], // 密送
  subject: "", // 主题
  html_body: "", // html正文
});

const loadedFromDraft = ref(false);
const sending = ref(false);

const handleSaveDraft = () => {
  localStorage.setItem("draft", JSON.stringify(mail.value));
  showSuccessPrompt("草稿保存成功");
  loadedFromDraft.value = true;
};

const handleRmDraft = () => {
  localStorage.removeItem("draft");
  showSuccessPrompt("草稿删除成功");
  loadedFromDraft.value = false;
};

const handleSend = () => {
  formRef.value.validate(valid => {
    if (valid) {
      sending.value = true;
      sendMail(mail.value)
        .then(() => {
          showSuccessPrompt("发送成功");
          if (loadedFromDraft.value) {
            localStorage.removeItem("draft");
          }
          router.push("/home/sent");
        })
        .catch(err => {
          showErrorPrompt("发送失败", err);
        })
        .finally(() => {
          sending.value = false;
        });
    }
  });
};

onMounted(() => {
  let draft = localStorage.getItem("draft");
  if (draft) {
    loadedFromDraft.value = true;
    mail.value = JSON.parse(draft);
    showSuccessPrompt("已加载草稿");
  }
});
</script>

<style lang="less" scoped>
.container {
  padding: 20px;
  height: auto;

  :deep(.editor-container) {
    width: 100%;
    min-height: 400px;
    display: flex;
    flex-direction: column;

    .ql-container {
      flex: 1;
    }
  }
}
</style>
