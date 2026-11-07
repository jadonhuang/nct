

<template>
  <el-form-item label="折叠选项" v-if="model.MultiSelect">

    <el-switch v-model="model.CollapseTags">
    </el-switch>

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

