<template>
  <el-form-item label="数据来源" v-if="model.DataSourceType<=2">
    <el-radio-group v-model="model.DataSourceType" size="mini">
      <el-radio-button :label="1">数据字典</el-radio-button>
      <el-radio-button :label="2">数据源</el-radio-button>

    </el-radio-group>
  </el-form-item>


</template>

<script>export default {
    props: {
      designer: Object,
      model: Object

    },
    computed: {
    
    },
    methods: {
        
    }


  }</script>

