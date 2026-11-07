<template>
    <el-form-item label="默认值">
        <el-select v-model="model.DefaultValue" key="df" collapse-tags placeholder="请选择" v-if="model.Type=='Radio'||model.Type=='CheckBox'||model.Type=='Select'"
                   :multiple="model.Type=='CheckBox'||model.MultiSelect"
                   filterable clearable>
            <el-option :key="index" :label="item.name" :value="item.value" v-for="(item,index) in designer.getItemBindData(model)"> </el-option>

        </el-select>


        <el-switch :active-value="1"
                   :inactive-value="0" v-model="model.DefaultValue" active-text="1" inactive-text="0" v-else-if="model.Type=='Switch'"> </el-switch>


        <el-input v-model="model.DefaultValue" v-else clearable></el-input>

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

