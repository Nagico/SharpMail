<template>
  <el-container class="container">
    <el-header>
      <h1>SharpMail</h1>
      <div class="user">
        <span>{{ email }}</span>
        <el-button @click="handleLogout" type="primary">退出</el-button>
      </div>
    </el-header>
    <el-container style="height: calc(100% - 60px)">
      <el-aside width="250px">
        <el-menu :default-active="route.path" router class="">
          <el-menu-item index="/home/draft">
            <el-icon><i-ep-editPen /></el-icon>
            <span>写邮件</span>
          </el-menu-item>
          <el-menu-item index="/home/inbox">
            <el-icon><i-ep-download /></el-icon>
            <span>收件箱</span>
            <!-- <span class="badge" v-if="unreadCount > 0">{{ unreadCount }}</span> -->
          </el-menu-item>
          <el-menu-item index="/home/sent">
            <el-icon><i-ep-upload /></el-icon>
            <span>已发送</span>
          </el-menu-item>
          <el-menu-item index="/home/settings">
            <el-icon><i-ep-setting /></el-icon>
            <span>设置</span>
          </el-menu-item>
        </el-menu>
      </el-aside>
      <el-main>
        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup>
import { logout } from "@/api/user";
import { showSuccessPrompt } from "@/utils/MyPrompt";

const router = useRouter();
const route = useRoute();

const email = ref(localStorage.getItem("email") || "您未登录");

// const unreadCount = ref(0);

const handleLogout = () => {
  logout();
  router.replace("/");
  showSuccessPrompt("您已成功退出登录");
};
</script>

<style lang="less" scoped>
:deep(.el-main) {
  --el-main-padding: 0 !important;
  height: 100%;
}

.el-header {
  background-color: var(--el-color-primary);
  display: flex;
  align-items: center;
  justify-content: space-between;
  color: #fff;

  h1 {
    font-weight: normal;
  }

  .user {
    display: flex;
    align-items: center;
    gap: 8px;
  }
}

.el-menu {
  height: 100%;
}

:deep(h2) {
  margin-bottom: 20px;
  text-align: left;
  font-weight: 500;
}

// :deep(.badge) {
//   background-color: var(--el-color-primary);
//   color: #fff;
//   line-height: 16px;
//   border-radius: 8px;
//   padding: 0 5px;
//   font-size: 12px;
//   font-weight: 500;
//   text-align: center;
//   margin-left: 8px;
// }
</style>
