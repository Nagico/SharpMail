import { createRouter, createWebHistory } from "vue-router";
import LoginView from "../views/LoginView.vue";

import { ElMessage } from "element-plus";

const routes = [
  {
    path: "/",
    name: "index",
    component: LoginView,
  },
  {
    path: "/home",
    name: "home",
    meta: {
      needAuth: true,
    },
    component: () => import("../views/HomeView.vue"),
    children: [
      {
        path: "inbox",
        component: () => import("../views/InboxView.vue"),
      },
      {
        path: "draft",
        component: () => import("../views/DraftView.vue"),
      },
      {
        path: "sent",
        component: () => import("../views/SentView.vue"),
      },
      {
        path: "settings",
        component: () => import("../views/SettingView.vue"),
      },
    ],
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach(to => {
  if (to.meta.needAuth) {
    //判断当前路由是否需要进行权限控制
    let token = localStorage.getItem("token");
    let email = localStorage.getItem("email");
    if (!token || !email) {
      router.replace("/");
      ElMessage({
        message: "您未登录",
        type: "error",
      });
      return false;
    }
    return true;
  } else {
    return true;
  }
});

export default router;
