<template>
  <el-form-item label="文件个数">
    <el-input-number v-model="model.Limit" :min="1" :precision="0" controls-position="right"></el-input-number>
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

