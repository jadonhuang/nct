
<template>
    <el-main class="nct-work-area">

        <div class="nct-cmd-area">
            <div class="nct-search" v-if="isShow">
                <el-col :span="6"> <label> 任务名称：</label><el-input v-model="params.Name" size="small" clearable></el-input></el-col>
                <el-col :span="4" class="search-btn-area">
                    <el-button icon="el-icon-search" type="primary" @click="getList" size="small">查询</el-button>
                    <el-button icon="el-icon-delete" type="primary" plain @click="refreshChange(true)" size="small">重置</el-button>
                </el-col>
            </div>

        </div>

        <el-table :data="data" border element-loading-text="拼命加载中" size="small" v-loading="loading" height="0" class="nct-list-table" @selection-change="selectionChange">

            <el-table-column width="55" type="index" label="序号">
                <template slot-scope="scope">
                    <label>{{getRowIndex(scope.$index)}}</label>
                </template>
            </el-table-column>

            <el-table-column prop="Name" label="任务名称" min-width="120">  </el-table-column>


            <el-table-column label="任务类型" min-width="150" align="center">

                <template slot-scope="scope">
                    {{scope.row.ExeMethod}}
                </template>
            </el-table-column>



            <el-table-column prop="IntervalType" label="执行方式" min-width="100">

                <template slot-scope="scope">

                    <template v-if="scope.row.IntervalType==1">
                        每隔{{scope.row.IntervalMinute}}分钟
                    </template>

                    <template v-else>
                        {{scope.row.IntervalCron}}
                    </template>

                </template>

            </el-table-column>

            <el-table-column prop="IsEnabled" label="是否启用" min-width="80" align="center">

                <template slot-scope="scope">
                    <el-tag type="success" v-if="scope.row.IsEnabled">启用</el-tag>
                    <el-tag type="warning" v-else>停用</el-tag>

                </template>
            </el-table-column>


            <el-table-column prop="LastTime" label="最后运行时间" min-width="100">  </el-table-column>


            <el-table-column prop="LastTime" label="最后运行状态" min-width="80" align="center">
                <template slot-scope="scope">
                    <el-tag type="success" v-if="scope.row.LastExeSuccess">成功</el-tag>
                    <el-tag type="warning" v-else-if="scope.row.LastExeSuccess==false">失败</el-tag>
                </template>

            </el-table-column>


            <el-table-column prop="CreateTime" label="创建时间" min-width="100">  </el-table-column>

            <el-table-column label="操作" width="250" align="center">
                <template slot-scope="scope">
                    <el-link type="primary" :underline="false" icon="el-icon-caret-right" @click="runJob(scope.row)" :disabled="!scope.row.IsEnabled">运行任务</el-link>
                    <el-link type="primary" :underline="false" icon="el-icon-tickets" @click="viewLog(scope.row)" >运行日志</el-link>
                    <el-link type="primary" :underline="false" icon="el-icon-circle-check" @click="enableJob(scope.row)">启用/禁用</el-link>


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
        name: 'jobdeflist',
        permissions: 'viewjobdef',
        mixins: [crud({ api: window.$jobDefApi, isShow: true })],
        data() {
            return {



            }
        },

        methods: {
            processParams(params) {
                params.Types = [3];
                const queryParams = { pageIndex: this.page.pageIndex, pageSize: this.page.pageSize, Query: params }
                return queryParams;

            },
            viewLog(row) {
                var title = `查看日志 - ${row.Name}`
                var url = '/joblog?jobId=' + row.ExeMethod;
                let opts = { url: url, title: title, height: this.$consts.diaHeight, width: 1000 };
                this.$showDialog(opts);

            },
            async enableJob(row) {
                var loading = this.$showLoading('提交中...');
                var res = await $jobDefApi.switchJobEnabled(row.Id);
                loading.close()
                if (res.IsSuccess) {

                    this.$okTip("执行成功");
                    this.getList();

                } else {

                    this.$errTip(res.Msg || '执行失败，请查看日志');

                }
               
            },


            async runJob(row) {
                var loading = this.$showLoading('执行中...');
                var res = await $jobDefApi.run(row.Id,false);
                loading.close()
                if (res.IsSuccess) {

                    this.$okTip("执行成功");

                } else {

                    this.$errTip(res.Msg);

                }
                this.getList();
            }


        },
        mounted() {
            this.initPageAuth('jobdef');
            this.getList();
        },
        components: { CreateForm, EditForm }


    }


</script>



