

<template>
  <el-form-item label="是否只读">

    <el-switch v-model="model.Readonly">
    </el-switch>

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
