<template>
  <el-form-item label="文件格式" >
    <el-input v-model="model.Accept" placeholder="为空则不限制,如:.jpg,.xls"></el-input>
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

