<template>
  <el-form-item label="单选多选">

    <el-radio-group v-model="model.MultiSelect" size="mini">
      <el-radio-button :label="false">单选</el-radio-button>
      <el-radio-button  :label="true">多选</el-radio-button>

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

