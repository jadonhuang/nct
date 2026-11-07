<template>
  <el-form-item label="显示样式">
    <el-radio-group v-model="model.RenderType" size="mini">
      <el-radio-button :label="1">默认</el-radio-button>
      <el-radio-button :label="2">树形</el-radio-button>

    </el-radio-group>

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

