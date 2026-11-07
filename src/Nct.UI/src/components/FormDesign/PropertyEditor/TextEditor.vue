<template>


  <el-form-item label="控件文字">
    <el-input type="textarea" v-model="model.Text" :rows="2" v-if="model.Type=='Label'" placeholder="请输入"></el-input>
    <el-input v-model="model.Text" placeholder="请输入" v-else></el-input>
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

