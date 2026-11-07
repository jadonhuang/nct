<template>
  <el-form-item label="最少长度">
    <el-input-number v-model="model.MinLen" :min="0" :precision="0" controls-position="right"></el-input-number>
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

