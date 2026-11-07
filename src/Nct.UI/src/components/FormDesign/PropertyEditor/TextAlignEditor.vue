<template>

  <el-form-item :label="model.Type=='Label'? '文字对齐':'对齐方式'">

      <el-radio-group v-model="model.TextAlign" class="text-align" size="mini">
          <el-radio-button label="left">左对齐</el-radio-button>
          <el-radio-button label="center">居中对齐</el-radio-button>
          <el-radio-button label="right">右对齐</el-radio-button>
      </el-radio-group>

  </el-form-item>
</template>

<script>
  export default {
    props: {
      desiner: Object,
      model: Object

    },
    computed: {

    },
    methods: {

    }


  }</script>

<style scoped>

  /deep/ .text-align .el-radio-button__inner {

    padding-left:6px;
    padding-right:6px;

  }
</style>

