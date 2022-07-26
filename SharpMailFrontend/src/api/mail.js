import { http } from "@/api/index";

/**
 * 获取邮件列表
 * @param {String} type 筛选类型 1-收件 2-发件
 * @param {Number} page 页码
 * @param {Number} pageSize 每页条数
 * @returns 列表结果Promise
 */
export function getMailList(type, page = 1, pageSize = 10) {
  return new Promise((resolve, reject) => {
    http
      .get(`/mails?type=${type}&page=${page}&pageSize=${pageSize}`)
      .then(res => {
        resolve(res.data);
      })
      .catch(err => {
        reject(err.response ? err.response.data.detail : err.message);
      });
  });
}

/**
 * 从服务器获取新邮件
 * @returns 是否有新邮件Promise
 */
export function fetchMail() {
  return new Promise((resolve, reject) => {
    http
      .post("/mails/pull")
      .then(res => {
        resolve(res.data);
      })
      .catch(err => {
        reject(err.response ? err.response.data.detail : err.message);
      });
  });
}

/**
 * 将邮件标记为已读
 * @param {Number} id 邮件id
 * @returns 标记已读Promise
 */
export function setRead(id) {
  return new Promise((resolve, reject) => {
    http
      .post(`/mails/${id}/read`)
      .then(res => {
        resolve(res.data);
      })
      .catch(err => {
        reject(err.response ? err.response.data.detail : err.message);
      });
  });
}
