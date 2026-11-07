

<template>
  <el-form-item label="控件宽度">
    <el-select v-model="model.Span" placeholder="请选择">
      <el-option v-for="item in spanOpts"
                 :key="item.value"
                 :label="item.label"
                 :value="item.value">
      </el-option>
    </el-select>
  </el-form-item>

</template>

<script>
  import consts from '@/utils/consts'
  export default {
    props: {
      desiner: Object,
      model: Object

    },
    data() {
      return {
        spanOpts: consts.form.spanOpts

      }
    },
    computed: {
   

    },
    methods: {

    }


  }

</script>

<style scoped>

</style>
