<template>
  <el-form-item label="数据精度">
    <el-input-number v-model="model.Precision" :min="0" :max="6" controls-position="right"></el-input-number>
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

