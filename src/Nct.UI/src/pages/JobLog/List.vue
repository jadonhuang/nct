

<template>

    <el-main class="nct-work-area">

        <div class="nct-cmd-area">
            <div class="nct-search" v-if="isShow">
                <el-col :span="6">
                    <label> 执行结果：</label>

                    <el-select placeholder="全部" v-model="params.Success">
                        <el-option label="全部" :value="null"> </el-option>
                        <el-option label="成功" :value="true"> </el-option>
                        <el-option label="失败" :value="false"> </el-option>
                    </el-select>

                </el-col>
                <el-col :span="10">
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

                    <el-button icon="el-icon-delete-solid" type="danger" plain @click="clearLog()" size="small">清空日志</el-button>

                </el-col>

            </div>

        </div>

        <el-table :data="data" border element-loading-text="拼命加载中" size="small" v-loading="loading" height="200" class="nct-list-table" @selection-change="selectionChange">


            <!--<el-table-column type="selection" align="center"
                             width="50">
            </el-table-column>-->
            <el-table-column width="55" type="index" label="序号">
                <template slot-scope="scope">
                    <label>{{getRowIndex(scope.$index)}}</label>
                </template>
            </el-table-column>

            <el-table-column prop="IsSuccess" label="执行结果" min-width="120">

                <template slot-scope="scope">
                    <el-tag type="success" v-if="scope.row.IsSuccess">成功</el-tag>
                    <el-tag type="warning" v-else>失败</el-tag>
                </template>
            </el-table-column>

            <el-table-column prop="ElapsedTime" label="执行用时" min-width="120">
                <template slot-scope="scope">

                    {{$formatInterval(scope.row.ElapsedTime)}}
                </template>


            </el-table-column>

            <el-table-column prop="CreateTime" label="执行时间" min-width="120">  </el-table-column>

            <el-table-column label="操作" width="180" align="center">
                <template slot-scope="scope">
                    <el-link type="primary" :underline="false" icon="el-icon-view" @click="viewDetail(scope.row)">详情</el-link>
                    <el-link type="primary" :underline="false" icon='el-icon-delete' @click="delRow(scope.row,scope.$index+1)" v-if="auth.deljobflowlog">删除</el-link>

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

    </el-main>


</template>
<script>

    import crud from '@/mixins/crud.js'
    export default {
        name: 'jobloglist',
        permissions: 'viewjoblog',
        mixins: [crud({ api: window.$jobLogApi, isShow: true })],
        data() {
            return {
                jobId: 0,

            }
        },

        methods: {
            viewDetail(row) {
                var title = `详情`
                var url = '/joblog/detail?id=' + row.Id;
                let opts = { url: url, title: title, height: 500, width: 400, resizeable: 'N' };
                this.$showDialog(opts);

            },
            processParams(params) {
                params.jobName = this.jobId;
                const queryParams = { pageIndex: this.page.pageIndex, pageSize: this.page.pageSize, Query: params }
                return queryParams;

            },
            async clearLog() {

                const confirm = await this.$confirmTip('确认清除?');
                if (!confirm) return;

                var loading = this.$showLoading('提交中...');
                var res = await $jobLogApi.clearLog(this.jobId);
                loading.close()
                if (res.IsSuccess) {
                    this.$okTip("操作成功");
                    this.getList();
                } else {
                    this.$errTip(res.Msg || '操作失败');

                }

            },


        },
        mounted() {
            this.jobId = this.$route.query.jobId;
            this.initPageAuth('joblog');
            this.getList();
        },



    }


</script>


