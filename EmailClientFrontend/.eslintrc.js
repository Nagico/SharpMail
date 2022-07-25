module.exports = {
  root: true,
  env: {
    node: true,
  },
  extends: ["plugin:vue/vue3-essential", "eslint:recommended", "plugin:prettier/recommended"],
  rules: {
    "no-console": process.env.NODE_ENV === "production" ? "warn" : "off",
    "no-debugger": process.env.NODE_ENV === "production" ? "warn" : "off",
    "no-case-declarations": "off",
    "max-len": [
      "warn",
      {
        code: 120,
        ignoreComments: true,
        ignoreTrailingComments: true,
        ignoreTemplateLiterals: true,
      },
    ],
  },
  globals: {
    computed: false,
    createApp: false,
    customRef: false,
    defineAsyncComponent: false,
    defineComponent: false,
    effectScope: false,
    getCurrentInstance: false,
    getCurrentScope: false,
    h: false,
    inject: false,
    isReadonly: false,
    isRef: false,
    markRaw: false,
    nextTick: false,
    onActivated: false,
    onBeforeMount: false,
    onBeforeUnmount: false,
    onBeforeUpdate: false,
    onDeactivated: false,
    onErrorCaptured: false,
    onMounted: false,
    onRenderTracked: false,
    onRenderTriggered: false,
    onScopeDispose: false,
    onServerPrefetch: false,
    onUnmounted: false,
    onUpdated: false,
    provide: false,
    reactive: false,
    readonly: false,
    ref: false,
    resolveComponent: false,
    shallowReactive: false,
    shallowReadonly: false,
    shallowRef: false,
    toRaw: false,
    toRef: false,
    toRefs: false,
    triggerRef: false,
    unref: false,
    useAttrs: false,
    useCssModule: false,
    useCssVars: false,
    useRoute: false,
    useRouter: false,
    useSlots: false,
    watch: false,
    watchEffect: false,
  },
};
