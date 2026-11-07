<template>
  <el-form-item label="弹窗宽度">
    <el-input-number v-model="model.DialogWidth" :min="100" :precision="0" controls-position="right"></el-input-number>
  </el-form-item>
</template>

<script>export default {
    props: {
      desiner: Object,
      model: Object

    },
    computed: {
    
    },
    methods: {
    
    }


  }</script>

