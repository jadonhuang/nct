<template>
    <section class="nct-work-area">

        <div class="nct-cmd-area">
            <div class="nct-search" v-if="isShow">
                <el-col :span="6"> <label> 关键字：</label><el-input v-model="params.Keyword" size="small" clearable></el-input></el-col>
                <el-col :span="6">
                    <label> 操作日期：</label>  <el-date-picker v-model="params.DateRange" size="small" value-format="yyyy-MM-dd"
                                                           type="daterange"
                                                           range-separator="至"
                                                           start-placeholder="开始日期"
                                                           end-placeholder="结束日期">
                    </el-date-picker>
                </el-col>

                <el-col :span="4" class="search-btn-area">
                    <el-button icon="el-icon-search" type="primary" @click="getList" size="small">查询</el-button>
                    <el-button icon="el-icon-delete" type="primary" plain @click="refreshChange(true)" size="small">重置</el-button>
                </el-col>

            </div>

        </div>
        <el-table :data="data" border element-loading-text="拼命加载中" size="small" v-loading="loading" height="200" class="nct-list-table"
                  @selection-change="selectionChange">

            <el-table-column width="55" type="index" label="序号">
                <template slot-scope="scope">
                    <label>{{getRowIndex(scope.$index)}}</label>
                </template>
            </el-table-column>
            <el-table-column prop="UserName" label="用户" min-width="90">
            </el-table-column>
            <el-table-column prop="Name" label="状态" min-width="80" align="center">
                <template slot-scope="scope">
                    <el-tag v-if="scope.row.Success">成功</el-tag>
                    <el-tag v-else type="warning">失败</el-tag>
                </template>

            </el-table-column>

            <el-table-column prop="BrowserInfo" label="浏览器" min-width="200">
            </el-table-column>

            <el-table-column prop="ClientIpAddress" label="IP地址" min-width="100">
            </el-table-column>

            <el-table-column prop="ExecutionTime" label="操作时间" min-width="100">
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
        name: 'loglist',
        permissions: 'viewlog',
        mixins: [crud({ api: window.$logApi, isShow: true })],
        data() {
            return {

            }
        },

        methods: {


        },
        mounted() {


            this.getList();

        },



    }


</script>

