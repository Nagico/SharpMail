<template>
  <div class="operations">
    <el-button @click="handleManuallyFetchMail" type="primary" :loading="fetching">
      <el-icon class="el-icon--left" v-if="!fetching"><i-ep-refresh /></el-icon>同步邮件
    </el-button>
    <el-popconfirm title="确认要删除这些邮件吗？此操作不可撤销" @confirm="handleDelete">
      <template #reference>
        <el-button type="danger" plain :disabled="empty">
          <el-icon class="el-icon--left"><i-ep-delete /></el-icon>删除
        </el-button>
      </template>
    </el-popconfirm>
  </div>
  <MailList type="sent" ref="listRef" />
</template>

<script setup>
import MailList from "@/components/MailList.vue";
import { showSuccessPrompt, showErrorPrompt } from "@/utils/MyPrompt";

const listRef = ref(null);
const fetching = ref(false);
const empty = computed(() => {
  if (listRef.value) {
    return listRef.value.tableData.length == 0;
  } else {
    return true;
  }
});

const handleManuallyFetchMail = () => {
  fetching.value = true;
  listRef.value
    .performFetchMail()
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

const handleDelete = () => {
  listRef.value.performDelete();
};
</script>

<style lang="less" scoped>
.operations {
  display: flex;
  align-items: center;
  justify-content: flex-start;
  padding: 12px;
}
</style>
