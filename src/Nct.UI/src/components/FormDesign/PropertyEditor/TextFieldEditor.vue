<template>
  <el-form-item label="显示字段" v-if="model.DataSourceType>1">

    <el-select v-model="model.TextField" placeholder="请选择" filterable>
     <el-option v-for="item in designer.getItemDsColumns(model)"
                 :key="item.Value"
                 :label="item.Name"
                 :value="item.Value">
      </el-option>
    </el-select>


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

