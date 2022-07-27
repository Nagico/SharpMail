// Modules to control application life and create native browser window
const { app, BrowserWindow } = require("electron");
const path = require("path");

function createWindow() {
  // Create the browser window.
  const mainWindow = new BrowserWindow({
    width: 1200,
    height: 1000,
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
  createWindow();

  app.on("activate", function () {
    // On macOS it's common to re-create a window in the app when the
    // dock icon is clicked and there are no other windows open.
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
  });
});

app.on("ready", function () {
  // Run server
  startServer();
});

// Quit when all windows are closed, except on macOS. There, it's common
// for applications and their menu bar to stay active until the user quits
// explicitly with Cmd + Q.
app.on("window-all-closed", function () {
  if (process.platform !== "darwin") app.quit();
  stopServer();
});

// In this file you can include the rest of your app's specific main process
// code. You can also put them in separate files and require them here.

// run backend server
let serverProcess = null;
function startServer() {
  let serverPath = process.env.NODE_ENV === "electron_dev" ? "server" : path.join("resources", "server");

  // 在启动后台服务前闲检测关闭一遍后台服务，防止开启多个后台服务
  stopServer();
  serverProcess = require("child_process").execFile(
    "SharpMailBackend.exe",
    ["--urls=https://localhost:7264/"],
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
  if (serverProcess) {
    console.log("kill server process , serverProcess.pid-->", serverProcess.pid);

    serverProcess.kill();
  }
}
