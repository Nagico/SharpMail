<template>
  <el-form label-width="5em" ref="formRef">
    <el-form-item label="发件人" prop="from">
      <div class="tag-container">
        <el-tag v-for="tag in handleAddressList(props.mailItem.from)" :key="tag">
          {{ tag }}
        </el-tag>
      </div>
    </el-form-item>
    <el-form-item label="收件人" prop="to">
      <div class="tag-container">
        <el-tag v-for="tag in handleAddressList(props.mailItem.to)" :key="tag">
          {{ tag }}
        </el-tag>
      </div>
    </el-form-item>
    <el-form-item :label="'抄\u3000送'" prop="cc" v-if="props.mailItem.cc.length > 0">
      <div class="tag-container">
        <el-tag v-for="tag in handleAddressList(props.mailItem.cc)" :key="tag">
          {{ tag }}
        </el-tag>
      </div>
    </el-form-item>
    <el-form-item :label="'密\u3000送'" prop="bcc" v-if="props.mailItem.bcc.length > 0">
      <div class="tag-container">
        <el-tag v-for="tag in handleAddressList(props.mailItem.bcc)" :key="tag">
          {{ tag }}
        </el-tag>
      </div>
    </el-form-item>
    <el-form-item :label="'日\u3000期'" prop="date">
      <span>{{ formatTime(props.mailItem.date) }}</span>
    </el-form-item>
  </el-form>
  <el-divider />
  <div class="mail-detail-content">
    <div v-html="replaceImg(props.mailItem.content)"></div>
  </div>
  <el-divider />
</template>

<script setup>
import { formatTime } from "@/utils/util";

const props = defineProps({
  mailItem: Object,
});

const handleAddress = address => {
  return `${address.name} <${address.address}>`;
};

const handleAddressList = addressList => {
  return addressList.map(x => handleAddress(x));
};

const replaceImg = detailText => {
  detailText = detailText.replace(/<img[^>]*>/gi, function (match) {
    return match.replace(/(style="(.*?)")|(width="(.*?)")|(height="(.*?)")/gi, 'style="max-width:100%;height:auto;"'); // 替换style
  });
  return detailText;
};
</script>

<style lang="less" scoped>
.tag-container {
  display: flex;
  flex-wrap: wrap;
  gap: 5px;
}
.mail-detail-content {
  padding: 10px 0;
  line-height: 1.5;
  font-size: 14px;
  word-break: break-all;
  word-wrap: break-word;
  overflow-wrap: break-word;
  overflow: auto;
}
</style>
