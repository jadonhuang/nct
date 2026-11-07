<template>

  <el-form-item label="控件标题"> 
    <el-input v-model="model.Title" placeholder="请输入" ></el-input>
  </el-form-item>



</template>

<script>

  export default {
    props: {
      desiner: Object,
      model: Object


    },
    methods: {

    }


  }



</script>

