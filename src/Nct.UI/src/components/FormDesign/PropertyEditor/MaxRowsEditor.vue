<template>
  <el-form-item label="最多条数">
    <el-input-number v-model="model.MaxRows" :min="0" controls-position="right"></el-input-number>
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

