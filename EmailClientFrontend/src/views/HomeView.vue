<template>
  <el-container class="container">
    <el-header>
      <h1>SharpMail</h1>
      <div class="user">
        <span>{{ email }}</span>
        <el-button @click="handleLogout">退出</el-button>
      </div>
    </el-header>
    <el-container>
      <el-aside width="250px">
        <el-menu default-active="/home/inbox" router class="">
          <el-menu-item index="/home/draft">
            <el-icon><i-ep-editPen /></el-icon>
            <span>写邮件</span>
          </el-menu-item>
          <el-menu-item index="/home/inbox">
            <el-icon><i-ep-download /></el-icon>
            <span>收件箱</span>
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

const email = ref(localStorage.getItem("email") || "您未登录");

const handleLogout = () => {
  logout();
  router.replace("/");
  showSuccessPrompt("您已成功退出登录");
};
</script>

<style lang="less" scoped>
.el-header {
  background-color: var(--el-color-primary);
  display: flex;
  align-items: center;
  justify-content: space-between;

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
</style>
