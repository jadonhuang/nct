<template>
  <el-form-item label="时间格式">

    <el-select v-model="model.DateFormat" placeholder="请选择">
      <el-option :key="1" label="日期" :value="1"> </el-option>
      <el-option :key="2" label="日期和时间" :value="2"> </el-option>
      <!--<el-option :key="3" label="年月" :value="3"> </el-option>
      <el-option :key="4" label="仅年份" :value="4"> </el-option>-->
    </el-select>
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

