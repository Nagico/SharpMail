<template>
  <div class="operations">
    <el-button @click="handleManuallyFetchMail" type="primary" :loading="fetching">
      <el-icon class="el-icon--left" v-if="!fetching"><i-ep-download /></el-icon>收取新邮件
    </el-button>
  </div>
  <MailList type="sent" ref="listRef" />
</template>

<script setup>
import MailList from "@/components/MailList.vue";
import { showSuccessPrompt, showErrorPrompt } from "@/utils/MyPrompt";

const listRef = ref(null);
const fetching = ref(false);

const handleManuallyFetchMail = () => {
  fetching.value = true;
  listRef.value
    .handleFetchMail()
    .then(newCount => {
      if (newCount > 0) {
        showSuccessPrompt(`成功从邮件服务器收取了${newCount}条新邮件`);
        listRef.value.loadList(1);
      } else {
        showSuccessPrompt("收取成功，没有新邮件");
      }
    })
    .catch(err => {
      showErrorPrompt("收取新邮件失败", err);
    })
    .finally(() => {
      fetching.value = false;
    });
};
</script>

<style lang="less" scoped>
.operations {
  display: flex;
  align-items: center;
  justify-content: flex-start;
  padding: 12px;
  background-color: var(--el-fill-color-blank);
}
</style>
