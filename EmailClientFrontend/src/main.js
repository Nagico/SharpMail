import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";

// element+
import "element-plus/theme-chalk/dark/css-vars.css";
import "element-plus/theme-chalk/el-message.css";
import "element-plus/theme-chalk/el-message-box.css";

const app = createApp(App);
app.use(router);

app.mount("#app");
