/**
 * 格式化时间字符串
 * @param {String} time 可转化为Date的字符串
 * @param {String} fmt 格式（默认为yyyy-MM-dd hh:mm:ss）
 */
export function formatTime(time, fmt = "yyyy-MM-dd hh:mm:ss") {
  try {
    if (typeof time == "string") {
      time = new Date(time);
    }
    var o = {
      "M+": time.getMonth() + 1, //月份
      "d+": time.getDate(), //日
      "h+": time.getHours(), //小时
      "m+": time.getMinutes(), //分
      "s+": time.getSeconds(), //秒
      "q+": Math.floor((time.getMonth() + 3) / 3), //季度
      S: time.getMilliseconds(), //毫秒
    };
    if (/(y+)/.test(fmt)) {
      fmt = fmt.replace(RegExp.$1, (time.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
      if (new RegExp("(" + k + ")").test(fmt)) {
        fmt = fmt.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
      }
    }
    return fmt;
  } catch (error) {
    return "";
  }
}

export const formatNumber = n => {
  n = n.toString();
  return n[1] ? n : `0${n}`;
};

/**
 * 计算两个日期的年份差
 * @param {String} start 开始时间
 * @param {String} end 结束时间（默认为现在）
 */
export function yearDiff(start, end = new Date()) {
  let startDate = new Date(Date.parse(end));
  let endDate = new Date(Date.parse(start));
  return startDate.getFullYear() - endDate.getFullYear();
}

/**
 * 计算两个日期的天数差(end - start)
 * @param {String} start 开始时间
 * @param {String} end 结束时间（默认为现在）
 */
export function dateDiff(start, end = new Date()) {
  let startDate = Date.parse(end);
  let endDate = Date.parse(start);
  return (startDate - endDate) / 1000 / 86400;
}

export function validateMailAddr(_, value, callback) {
  if (!value) {
    return callback(new Error("邮箱不能为空"));
  }
  if (/^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,})$/.test(value)) {
    callback();
  } else {
    callback(new Error("邮箱地址格式不正确"));
  }
}
