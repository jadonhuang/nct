<template>
  <el-form-item label="行数">
    <el-input-number v-model="model.Rows" :min="1" controls-position="right"></el-input-number>
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

