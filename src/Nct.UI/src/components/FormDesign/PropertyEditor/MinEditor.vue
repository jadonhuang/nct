<template>
  <el-form-item label="最小值">
    <el-input-number v-model="model.Min" :min="0" :precision="model.Precision" controls-position="right"></el-input-number>
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

