<template>
    <section class="nct-work-area">

        <div class="nct-cmd-area">
            <div class="nct-search" v-if="isShow">
                <el-col :span="6"> <label> 角色编号：</label><el-input v-model="params.Code" size="small" clearable></el-input></el-col>
                <el-col :span="6"> <label> 角色名称：</label><el-input v-model="params.Name" size="small" clearable></el-input></el-col>

                <el-col :span="4" class="search-btn-area">
                    <el-button icon="el-icon-search" type="primary" @click="getList" size="small">查询</el-button>
                    <el-button icon="el-icon-delete" type="primary" plain @click="refreshChange(true)" size="small">清空</el-button>
                </el-col>

            </div>


            <div class="nct-toolbar">

                <el-button icon="el-icon-plus" type="primary" size="small" @click="showForm('createForm')" v-if="auth.addrole"> 添加角色</el-button>
                <el-button icon="el-icon-delete" size="small" type="danger" plain @click="multiDelRow" v-if="auth.delrole">删除角色</el-button>

                <div class="right-tool">
                    <el-button icon="el-icon-refresh" size="small" circle @click="refreshChange"></el-button>
                    <el-button icon="el-icon-search" size="small" circle @click="isShow=!isShow"></el-button>

                </div>

            </div>
        </div>
        <el-table :data="data" border element-loading-text="拼命加载中" size="small" v-loading="loading" height="200" class="nct-list-table"
                  @selection-change="selectionChange">
            <el-table-column type="selection" align="center"
                             width="50">
            </el-table-column>
            <el-table-column width="55" type="index" label="序号">
                <template slot-scope="scope">
                    <label>{{getRowIndex(scope.$index)}}</label>
                </template>
            </el-table-column>
            <el-table-column prop="Code" label="角色编号" min-width="160">
            </el-table-column>
            <el-table-column prop="Name" label="角色名称" min-width="160">
            </el-table-column>
            <el-table-column prop="CreateTime" label="创建时间" min-width="190">
            </el-table-column>

            <el-table-column prop="Descpt" label="角色描述" min-width="200">
            </el-table-column>


            <el-table-column label="操作" width="150" align="center">
                <template slot-scope="scope">

                    <el-link type="primary" :underline="false" icon="el-icon-edit" @click="showForm('editForm',scope.row.Id)" v-if="auth.editrole">编辑</el-link>
                    <el-link type="primary" :underline="false" icon='el-icon-delete' @click="delRow(scope.row,scope.$index+1)" v-if="auth.delrole">删除</el-link>

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



    </section>



</template>
<script>



    import CreateForm from './Create.vue';
    import EditForm from './Edit.vue';

    import crud from '@/mixins/crud.js'

    export default {
        name: 'rolelist',
        permissions: 'viewrole',
        mixins: [crud({ api: window.$roleApi, isShow: true })],
        data() {
            return {

            }
        },

        methods: {


        },
        mounted() {

            this.initPageAuth('role');
            this.getList();

        },
        components: { CreateForm, EditForm }


    }


</script>

