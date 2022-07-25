import axios from "axios";

export const http = axios.create({
  baseURL: "https://localhost:7264/",
  withCredentials: true,
});

http.interceptors.request.use(config => {
  if (config.url == "/tokens/login" || config.url == "/tokens/register") {
    // 注册和登录不需要携带token
    return config;
  }
  // 获取token
  const token = localStorage.getItem("token");
  if (token != null) {
    config.headers["Authorization"] = `Bearer ${token}`;
  }
  return config;
});

http.interceptors.response.use(
  response => {
    return response;
  },
  error => {
    if (error.response) {
      console.warn(error);
      switch (error.response.status) {
        case 401:
          location.href = "/";
          break;
        default:
          break;
      }
    }
    return Promise.reject(error);
  }
);
