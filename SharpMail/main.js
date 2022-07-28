// Modules to control application life and create native browser window
const { app, BrowserWindow, Menu } = require("electron");
const path = require("path");
const { exec, execFile } = require("child_process");

const backendExecutableFile = process.platform === "win32" ? "SharpMailBackend.exe" : "SharpMailBackend";

function createWindow() {
  Menu.setApplicationMenu(null); // null值取消顶部菜单栏
  const mainWindow = new BrowserWindow({
    width: 1200,
    height: 800,
    webPreferences: {
      preload: path.join(__dirname, "preload.js"),
    },
  });
  mainWindow.loadFile("dist/index.html");

  // Open the DevTools.
  // mainWindow.webContents.openDevTools()
}

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.whenReady().then(() => {
  // 检测后端服务是否启动，避免端口被占用
  isRunning(backendExecutableFile, status => {
    if (status > 0) {
      console.log("server has already started");
    } else {
      console.log("server is not running. starting...");
      startServer();
      var start = (new Date()).getTime();
      while ((new Date()).getTime() - start < 2000) {
        continue;
      }
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

// run backend server
let serverProcess = null;
function startServer() {
  let serverPath =
    process.env.NODE_ENV === "electron_dev"
      ? path.join(process.cwd(), "server")
      : path.resolve(process.resourcesPath, "server");
  serverProcess = execFile(
    path.join(serverPath, backendExecutableFile),
    [`--urls=https://localhost:7264/`],
    {
      cwd: serverPath,
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
  isRunning(process.platform == "win32" ? "SharpMail.exe" : "SharpMail", status => {
    if (status < 5) {
      // 最后一个前端运行
      if (serverProcess) {
        console.log("kill server process , serverProcess.pid-->", serverProcess.pid);

        serverProcess.kill();
      } else {
        const killProcess = require("kill-process-by-name");
        killProcess(backendExecutableFile);
      }
    }
  });
}

const isRunning = (query, cb) => {
  let platform = process.platform;
  let cmd = "";
  switch (platform) {
    case "win32":
      cmd = `tasklist`;
      break;
    case "darwin":
    case "linux":
      cmd = `ps -ef | grep ${query} | grep -v "grep"`;
      break;
    default:
      break;
  }
  exec(cmd, (_, stdout) => {
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
