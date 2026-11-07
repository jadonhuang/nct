<template>
    <section class="nct-work-area">

        <div class="nct-cmd-area">
            <div class="nct-search" v-if="isShow">
                <el-col :span="6"> <label> 表单名称：</label><el-input v-model="params.Name" size="small" clearable></el-input></el-col>
                <el-col :span="6"> <label> 表单编号：</label><el-input v-model="params.Code" size="small" clearable></el-input></el-col>

                <el-col :span="4" class="search-btn-area">
                    <el-button icon="el-icon-search" type="primary" @click="getList" size="small">查询</el-button>
                    <el-button icon="el-icon-delete" type="primary" plain @click="refreshChange(true)" size="small">清空</el-button>
                </el-col>

            </div>


            <div class="nct-toolbar">

                <el-button icon="el-icon-plus" type="primary" size="small" @click="openCreateTab" v-if="auth.addformdef"> 添加</el-button>

                <el-button icon="el-icon-delete" size="small" type="danger" plain @click="multiDelRow" v-if="auth.delformdef">删除</el-button>

                <div class="right-tool">
                    <el-button icon="el-icon-refresh" size="small" circle @click="refreshChange"></el-button>
                    <el-button icon="el-icon-search" size="small" circle @click="isShow=!isShow"></el-button>

                </div>

            </div>
        </div>
        <el-table :data="data" border element-loading-text="拼命加载中" size="small" v-loading="loading" height="200"
                  class="nct-list-table"
                  @selection-change="selectionChange">
            >
            <el-table-column type="selection" align="center"
                             width="50">
            </el-table-column>
            <el-table-column width="55" type="index" label="序号">
                <template slot-scope="scope">
                    <label>{{getRowIndex(scope.$index)}}</label>
                </template>
            </el-table-column>


            <el-table-column prop="Name" label="表单名称" min-width="120">
            </el-table-column>
            <el-table-column prop="Code" label="表单编号" min-width="120">
            </el-table-column>

            <el-table-column prop="CreateTime" label="创建时间" min-width="150">
            </el-table-column>


            <el-table-column prop="Descpt" label="表单说明" min-width="200">
            </el-table-column>

            <el-table-column label="操作" width="200" align="center">
                <template slot-scope="scope">

                    <el-link type="primary" :underline="false" icon="el-icon-edit" @click="openEditTab(scope.row)" v-if="auth.editformdef">编辑</el-link>
                    <el-link type="primary" :underline="false" icon='el-icon-delete' @click="delRow(scope.row,scope.$index+1)" v-if="auth.delformdef">删除</el-link>

                    <el-dropdown @command="handleClick">
                        <el-link type="primary" :underline="false">
                            更多<i class="el-icon-arrow-down el-icon--right"></i>
                        </el-link>
                        <el-dropdown-menu slot="dropdown">
                            <el-dropdown-item :command="{type:1,row:scope.row}">
                                预览列表
                            </el-dropdown-item>
                            <el-dropdown-item :command="{type:2,row:scope.row}">
                                预览单页
                            </el-dropdown-item>
                        </el-dropdown-menu>
                    </el-dropdown>

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




    </section>



</template>
<script>


    import crud from '@/mixins/crud.js'

    export default {
        name: 'formdeflist',
        permissions: 'viewformdef',
        mixins: [crud({ api: window.$formDefApi, isShow: true })],
        data() {
            return {
                selectedForms: [],
            }
        },

        methods: {
            handleClick(e) {
                if (e.type == 1) {
                    this.previewListPage(e.row)
                } else {
                    this.previewFormPage(e.row)
                }

            },

            previewListPage(row) {

                let url = '/form/list?code=' + row.Code + '&noCache=Y';
                let title = `预览列表 - ${row.Name}`
                this.$openPage(url, title);

            },

            previewFormPage(row) {

                let url = `/form/view?mode=preview&code=${row.Code}`;
                let title = `预览表单 - ${row.Name}`
                let opts = { url: url, title: title, width: this.$consts.diaWidth, height: this.$consts.diaHeight };
                this.$showDialog(opts);

            },

            openEditTab(row) {
                var path = '/formdef/design?id=' + row.Id;
                var title = `编辑表单 - ${row.Name}`
                this.$openPage(path, title);
            },

            openCreateTab() {
                var path = '/formdef/design';
                var title = `添加表单`
                this.$openPage(path, title);

            },
            selectionChange(selection) {

                this.selectedForms = selection;
                var ids = [];
                selection.forEach(function (m) {
                    ids.push(m["Id"]);

                });
                this.selectIds = ids;
            }


        },
        mounted() {

            this.initPageAuth('formdef');
            this.getList();
            this.$eventHub.$on('formdef-refresh', this.getList);

        },
        components: {}


    }


</script>
