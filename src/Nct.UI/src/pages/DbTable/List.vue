<template>
    <section class="nct-work-area">

        <div class="nct-cmd-area">
            <div class="nct-search" v-if="isShow">

                <el-col :span="6"> <label> 名称：</label><el-input v-model="params.Name" size="small" clearable></el-input></el-col>
                <el-col :span="6"> <label> 描述：</label><el-input v-model="params.Descpt" size="small" clearable></el-input></el-col>

                <el-col :span="4" class="search-btn-area">
                    <el-button icon="el-icon-search" type="primary" @click="getList" size="small">查询</el-button>
                    <el-button icon="el-icon-delete" type="primary" plain @click="refreshChange(true)" size="small">清空</el-button>
                </el-col>

            </div>


            <div class="nct-toolbar">

                <el-button icon="el-icon-plus" type="primary" size="small" @click="showForm('createForm')" v-if="auth.adddbtable"> 添加</el-button>
                <el-button icon="el-icon-upload2" size="small" @click="showForm('importForm')" v-if="auth.adddbtable">导入现有表</el-button>
                <div class="right-tool">
                    <el-button icon="el-icon-refresh" size="small" circle @click="refreshChange"></el-button>
                    <el-button icon="el-icon-search" size="small" circle @click="isShow=!isShow"></el-button>

                </div>
            </div>
        </div>

        <el-table :data="data" border element-loading-text="拼命加载中" ref="tb" size="small" height="100%" v-loading="loading" class="nct-list-table" @expand-change="expandChange"
                  row-key="Id">

            <el-table-column width="52" type="index" label="序号">
                <template slot-scope="scope">
                    <label>{{getRowIndex(scope.$index)}}</label>
                </template>
            </el-table-column>

            <el-table-column type="expand" class-name="expand-cell">
                <template slot-scope="props">

                    <el-table :data="fields[props.row.Name]" border element-loading-text="加载中" size="small">

                        <el-table-column width="55" type="index" label="序号">
                            <template slot-scope="scope">
                                <label>{{scope.$index+1}}</label>
                            </template>
                        </el-table-column>

                        <el-table-column prop="Name" label="字段名称" min-width="100">
                        </el-table-column>

                        <el-table-column prop="FieldType" label="数据类型" min-width="100">
                        </el-table-column>

                        <el-table-column prop="FieldTypeName" label="字段类型" min-width="100">
                        </el-table-column>

                        <el-table-column prop="Length" label="字段长度" min-width="100">
                        </el-table-column>

                        <el-table-column prop="Remark" label="说明" min-width="100">
                        </el-table-column>

                    </el-table>

                </template>
            </el-table-column>

            <el-table-column prop="Name" label="数据表名称" min-width="180">
            </el-table-column>

            <el-table-column prop="Descpt" label="数据表描述" min-width="150">
            </el-table-column>


            <el-table-column prop="CreateTime" label="创建时间" min-width="100">
            </el-table-column>


            <el-table-column label="操作" width="140" align="center">
                <template slot-scope="scope">

                    <el-link type="primary" :underline="false" icon="el-icon-edit" @click="showForm('editForm',scope.row.Id)" v-if="auth.editdbtable">编辑</el-link>

                </template>
            </el-table-column>
        </el-table>
        <el-pagination @size-change="refreshChange"
                       @current-change="currentChange"
                       :current-page="page.pageIndex"
                       :page-sizes="[10, 20, 30, 40,50]"
                       :page-size="page.pageSize"
                       layout="total, sizes, prev, pager, next, jumper"
                       :total="page.total">
        </el-pagination>



        <CreateForm ref="createForm" @refresh="searchChange"> </CreateForm>
        <EditForm ref="editForm" @refresh="refreshChange"> </EditForm>
        <ImportForm ref="importForm" @refresh="refreshChange"> </ImportForm>
    </section>


</template>
<script>


    import CreateForm from './Create.vue';
    import EditForm from './Edit.vue';
    import ImportForm from './Import.vue';
    import crud from '@/mixins/crud.js'



    export default {
        name: 'dbtablelist',
        permissions: 'viewdbtable',
        mixins: [crud({ api: window.$dbTableApi, isShow: true })],
        data() {
            return {
                fields: {}

            }
        },

        methods: {

            expandChange(row, rows) {

                if (rows.findIndex(m => m.Id == row.Id) != -1)
                    this.getFileds(row.Name);
            },
            getFileds(table) {

                $dbTableApi.getCustomFields(table).then(res => {
                    Vue.set(this.fields, table, res.Data);
                    this.$nextTick(function () {
                        this.$refs.tb.doLayout();

                    })


                });


            },


        },
        mounted() {
            this.initPageAuth('dbtable');
            this.getList();
        },
        components: { CreateForm, EditForm, ImportForm }


    }


</script>

