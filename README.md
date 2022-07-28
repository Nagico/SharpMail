<div align="center">

# SharpMail

<!-- markdownlint-disable-next-line MD036 -->
_✨ 基于 C# 和 Vue 的邮件管理平台 ✨_

_✨ Author: [LSX-s-Software](https://github.com/LSX-s-Software) | [NagisaCo](https://github.com/NagisaCo/) ✨_
</div>

<p align="center">
  <a href="license">
    <img src="https://img.shields.io/badge/LICENSE-GPLv3-red" alt="license">
  </a>
  <a href="stargazers">
    <img src="https://img.shields.io/github/stars/Nagico/SharpMail?color=yellow&label=Github%20Stars" alt="star">
  </a>
  <br/>
  <img src="https://img.shields.io/badge/ASP.Net%20Core-6.0-512BD4" alt="aspnetcore">
  <img src="https://img.shields.io/badge/Vue-3.0-41B784" alt="vue">
  <img src="https://img.shields.io/badge/Electron-19.0-2F3241" alt="electron">
</p>
<!-- markdownlint-enable MD033 -->

## 编译教程

### 后端编译

#### 使用 JetBrains Rider

使用 JetBrains Rider 打开项目 `SharpMail.sln`，使用 `Publish SharpMail to folder.run.xml` 配置进行编译发布。

编译成功后会在 `/SharpMail/server/` 文件夹下生成

- `SharpMailBackend.exe` 程序
- `appsettings.json` 配置文件。

#### 使用 Visual Studio

使用 Visual Studio 打开项目 `SharpMail.sln`，发布 `SharpMailBackend` 到 `../SharpMail/server/` 文件夹。

编译成功后会在 `/SharpMail/server/` 文件夹下生成

- `SharpMailBackend.exe` 程序
- `appsettings.json` 配置文件。

### 前端编译及项目打包

请先处理项目文件换行符为 **LF(\n)**。

在 `/SharpMail/` 文件夹下执行

```shell
pnpm install
pnpm electron:build
```

打包成功后会在 `/SharpMail/dist` 文件夹下生成

- win-unpacked 可执行程序目录
- sharp_mail Setup 1.0.0.exe 安装程序

可任选方式进行软件的运行。

## 项目结构

```
SharpMail
├─ .run  后端发布配置
├─ LICENSE
├─ README.md
├─ SharpMail  前端项目
├─ SharpMail.sln  项目文件
├─ SharpMailBackend  后端项目
└─ SharpMailBackend.Net  后端网络访问项目
       ├─ BaseClient.cs  基础网络访问类
       ├─ Pop3Client.cs  POP3协议
       ├─ SharpMailBackend.Net.csproj
       ├─ SharpMailNetException.cs  异常类
       └─ SmtpClient.cs  SMTP协议
```
