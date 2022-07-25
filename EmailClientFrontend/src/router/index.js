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
    name: "mail",
    meta: {
      needAuth: true,
    },
    component: () => import("../views/HomeView.vue"),
    children: [
      {
        path: "inbox",
        component: import("../views/InboxView.vue"),
      },
      {
        path: "draft",
        component: import("../views/DraftView.vue"),
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
    if (token == null || token == "") {
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
