<template>

    <section class="nct-work-area" v-loading="loading">

        <el-dialog title="拒绝理由" :visible.sync="dialogVisible" append-to-body width="500px">
            <el-input type="textarea"
                      :rows="4"
                      placeholder="请输入拒绝理由,至少5个字符"
                      v-model="auditRemark">
            </el-input>

            <span slot="footer" class="dialog-footer">
                <el-button @click="dialogVisible = false">取 消</el-button>
                <el-button type="primary" @click="audit(2)" :loading="saveLoading">确 定</el-button>
            </span>

        </el-dialog>

        <div class="form-container fl-container" v-show="!loading">
            <div class="flow-title" :class="statusImg">
                {{flowDef.Name}}
            </div>


            <el-tabs v-model="tabActiveName" type="card" class="main-tab">
                <el-tab-pane label="申请信息" name="first">
                    <FormView v-if="formViewArgs" :isNested="true" :args="formViewArgs" ref="formView"></FormView>

                    <div class="audit-cnt" v-if="isAudited">
                        <div class="left">
                            <div class="audit-time">审核时间：{{taskItem.AuditTime}}</div>
                            <div class="audit-remark" v-if="taskItem.Remark">审核备注：{{taskItem.Remark}}</div>
                        </div>
                        <div class="right" :class="taskItem.Status==1?'agree':'refuse'"></div>
                    </div>
                </el-tab-pane>
                <el-tab-pane label="审核流程" name="second">
                    <GraphView :config="flowData" v-if="flowData"></GraphView>
                </el-tab-pane>
                <el-tab-pane label="审核历史" v-if="hasHis" name="third">

                    <el-timeline v-if="flow" style="margin-top:15px;">
                        <el-timeline-item :timestamp="task.NodeName+'  '+task.CreateTime" :color="getStepColor(task)" placement="top" :key="task.Id" v-for="task in tasks">

                            <el-card v-if="task.Items.length>0" shadow="never">
                                <div v-for="item in task.Items" :key="item.Id">
                                    <p v-if="item.Status==0||item.Status==99">
                                        {{item.UserName}} <font color="orange">未处理</font>
                                    </p>
                                    <p v-else>

                                        {{item.UserName}} {{item.AuditTime}} <font color="green" v-if="item.Status==1">同意</font>

                                        <font color="red" v-if="item.Status==2">拒绝</font>

                                        <label style="margin-left:10px;">{{item.Remark}}</label>

                                    </p>
                                </div>
                            </el-card>


                        </el-timeline-item>


                        <el-timeline-item v-if="flow.IsEnd" :timestamp="'结束  '+flow.EndTime" color="#f56c6c" placement="top">
                        </el-timeline-item>


                    </el-timeline>


                </el-tab-pane>

            </el-tabs>



        </div>

        <div class="el-dialog__footer" v-show="!loading">
            <template v-if="isCreate">
                <el-button type="primary" size="small" @click="saveData" :loading="saveLoading" icon="el-icon-circle-check">提交申请</el-button>

            </template>
            <template v-if="canAudit">
                <el-button type="primary" size="small" @click="audit(1)" :loading="saveLoading" icon="el-icon-circle-check">同意</el-button>
                <el-button type="warning" size="small" @click="dialogVisible=true" :loading="saveLoading" icon="el-icon-error">拒绝</el-button>

            </template>

            <el-button size="small" icon="el-icon-circle-close" @click="close(false)">取消</el-button>
        </div>

    </section>
</template>

