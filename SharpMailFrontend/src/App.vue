<template>
  <router-view />
</template>

<script setup>
import { useDark } from "@vueuse/core";

useDark();
</script>

<style lang="less">
* {
  margin: 0;
  padding: 0;
}

:root {
  --theme: #409eff;
  --theme-transparent: #40a0ff33;

  --primary-bg: #f5f5f5;
  --secondary-bg: #cccccc;
  --primary-text: #000;
  --secondary-text: rgb(44, 36, 36);
  --separator-color: #e6e6e680;

  --material: rgba(245, 245, 245, 0.8);
}

@media (prefers-color-scheme: dark) {
  :root {
    --theme: #257ed6;
    --theme-transparent: #40a0ff33;

    --primary-bg: #1f1f1f;
    --secondary-bg: #2f2f2f;
    --primary-text: rgb(237, 237, 237);
    --secondary-text: #dadadad0;
    --separator-color: #60606080;

    --material: rgba(39, 39, 39, 0.8);
  }
}

#app {
  font-family: Arial, Helvetica, sans-serif;
  text-align: center;
  color: var(--primary-text);
  width: 100vw;
  height: 100vh;
  overflow: hidden;

  .dark & {
    color: #ecf0f1;
    background-color: #141414;
  }
}

.container {
  width: 100%;
  height: 100%;
  position: relative;
  box-sizing: border-box;
}
</style>

<style>
.v-enter-active,
.v-leave-active {
  transition: opacity 0.5s ease;
}

.v-enter-from,
.v-leave-to {
  opacity: 0;
}
</style>
