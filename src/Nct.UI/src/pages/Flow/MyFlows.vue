<template>
    <section class="nct-work-area">

        <div class="nct-cmd-area">
            <div class="nct-search" v-if="isShow">
                <el-col :span="6"> <label>名称：</label><el-input v-model="params.Title" size="small" clearable></el-input></el-col>
              
                <el-col :span="4" class="search-btn-area">
                    <el-button icon="el-icon-search" type="primary" @click="getList" size="small">查询</el-button>
                    <el-button icon="el-icon-delete" type="primary" plain @click="refreshChange(true)" size="small">清空</el-button>
                </el-col>

            </div>


            <div class="nct-toolbar">

                <el-dropdown>
                    <el-button type="primary"  size="small" >
                        发起申请<i class="el-icon-plus el-icon--right"></i>
                    </el-button>
                    <el-dropdown-menu slot="dropdown">
                        <el-dropdown-item v-for="flow in flowDefs" :key="flow.Code" @click.native="create(flow)">{{flow.Name}}</el-dropdown-item>      
                    </el-dropdown-menu>
                </el-dropdown>

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

            <el-table-column width="55" type="index" label="序号">
                <template slot-scope="scope">
                    <label>{{getRowIndex(scope.$index)}}</label>
                </template>
            </el-table-column>


            <el-table-column prop="Title" label="流程名称" min-width="200">
            </el-table-column>

            <el-table-column width="120" type="index" label="状态" align="center">
                <template slot-scope="scope">

                    <el-tag type="info" v-if="scope.row.Status==0">未开始</el-tag>
                    <el-tag v-else-if="scope.row.Status==1">处理中</el-tag>
                    <el-tag v-else   effect="plain">已结束</el-tag>

                </template>
            </el-table-column>

            <el-table-column width="120" type="index" label="审批结果" align="center">
                <template slot-scope="scope">

                  
                    <el-tag v-if="scope.row.Status==2" type="success">同意</el-tag>
                    <el-tag v-else-if="scope.row.Status==3" type="warning">拒绝</el-tag>

                </template>
            </el-table-column>


            <el-table-column prop="CreateTime" label="开始时间" min-width="130">
            </el-table-column>

            <el-table-column prop="EndTime" label="结束时间" min-width="130">
            </el-table-column>


            <el-table-column label="操作" width="200" align="center">
                <template slot-scope="scope">

                    <el-link type="primary" :underline="false" icon="el-icon-view" @click="view(scope.row)">查看</el-link>
                   
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

        mixins: [crud({ api: window.$flowApi, list: 'getMyFlowList', isShow: true })],
        data() {
            return {
               flowDefs:[]
            }
        },

        methods: {


            view(row) {

                var title = `查看申请 - ${row.Title}`
                var url = '/flow/view?id=' + row.Id;
                let opts = { url: url, title: title, width: this.$consts.diaWidth, height: this.$consts.diaHeight };
                this.$showDialog(opts);
            },

            create(flow) {
                var title = `创建申请 - ${flow.Name}`
                var url = '/flow/view?code=' + flow.Code;
                let opts = { url: url, title: title, width: this.$consts.diaWidth, height: this.$consts.diaHeight };
                this.$showDialog(opts);

            },
            selectionChange(selection) {
                var ids = [];
                selection.forEach(function (m) {
                    ids.push(m["Id"]);

                });
                this.selectIds = ids;
            },
            async getFlowDefs() {
                var res = await $flowDefApi.getAllSimpleList();
                this.flowDefs = res.Data;

            }


        },
        mounted() {

            this.getList();
            this.getFlowDefs();
            this.$eventHub.$on('flow-created', this.getList);

        },
        components: {}


    }


</script>
