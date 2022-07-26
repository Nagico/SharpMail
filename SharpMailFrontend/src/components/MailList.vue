<template>
  <el-table :data="tableData" v-loading="loading">
    <el-table-column type="selection" width="55" />
    <el-table-column prop="from" label="发件人" width="200" v-if="type == 'inbox'" />
    <el-table-column prop="to" label="收件人" width="200" v-else-if="type == 'sent'" />
    <el-table-column prop="subject" label="主题" />
    <el-table-column label="发送时间" width="160">
      <template #default="scope">
        <span>{{ formatTime(scope.row.date) }}</span>
      </template>
    </el-table-column>
    <el-table-column prop="read" label="已读" width="60">
      <template #default="scope">
        <el-icon v-if="scope.row.read"><i-ep-circleCheckFilled /></el-icon>
        <span v-else></span>
      </template>
    </el-table-column>
  </el-table>
  <el-pagination
    layout="sizes, prev, pager, next, jumper"
    :total="total"
    v-model:current-page="currentPage"
    v-model:page-size="pageSize"
    @current-change="handleCurrentChange"
    @size-change="handleSizeChange"
  />
</template>

<script setup>
import * as mailAPI from "@/api/mail.js";
import { showErrorPrompt } from "@/utils/MyPrompt";
import { formatTime } from "@/utils/util";

const props = defineProps({
  type: {
    type: String,
    default: "inbox",
  },
});

const loading = ref(true);
const tableData = ref([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(parseInt(localStorage.getItem("pageSize")) || 30);

const loadList = page => {
  loading.value = true;
  mailAPI
    .getMailList(props.type == "inbox" ? 1 : 2, page, pageSize.value)
    .then(mails => {
      total.value = mails.total;
      tableData.value = mails.data;
      currentPage.value = page;
    })
    .catch(err => {
      showErrorPrompt("获取邮件列表失败", err);
    })
    .finally(() => {
      loading.value = false;
    });
};

const handleFetchMail = () => {
  return new Promise((resolve, reject) => {
    mailAPI
      .fetchMail()
      .then(data => {
        resolve(data.new);
      })
      .catch(err => {
        reject(err);
      });
  });
};

const handleCurrentChange = newPage => {
  loadList(newPage);
};

const handleSizeChange = newSize => {
  loadList(1);
  localStorage.setItem("pageSize", newSize);
};

onMounted(() => {
  loadList();
  handleFetchMail()
    .then(newCount => {
      if (newCount > 0) {
        loadList();
      }
    })
    .catch(err => {
      showErrorPrompt("无法从邮件服务器收取新邮件", err);
    });
});

defineExpose({ tableData, loadList, handleFetchMail });
</script>

<style lang="less">
.el-pagination {
  justify-content: center;
  margin: 8px 0;
}
</style>
