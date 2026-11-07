<template>


    <el-form-item label="验证提示">
        <el-input v-model="model.VerifyInfo" placeholder="验证不通过提示语"></el-input>
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

