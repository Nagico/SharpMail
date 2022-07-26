<template>
  <el-table
    :data="tableData"
    v-loading="loading"
    ref="table"
    row-key="id"
    @row-click="handleRowClick"
    row-class-name="list-item"
  >
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
    v-if="tableData.length > 0"
    layout="total, sizes, prev, pager, next, jumper"
    :total="total"
    v-model:current-page="currentPage"
    v-model:page-size="pageSize"
    @current-change="handleCurrentChange"
    @size-change="handleSizeChange"
  />
  <!-- 阅读对话框 -->
  <el-dialog v-model="dialogVisible" :title="currentMail.subject">
    <MailDetail :mail-item="currentMail" />
  </el-dialog>
</template>

<script setup>
import * as mailAPI from "@/api/mail.js";
import { showSuccessPrompt, showErrorPrompt } from "@/utils/MyPrompt";
import { formatTime } from "@/utils/util";
import MailDetail from "@/components/MailDetail.vue";

const props = defineProps({
  type: {
    type: String,
    default: "inbox",
  },
});

const loading = ref(true);
const table = ref(null);
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

const performFetchMail = () => {
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

const performDelete = () => {
  try {
    table.value.getSelectionRows().forEach(async row => {
      await mailAPI.deleteMail(row.id);
      let index = tableData.value.findIndex(p => p.id == row.id);
      tableData.value.splice(index, 1);
    });
    showSuccessPrompt("删除成功");
  } catch (error) {
    showErrorPrompt("删除失败", error);
  }
};

const dialogVisible = ref(false);
const currentMail = ref({
  subject: "",
});

const handleRowClick = row => {
  // 在这里获取邮件详情，然后设置到currentMail中，再使用下面的dialogVisible.value = true显示对话框
  dialogVisible.value = true;
  currentMail.value = row;
};

onMounted(() => {
  loadList();
  performFetchMail()
    .then(newCount => {
      if (newCount > 0) {
        loadList();
      }
    })
    .catch(err => {
      showErrorPrompt("无法从邮件服务器收取新邮件", err);
    });
});

defineExpose({ tableData, loadList, performFetchMail, performDelete });
</script>

<style lang="less">
.el-pagination {
  justify-content: center;
  margin: 8px 0;
}

.list-item {
  cursor: pointer;
}
</style>
