<template>
  <div class="operations">
    <el-button @click="handleManuallyFetchMail" type="primary" :loading="fetching">
      <el-icon class="el-icon--left" v-if="!fetching"><i-ep-download /></el-icon>收取新邮件
    </el-button>
    <el-button @click="handleReadAll" plain :disabled="empty">
      <el-icon class="el-icon--left"><i-ep-circleCheck /></el-icon>全部标注已读
    </el-button>
    <el-popconfirm title="确认要删除这些邮件吗？此操作不可撤销" @confirm="handleDelete">
      <template #reference>
        <el-button type="danger" plain :disabled="empty">
          <el-icon class="el-icon--left"><i-ep-delete /></el-icon>删除
        </el-button>
      </template>
    </el-popconfirm>
  </div>
  <MailList type="inbox" ref="listRef" />
</template>

<script setup>
import MailList from "@/components/MailList.vue";
import { showSuccessPrompt, showErrorPrompt } from "@/utils/MyPrompt";
import { setRead } from "@/api/mail";

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

const handleReadAll = () => {
  let tasks = listRef.value.tableData.map(ele => setRead(ele.id));
  Promise.all(tasks)
    .then(() => {
      listRef.value.tableData.forEach(ele => (ele.read = true));
      showSuccessPrompt("全部标注已读成功");
    })
    .catch(err => {
      showErrorPrompt("标注已读失败", err);
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
  background-color: var(--el-fill-color-blank);
}
</style>