<script>

    import FormView from '@/pages/Form/View';
    import GraphView from './GraphViewComp';
    import emitter from '@/mixins/emitter';


    export default {
        mixins: [emitter],
        data() {
            return {
                flowDef: {},
                flowData: null,
                loading: true,
                formViewArgs: null,
                saveLoading: false,
                tabActiveName: 'first',
                flow: null,
                id: null,
                taskItem: null,
                curNode: null,
                auditRemark: '',
                dialogVisible: false,
                tasks: []
            }
        },
        computed: {
            isCreate() {
                return !this.id && !this.taskItem
            },
            hasHis() {
                return this.id
            },
            isAudited() {
                return this.taskItem && this.taskItem.Status != 0
            },
            canAudit() {
                return this.taskItem && this.taskItem.Status == 0
            },
            auditFormEdit() {

                return this.canAudit && this.curNode.properties.FormEdit
            },
            statusImg() {

                if (!this.flow) return null;
                if (this.flow.Status == 2)
                    return 'passed'
                if (this.flow.Status == 3)
                    return 'notpassed'
                return null;
            }


        },

        methods: {
            getStepColor(task) {
                if (task.NodeName == '开始')
                    return '#1989fa';

                if (task.Status == 2)
                    return '#67c23a';

                if (task.Status == 3)
                    return '#e6a23c';

                return '';

            },
            async getData() {

                let id = this.id;
                let formArgs = {};
                if (id) {

                    let res = await $flowApi.get(id);
                    if (!res.IsSuccess) {

                        this.$errTip('流程不存在')
                        this.close(true);
                        return;
                    }
                    this.flow = res.Data;
                    formArgs.id = this.flow.FormDataId;
                    this.code = this.flow.FlowCode;
                    if (this.taskId) {

                        let res2 = await $flowApi.getTaskItem(this.taskId);
                        if (!res2.IsSuccess) {

                            this.$errTip('任务不存在');
                            this.close(true);
                            return;
                        }
                        this.taskItem = res2.Data;

                    }

                }

                let res = await $flowDefApi.getByCode(this.code);
                if (!res.IsSuccess) {

                    this.$errTip('流程不存在')
                    this.close(true);
                    return;
                }
                let data = res.Data;
                this.flowDef = data;
                this.flowData = JSON.parse(data.Data);
                if (this.taskItem) {

                    let curNode = this.flowData.nodes.find(m => m.id == this.taskItem.NodeId);
                    this.curNode = curNode;
                }
                let startNode = this.flowData.nodes.find(m => m.type == 'start');
                let formCode = startNode.properties.FormCode;
                this.formCode = formCode;

                if (!(this.isCreate || this.auditFormEdit)) {
                    formArgs.mode = 'view';

                }

                formArgs.code = formCode;
                this.formViewArgs = formArgs;

                if (this.hasHis) {

                    res = await $flowApi.getTasks(this.flow.Id);
                    this.tasks = res.Data;
                }

            },


            async initData() {

                this.loading = true;
                const code = this.$route.query.code;
                this.code = code;
                const id = this.$route.query.id || '';
                this.id = id

                const taskId = this.$route.query.taskId || '';
                this.taskId = taskId

                await this.getData();
                await this.$refs.formView.initData();
                this.loading = false;

            },



            async audit(status) {

             
                this.tabActiveName = 'first'


                if (status == 2 && (!this.auditRemark || this.auditRemark.trim().length < 5)) {

                    this.$errTip('请正确输入拒绝理由');
                    return;

                }

                this.saveLoading = true;
                let formData = null;
                if (this.auditFormEdit) {
                    let formView = this.$refs.formView;
                    let valid = await formView.validateData();

                    if (!valid) {
                        this.saveLoading = false;
                        return;
                    }
                    let result = await formView.executeBeforeSaveRule();
                    if (result === false) {
                        this.saveLoading = false;
                        return;
                    }
                    formData = JSON.stringify(formView.getFormData());
                }

                let remark = status == 1 ? null : this.auditRemark;
                let dto = { Status: status, FormData: formData, Remark: remark, TaskId: this.taskId, FormDataId: this.flow.FormDataId };

                let res = await $flowApi.audit(dto);
                this.saveLoading = false;
                if (res.IsSuccess) {
                    this.$okTip(res.Msg || '审核成功')
                    this.$eventHub.$emit('flow-audited');
                    this.dialogVisible = false;
                    this.close(true);

                } else {
                    this.$errTip(res.Msg)

                }


            },

            async saveData() {

                this.tabActiveName = 'first'
                var formView = this.$refs.formView;
                var valid = await formView.validateData();

                if (!valid) return;

                this.saveLoading = true;
                var result = await formView.executeBeforeSaveRule();
                if (result === false) {
                    this.saveLoading = false;
                    return;
                }

                var formData = JSON.stringify(formView.getFormData());
                var dto = { FlowCode: this.code, FormData: formData, Id: 0 };
                var res = await $flowApi.create(dto);
                this.saveLoading = false;
                if (res.IsSuccess) {
                    this.$okTip(res.Msg || '创建成功，请等待审核')
                    this.$eventHub.$emit('flow-created');
                    this.close(true);

                } else {
                    this.$errTip(res.Msg)

                }


            },




            async close(force) {

                this.forceClose = force || false;
                if (this.pageIsInDialog) {
                    this.$closeDialog();
                } else {

                    this.$closePage();
                }

            },
            calPageIsInDialog() {

                var parent = this.$parent;
                var name = parent.$options.componentName;

                while (parent && (!name || name !== 'Dialog')) {
                    parent = parent.$parent;
                    if (parent) {
                        name = parent.$options.componentName;
                    }
                }

                return !!parent

            },




        },
        async mounted() {

            this.pageIsInDialog = this.calPageIsInDialog();
            await this.initData();

        },
        components: { FormView, GraphView }


    }


</script>

<style scoped>
    .flow-title {
        font-size: 20px;
        font-weight: bold;
        text-align: center;
        height: 64px;
        line-height: 64px;
        margin-top: 10px;
    }

    .fl-container {
        padding: 0px 20px 20px 20px;
        display: flex;
        flex-flow: column;
    }

    .nct-work-area {
        background-color: #fff;
        padding: 0;
    }

    .audit-cnt {
        display: flex;
        padding-top: 15px;
        padding-bottom: 15px;
        font-size: 14px;
        padding-left: 30px;
    }

        .audit-cnt .left {
            display: flex;
            vertical-align: central;
            flex-flow: column;
            height: 64px;
            justify-content: center;
        }

        .audit-cnt .right {
            margin-left: 100px;
            width: 68px;
            text-align: center;
            height: 64px;
            margin-right: 100px;
        }



    /deep/ .form-container .nct-work-area {
        height: auto;
    }

    .main-tab {
        flex: 1;
        display: flex;
        flex-flow: column;
    }

    .audit-time {
        padding-bottom: 10px;
    }

    .passed {
        background: url("../../assets/img/passed.png") no-repeat;
        background-position: right;
    }

    .notpassed {
        background: url("../../assets/img/notpassed.png") no-repeat;
        background-position: right;
    }

    .agree {
        background: url("../../assets/img/agree.png") no-repeat;
        background-position: center;
        background-size: contain;
    }

    .refuse {
        background: url("../../assets/img/refuse.png") no-repeat;
        background-position: center;
        background-size: contain;
    }

    /deep/ .main-tab .el-tabs__content {
        flex: 1;
    }

        /deep/ .main-tab .el-tabs__content .el-tab-pane {
            height: 100%;
        }
</style>