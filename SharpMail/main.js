// Modules to control application life and create native browser window
const { app, BrowserWindow, Menu } = require("electron");
const path = require("path");

function createWindow() {
  Menu.setApplicationMenu(null); // null值取消顶部菜单栏
  // Create the browser window.
  const mainWindow = new BrowserWindow({
    width: 1200,
    height: 800,
    webPreferences: {
      preload: path.join(__dirname, "preload.js"),
    },
  });

  // and load the index.html of the app.
  mainWindow.loadFile("dist/index.html");

  // Open the DevTools.
  // mainWindow.webContents.openDevTools()
}

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.whenReady().then(() => {
  // 检测后端服务是否启动，避免端口被占用
  isRunning("SharpMailBackend.exe", status => {
    if (status > 0) {
      console.log("server has already started");
    } else {
      console.log("server is not running");
      startServer();
    }
    createWindow();

    app.on("activate", function () {
      // On macOS it's common to re-create a window in the app when the
      // dock icon is clicked and there are no other windows open.
      if (BrowserWindow.getAllWindows().length === 0) createWindow();
    });
  });
});

// Quit when all windows are closed, except on macOS. There, it's common
// for applications and their menu bar to stay active until the user quits
// explicitly with Cmd + Q.
app.on("window-all-closed", function () {
  if (process.platform !== "darwin") app.quit();
});

app.on("before-quit", () => {
  stopServer();
});

// In this file you can include the rest of your app's specific main process
// code. You can also put them in separate files and require them here.

// run backend server
let serverProcess = null;
function startServer() {
  let serverPath = process.env.NODE_ENV === "electron_dev" ? "server" : path.join("resources", "server");
  serverProcess = require("child_process").execFile(
    "SharpMailBackend.exe",
    [`--urls=https://localhost:7264/`],
    {
      cwd: path.join(process.cwd(), serverPath),
      env: process.env,
    },
    (error, stdout, stderr) => {
      if (error) {
        throw error;
      }
      console.log(stdout);
      console.log(stderr);
    }
  );
  //serverProcess = require("child_process").spawn("./SharpMail.exe --urls=https://localhost:7264/", { cwd: "./server" });
  // 启动成功的输出
  serverProcess.stdout.on("data", function (data) {
    console.log("[server]" + data);
  });
  // 发生错误的输出
  serverProcess.stderr.on("data", function (data) {
    console.log("[server ERROR]" + data);
  });
  // 退出后的输出
  serverProcess.on("close", function (code) {
    console.log("[server EXIT]" + code);
  });
}

function stopServer() {
  isRunning("sharp_mail.exe", status => {
    if (status < 5) {
      // 最后一个前端运行
      if (serverProcess) {
        console.log("kill server process , serverProcess.pid-->", serverProcess.pid);

        serverProcess.kill();
      } else {
        const killProcess = require("kill-process-by-name");
        killProcess("SharpMailBackend.exe");
      }
    }
  });
}

const exec = require("child_process").exec;

const isRunning = (query, cb) => {
  let platform = process.platform;
  let cmd = "";
  switch (platform) {
    case "win32":
      cmd = `tasklist`;
      break;
    case "darwin":
      cmd = `ps -ax | grep ${query}`;
      break;
    case "linux":
      cmd = `ps -A`;
      break;
    default:
      break;
  }
  exec(cmd, (err, stdout) => {
    cb(occurrences(stdout.toLowerCase(), query.toLowerCase(), false));
  });
};

function occurrences(string, subString, allowOverlapping) {
  string += "";
  subString += "";
  if (subString.length <= 0) return string.length + 1;

  let n = 0,
    pos = 0,
    step = allowOverlapping ? 1 : subString.length;

  // eslint-disable-next-line no-constant-condition
  while (true) {
    pos = string.indexOf(subString, pos);
    if (pos >= 0) {
      ++n;
      pos += step;
    } else break;
  }
  return n;
}
