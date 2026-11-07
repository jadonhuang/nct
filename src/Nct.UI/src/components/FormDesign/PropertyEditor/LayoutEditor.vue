<template>
  <el-form-item label="布局方式">
    <el-radio-group v-model="model.Layout" size="mini">
      <el-radio-button :label="1">表格</el-radio-button>
      <el-radio-button :label="2">流布局</el-radio-button>

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

