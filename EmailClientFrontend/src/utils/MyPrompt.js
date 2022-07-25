import { ElMessage, ElMessageBox } from "element-plus";

/**
 * 显示成功提示
 * @param {String} message 提示信息
 */
export function showSuccessPrompt(message) {
  ElMessage({
    message: message,
    type: "success",
  });
}

/**
 * 显示错误提示
 * @param {String} title 标题
 * @param {String} message 内容
 */
export function showErrorPrompt(title, message) {
  ElMessageBox.alert(message, title, {
    confirmButtonText: "确定",
    type: "error",
  });
}
