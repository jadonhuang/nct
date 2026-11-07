<template>
    <section class="nct-work-area">

        <div class="nct-cmd-area">
            <div class="nct-search" v-if="isShow">
                <el-col :span="6"> <label>关键字：</label><el-input v-model="params.Keyword" size="small" clearable></el-input></el-col>

                <el-col :span="4" class="search-btn-area">
                    <el-button icon="el-icon-search" type="primary" @click="getList" size="small">查询</el-button>
                    <el-button icon="el-icon-delete" type="primary" plain @click="refreshChange(true)" size="small">清空</el-button>
                </el-col>

            </div>


            <div class="nct-toolbar">

                <el-button icon="el-icon-delete" size="small" type="danger" plain @click="multiDelRow">删除</el-button>

                <el-button icon="el-icon-view" size="small" type="primary" plain @click="setRead()">全部已读</el-button>

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


            <el-table-column prop="Title" label="标题" min-width="140">
            </el-table-column>

            <el-table-column prop="Content" label="内容" min-width="300">
            </el-table-column>



            <el-table-column width="120" type="index" label="状态" align="center">
                <template slot-scope="scope">

                    <el-tag type="warning" v-if="scope.row.Status==0">未读</el-tag>
                    <el-tag v-else-if="scope.row.Status==1" type="success">已读</el-tag>

                </template>
            </el-table-column>


            <el-table-column prop="CreateTime" label="时间" min-width="120">
            </el-table-column>



            <el-table-column label="操作" width="200" align="center">
                <template slot-scope="scope">


                    <el-link type="primary" :underline="false" :disabled="!needHandle(scope.row)" icon="el-icon-edit" @click="handle(scope.row)">处理</el-link>
                    <el-link type="primary" :underline="false" icon="el-icon-view" @click="setRead(scope.row.Id)" :disabled="scope.row.Status!=0">已读</el-link>
                    <el-link type="primary" :underline="false" icon='el-icon-delete' @click="delRow(scope.row,scope.$index+1)">删除</el-link>

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

        mixins: [crud({ api: window.$messageApi, isShow: true, list:'getMyList' })],
        data() {
            return {

            }
        },

        methods: {

            async setRead(id) {

                await $messageApi.setReaded([id || 0]);
                this.$eventHub.$emit('msg-change');
                this.getList();

            },
            needHandle(row) {
                if (!row.Props) return false;
                var props = JSON.parse(row.Props);

                return !!props.url;
            },


            handle(row) {
                var props = JSON.parse(row.Props);
                if (props.url) {

                    var title = `${row.Title}`
                    var url = props.url;
                    let opts = { url: url, title: title, width: this.$consts.diaWidth, height: this.$consts.diaHeight };
                    this.$showDialog(opts);

                }


            },
            selectionChange(selection) {

                var ids = [];
                selection.forEach(function (m) {
                    ids.push(m["Id"]);

                });
                this.selectIds = ids;
            }


        },
        mounted() {

            this.getList();
            var self = this;
            this.$event.on("notify", function (data) {

                self.getList();

            });

        },
        components: {}


    }


</script>
