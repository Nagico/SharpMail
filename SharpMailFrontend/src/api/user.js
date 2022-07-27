import { http } from "@/api/index";

/**
 * 登录
 * @param {String} email 邮箱地址
 * @param {String} password 密码
 * @returns {Promise} 登录Promise
 */
export function login(email, password) {
  return new Promise((resolve, reject) => {
    http
      .post("/tokens/login", {
        email: email,
        password: password,
      })
      .then(res => {
        if (res.data.token) {
          resolve(res.data);
        } else {
          reject(res.data ? res.data.detail : res.message);
        }
      })
      .catch(err => {
        reject(err.response ? err.response.data.detail : err.message);
      });
  });
}

/**
 * 登出
 */
export function logout() {
  localStorage.removeItem("token");
  localStorage.removeItem("userid");
  localStorage.removeItem("email");
}

/**
 * 修改配置
 * @param {String} smtpHost SMTP服务器地址
 * @param {Number} smtpPort SMTP服务器端口
 * @param {Boolean} smtpSsl SMTP服务器是否使用SSL
 * @param {String} pop3Host POP3服务器地址
 * @param {Number} pop3Port POP3服务器端口
 * @param {Boolean} pop3Ssl POP3服务器是否使用SSL
 * @returns 修改配置Promise
 */
export function config(smtpHost, smtpPort, smtpSsl, pop3Host, pop3Port, pop3Ssl) {
  return new Promise((resolve, reject) => {
    http
      .put("/accounts", {
        smtp_host: smtpHost,
        smtp_port: smtpPort,
        smtp_ssl: smtpSsl,
        pop3_host: pop3Host,
        pop3_port: pop3Port,
        pop3_ssl: pop3Ssl,
      })
      .then(res => {
        if (res.data.is_connected) {
          resolve();
        } else {
          reject("无法连接至SMTP或POP3邮件服务器，请检查配置信息是否正确");
        }
      })
      .catch(err => {
        reject(err.response ? err.response.data.detail : err.message);
      });
  });
}

/**
 * 获取配置信息
 * @returns {Promise} 获取配置Promise
 */
export function getConfig() {
  return new Promise((resolve, reject) => {
    http
      .get("/accounts")
      .then(res => {
        resolve(res.data);
      })
      .catch(err => {
        reject(err.response ? err.response.data.detail : err.message);
      });
  });
}
