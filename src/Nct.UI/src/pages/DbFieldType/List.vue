<template>
    <section class="nct-work-area">

        <div class="nct-cmd-area">



            <div class="nct-toolbar">

                <el-button icon="el-icon-plus" type="primary" size="small" @click="showForm('createForm')" v-if="auth.adddbfieldtype"> 添加</el-button>
                <div class="right-tool">
                    <el-button icon="el-icon-refresh" size="small" circle @click="refreshChange"></el-button>


                </div>
            </div>
        </div>


        <el-table :data="data" border element-loading-text="拼命加载中" size="small" v-loading="loading" height="200" class="nct-list-table"
                  row-key="Id">

            <el-table-column width="55" type="index" label="序号">
                <template slot-scope="scope">
                    <label>{{getRowIndex(scope.$index)}}</label>
                </template>
            </el-table-column>
            <el-table-column prop="Descpt" label="名称" min-width="150">
            </el-table-column>
            <el-table-column prop="Name" label="自定义类型" min-width="120">
            </el-table-column>

            <el-table-column prop="DataType" label="Db数据类型" min-width="120">
                <template slot-scope="scope">

                    {{formateType(scope.row)}}

                </template>


            </el-table-column>

            <el-table-column label="操作" width="180" align="center">
                <template slot-scope="scope">

                    <el-link type="primary" :underline="false" icon="el-icon-edit" @click="showForm('editForm',scope.row.Id)" v-if="auth.editdbfieldtype">编辑</el-link>
                    <el-link type="primary" :underline="false" icon='el-icon-delete' @click="delRow(scope.row,scope.$index+1)" v-if="auth.deldbfieldtype">删除</el-link>

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
        name: 'dbfieldtypelist',
        permissions: 'viewdbfieldtype',
        mixins: [crud({ api: window.$dbFieldTypeApi, isShow: true })],
        data() {
            return {


            }
        },

        methods: {

            formateType(fieldType) {
                var lenStr = "";
                if (fieldType.Length > 0) {
                    lenStr += `(${fieldType.Length}`;

                }
                if (fieldType.Precision > 0) {
                    lenStr += `,${fieldType.Precision}`;

                }
                if (lenStr) {
                    lenStr += ")";
                }

                return fieldType.DataType + lenStr;

            },
            processParams(params) {

                var queryParams = { pageIndex: this.page.pageIndex, pageSize: this.page.pageSize, Query: params }
                queryParams.OrderBy = "Id asc";
                return queryParams;

            }

        },
        mounted() {
            this.initPageAuth('dbfieldtype');
            this.getList();
        },
        components: { CreateForm, EditForm }


    }


</script>

