<template>
  <el-form-item label="工具栏">

    <el-radio-group v-model="model.Toolbar" size="mini">
      <el-radio-button label="default">默认</el-radio-button>
      <el-radio-button label="simple">简单</el-radio-button>
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

