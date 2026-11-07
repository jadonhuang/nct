<template>
    <el-form-item :label="dsTypeName">

        <template v-if="model.DataSourceType==1">

            <el-link style="margin-right:20px" type="primary" :underline="false" icon="el-icon-circle-plus-outline" @click="designer.showDicCreator()">新增</el-link>

            <el-link type="primary" :disabled="!model.DataSourceCode" :underline="false" icon="el-icon-edit" @click="designer.showDicCreator(model.DataSourceCode)">编辑</el-link>


            <el-cascader placeholder="请选择" v-model="model.DataSourceCode" :props="{'emitPath':false,'label':'Name','value':'Code','children':'Children','expandTrigger':'click'}"
                         :options="designer.dicList"
                         filterable clearable></el-cascader>

        </template>
        <template v-else-if="model.DataSourceType==2">

            <el-link style="margin-right: 20px" type="primary" :underline="false" icon="el-icon-circle-plus-outline" @click="designer.showDsCreator()">新增</el-link>

            <el-link type="primary" :disabled="!model.DataSourceCode" :underline="false" icon="el-icon-edit" @click="designer.showDsCreator(model.DataSourceCode)">编辑</el-link>


            <el-select v-model="model.DataSourceCode" placeholder="请选择" filterable>
                <el-option v-for="item in designer.dataSources"
                           :key="item.Code"
                           :label="item.Name"
                           :value="item.Code">
                </el-option>
            </el-select>


        </template>


        <el-select v-else v-model="model.DataSourceCode" placeholder="请选择" filterable>
            <el-option v-for="item in designer.formDefs"
                       :key="item.Code"
                       :label="item.Name"
                       :value="item.Code">
            </el-option>
        </el-select>

    </el-form-item>

</template>

<script>
    export default {
        props: {
            designer: Object,
            model: Object

        },
        computed: {
            dsTypeName() {
                let dsType = this.model.DataSourceType;
                if (dsType <= 2) return '数据选项';
                return '动态表单';

            }
        },
        methods: {

        }


    }</script>

