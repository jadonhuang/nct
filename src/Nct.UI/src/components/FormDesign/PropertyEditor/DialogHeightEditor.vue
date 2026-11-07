<template>
  <el-form-item label="弹窗高度">
    <el-input-number v-model="model.DialogHeight" :min="100" :precision="0" controls-position="right"></el-input-number>
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

