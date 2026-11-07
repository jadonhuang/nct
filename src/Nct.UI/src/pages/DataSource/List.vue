<template>


    <el-main class="nct-work-area">

        <div class="nct-cmd-area">
            <div class="nct-search" v-if="isShow">

                <el-col :span="6"> <label> 名称：</label><el-input v-model="params.Name" size="small" clearable></el-input></el-col>
                <el-col :span="6"> <label> 编码：</label><el-input v-model="params.Code" size="small" clearable></el-input></el-col>

                <el-col :span="4" class="search-btn-area">
                    <el-button icon="el-icon-search" type="primary" @click="getList" size="small">查询</el-button>
                    <el-button icon="el-icon-delete" type="primary" plain @click="reset" size="small">清空</el-button>

                </el-col>

            </div>


            <div class="nct-toolbar">

                <el-button icon="el-icon-plus" type="primary" size="small" @click="showForm('createForm')" v-if="auth.adddatasource"> 添加</el-button>

                <div class="right-tool">
                    <el-button icon="el-icon-refresh" size="small" circle @click="refreshChange"></el-button>
                    <el-button icon="el-icon-search" size="small" circle @click="isShow=!isShow"></el-button>

                </div>
            </div>
        </div>

        <el-table :data="data" border element-loading-text="拼命加载中" size="small" v-loading="loading" height="200" class="nct-list-table">

            <el-table-column width="55" type="index" label="序号">
                <template slot-scope="scope">
                    <label>{{getRowIndex(scope.$index)}}</label>
                </template>
            </el-table-column>
            <el-table-column prop="Name" label="名称" min-width="120">
            </el-table-column>
            <el-table-column prop="Code" label="编码" min-width="120">
            </el-table-column>


            <el-table-column prop="Type" label="类型" min-width="60" align="center">
                <template slot-scope="scope">
                    <el-tag>{{scope.row.Type==1?'Sql':'C#'}}</el-tag>
                </template>
            </el-table-column>
            <el-table-column prop="CreateTime" label="创建时间" min-width="100">
            </el-table-column>

            <el-table-column prop="Descpt" label="描述" min-width="150">
            </el-table-column>

            <el-table-column label="操作" width="180" align="center">
                <template slot-scope="scope">

                    <el-link type="primary" :underline="false" icon="el-icon-edit" @click="showForm('editForm',scope.row.Id)" v-if="auth.editdatasource">编辑</el-link>
                    <el-link type="primary" :underline="false" icon='el-icon-delete' @click="delRow(scope.row,scope.$index+1)" v-if="auth.deldatasource">删除</el-link>

                </template>
            </el-table-column>
        </el-table>

        <el-pagination @size-change="sizeChange"
                       @current-change="currentChange"
                       :current-page="page.pageIndex"
                       :page-sizes="[10, 20, 30, 40,50]"
                       :page-size="page.pageSize"
                       layout="total, sizes, prev, pager, next, jumper"
                       :total="page.total">
        </el-pagination>
        <CreateForm ref="createForm" @refresh="searchChange"> </CreateForm>
        <EditForm ref="editForm" @refresh="refreshChange"> </EditForm>

    </el-main>









</template>
<script>

   
    import CreateForm from './Create.vue';
    import EditForm from './Edit.vue';
    import crud from '@/mixins/crud.js'

    export default {
        name: 'datasourcelist',
        permissions: 'viewdatasource',
        mixins: [crud({ api: window.$dataSourceApi, isShow: true })],
        data() {
            return {

                config: {


                }

            }
        },

        methods: {

            reset() {
                this.$emptyObject(this.params);
                this.params.Cate = 0;
                this.getList();
            },
            dicChange(dic) {
                this.params.Cate = dic.Id;
                this.getList();

            },



        },
        mounted() {
            this.initPageAuth('datasource');
            this.getList();
        },
        components: { CreateForm, EditForm }


    }


</script>

