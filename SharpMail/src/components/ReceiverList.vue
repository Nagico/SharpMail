<template>
  <div class="tag-container">
    <el-tag
      v-for="tag in dynamicTags"
      :key="tag"
      closable
      :disable-transitions="false"
      @close="handleClose(tag)"
      :type="type"
    >
      {{ tag }}
    </el-tag>
    <el-input
      v-if="inputVisible"
      ref="InputRef"
      v-model="inputValue"
      size="small"
      @keyup.enter="handleInputConfirm"
      @keyup.esc="handleInputClose"
      @blur="handleInputBlur"
    />
    <el-button v-else class="button-new-tag" size="small" @click="showInput"> + </el-button>
  </div>
</template>

<script setup>
import { showErrorPrompt } from "@/utils/MyPrompt";
import { validateMailAddr } from "@/utils/util";

const props = defineProps(["modelValue", "type"]);
const emit = defineEmits(["update:modelValue"]);

const inputValue = ref("");
const { modelValue: dynamicTags } = toRefs(props);
const inputVisible = ref(false);
const InputRef = ref(null);

const handleClose = tag => {
  dynamicTags.value.splice(dynamicTags.value.indexOf(tag), 1);
};

const showInput = () => {
  inputVisible.value = true;
  nextTick(() => {
    InputRef.value.input.focus();
  });
};

const handleInputClose = () => {
  inputVisible.value = false;
  inputValue.value = "";
};

const handleInputBlur = () => {
  if (inputValue.value) {
    handleInputConfirm();
  } else {
    handleInputClose();
  }
};

const handleInputConfirm = () => {
  validateMailAddr(null, inputValue.value, err => {
    if (!err) {
      dynamicTags.value.push(inputValue.value);
      emit("update:modelValue", dynamicTags.value);
      handleInputClose();
    } else {
      showErrorPrompt("添加失败", err.message);
    }
  });
};
</script>

<style lang="less" scoped>
.tag-container {
  display: flex;
  flex-wrap: wrap;
  gap: 5px;

  :deep(.el-input) {
    width: 250px;
  }
}
</style>
