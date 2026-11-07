<template>
    <section class="nct-work-area">

        <div class="nct-cmd-area">
            <div class="nct-search" v-if="isShow">

                <el-col :span="6"> <label> 名称：</label><el-input v-model="params.Name" size="small" clearable></el-input></el-col>
                <el-col :span="6"> <label> 编码：</label><el-input v-model="params.Code" size="small" clearable></el-input></el-col>

                <el-col :span="4" class="search-btn-area">
                    <el-button icon="el-icon-search" type="primary" @click="getList" size="small">查询</el-button>
                    <el-button icon="el-icon-delete" type="primary" plain @click="refreshChange(true)" size="small">清空</el-button>


                </el-col>

            </div>


            <div class="nct-toolbar">

                <el-button icon="el-icon-plus" type="primary" size="small" @click="showCreateForm(0)" v-if="auth.adddic"> 添加顶级</el-button>

                <div class="right-tool">
                    <el-button icon="el-icon-refresh" size="small" circle @click="refreshChange"></el-button>
                    <el-button icon="el-icon-search" size="small" circle @click="isShow=!isShow"></el-button>

                </div>
            </div>
        </div>


        <el-table :data="data" border element-loading-text="拼命加载中" size="small" v-loading="loading" height="200" class="nct-list-table"
                  :tree-props="{children: 'Children'}"
                  row-key="Id">

            <el-table-column width="55" type="index" label="序号">
                <template slot-scope="scope">
                    <label>{{getRowIndex(scope.$index)}}</label>
                </template>
            </el-table-column>
            <el-table-column prop="Name" label="名称" min-width="200">
            </el-table-column>
            <el-table-column prop="Code" label="编码" min-width="100">
            </el-table-column>

            <el-table-column prop="Sort" label="排序" min-width="100">
            </el-table-column>

            <el-table-column prop="Descpt" label="描述" min-width="120">
            </el-table-column>


            <el-table-column label="操作" width="220" align="center">
                <template slot-scope="scope">

                    <el-link type="primary" :underline="false" icon='el-icon-circle-plus-outline' @click="showCreateForm(scope.row.Id)" v-if="auth.adddic">新增子项</el-link>
                    <el-link type="primary" :underline="false" icon="el-icon-edit" @click="showForm('editForm',scope.row.Id)" v-if="auth.editdic">编辑</el-link>
                    <el-link type="primary" :underline="false" icon='el-icon-delete' @click="delRow(scope.row,scope.$index+1)" v-if="auth.deldic">删除</el-link>

                </template>
            </el-table-column>
        </el-table>



        <CreateForm ref="createForm" @refresh="searchChange"> </CreateForm>
        <EditForm ref="editForm" @refresh="refreshChange"> </EditForm>

    </section>


</template>
<script>


    import CreateForm from './Create.vue';
    import EditForm from './Edit.vue';
    import crud from '@/mixins/crud.js'


    export default {
        name: 'diclist',
        permissions: 'viewdic',
        mixins: [crud({ api: window.$dicApi, list:'getTree', isShow: true })],
        data() {
            return {


            }
        },

        methods: {

            processParams(params) {
                return params;
            },
            showCreateForm(pid) {

                this.$refs.createForm.show();
                this.$refs.createForm.setPid(pid);

            },
            processData(data) {
                return data;
            },



        },
        mounted() {
            this.initPageAuth('dic');
            this.getList();
        },
        components: { CreateForm, EditForm }


    }


</script>

