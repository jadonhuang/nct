<template>
    <el-container class="h100">
        <el-main style="padding:0" class="main">
            <el-tabs v-model="curTabId" class="main-tab" type="border-card">
                <el-tab-pane label="基本信息" name="first" style="padding:20px;">
                    <el-form ref="infoform" label-width="100px" :model="model" :rules="rules" size="medium">
                        <el-form-item label="流程名称" prop="Name">
                            <el-input v-model="model.Name" placeholder="请输入流程名称"></el-input>
                        </el-form-item>
                        <el-form-item label="流程编码" prop="Code">
                            <el-input v-model="model.Code" :disabled="saveType==2" placeholder="只能输入数字、字母、下划线">

                            </el-input>
                        </el-form-item>


                        <el-form-item label="流程说明">
                            <el-input v-model="model.Descpt" type="textarea" :rows="2"></el-input>
                        </el-form-item>
                    </el-form>
                </el-tab-pane>
                <el-tab-pane label="流程设计" name="second">

                    <el-container class="work-area">

                        <el-main class="main-area">
                            <div class="fl-tool-holder">

                                <ToolNode type="task" :lf="lf" text="任务节点"></ToolNode>
                                <ToolNode type="case" :lf="lf" text="判断节点" style="margin-left:10px;"></ToolNode>
                                <ToolNode type="script" :lf="lf" text="脚本节点" style="margin-left:10px;"></ToolNode>

                            </div>

                            <div ref="container" class="fl-container">

                            </div>

                        </el-main>

                        <el-aside class="item-prop-holder" width="380px" v-if="curNode">

                            <el-card class="item-prop-card" shadow="never">

                                <div slot="header">

                                    <div class="prop-tab">
                                        <div class="prop-tab-title">节点属性</div>

                                    </div>


                                </div>

                                <el-form ref="form" :model="curNode" class="edit-form" size="small" label-width="80px" label-position="left">

                                    <el-form-item label="节点Id">
                                        {{curNode.id}}
                                    </el-form-item>

                                    <el-form-item label="节点标签" v-if="curNode.type=='case'||curNode.type=='task'||curNode.type=='script'">
                                        <el-input v-model="curNode.text" placeholder="请输入"></el-input>
                                    </el-form-item>

                                    <el-form-item label="流程表单" v-if="curNode.type=='start'">
                                        <el-select v-model="curNode.props.FormCode" placeholder="请选择" filterable class="w100">
                                            <el-option v-for="item in formDefs"
                                                       :key="item.Code"
                                                       :label="item.Name"
                                                       :value="item.Code">
                                            </el-option>
                                        </el-select>

                                    </el-form-item>


                                    <template v-if="curNode.type=='script'">

                                        <el-form-item label="代码脚本">
                                            <el-input v-model="curNode.props.Code" type="textarea" :rows="10" placeholder="C#脚本，返回true则继续执行下一节点，否则终止"></el-input>

                                        </el-form-item>

                                    </template>


                                    <template v-if="curNode.type=='task'">

                                        <el-form-item label="审批人">

                                            <div class="audit-cmd">

                                                <el-button type="primary" @click="addAuditUser" size="mini" icon="el-icon-user-solid">添加用户</el-button>
                                                <el-button type="primary" @click="showAuditLeaderDialog" size="mini" icon="el-icon-s-custom">添加领导</el-button>
                                                <el-button type="primary" @click="showAuditRoleDialog" size="mini" icon="el-icon-user">添加角色</el-button>

                                            </div>

                                            <el-table ref="auditTable" :data="curNode.props.Audit.Members" border size="mini" class="edit-table">

                                                <el-table-column prop="Name" label="类型" min-width="50">
                                                    <template slot-scope="scope">

                                                        <template v-if="scope.row.Type==1">
                                                            用户
                                                        </template>
                                                        <template v-else-if="scope.row.Type==2">
                                                            角色
                                                        </template>
                                                        <template v-if="scope.row.Type==3">
                                                            领导
                                                        </template>

                                                    </template>
                                                </el-table-column>

                                                <el-table-column prop="Name" label="名称" min-width="90">
                                                    <template slot-scope="scope">
                                                        {{scope.row.Name}}

                                                    </template>
                                                </el-table-column>


                                                <el-table-column label="操作" width="80" align="center">
                                                    <template slot-scope="scope">

                                                        <el-link type="primary" :underline="false" icon='el-icon-delete' @click="delAudit(scope.$index)">删除</el-link>

                                                    </template>
                                                </el-table-column>
                                            </el-table>

                                        </el-form-item>
                                        <el-form-item label="审批方式">
                                            <el-radio-group v-model="curNode.props.Audit.Type" class="audit-type">
                                                <el-radio :label="1">会签(默认需要所有人同意)</el-radio>
                                                <el-radio :label="2">或签(一名审批人同意即可)</el-radio>
                                                <el-radio :label="3">依次审批(按顺序依次审批)</el-radio>
                                            </el-radio-group>

                                            <template v-if="curNode.props.Audit.Type==1">
                                                通过比例(%)
                                                <el-input-number style="width:100px" v-model="curNode.props.Audit.Percent"
                                                                 controls-position="right" :min="1" :max="100"></el-input-number>
                                            </template>
                                        </el-form-item>
                                        <el-form-item label="审批时限">
                                            <el-switch v-model="curNode.props.Audit.TimeLimit.Enabled" active-text="开" inactive-text="关">
                                            </el-switch>

                                        </el-form-item>

                                        <el-form-item label="过期时长" v-if="curNode.props.Audit.TimeLimit.Enabled">

                                            <el-input-number style="width:58%" v-model="curNode.props.Audit.TimeLimit.Timeout" controls-position="right" :min="1"></el-input-number>

                                            <el-select style="width:40%;margin-left:2px;" v-model="curNode.props.Audit.TimeLimit.Unit">
                                                <el-option label="天" :value="1"> </el-option>
                                                <el-option label="小时" :value="2"> </el-option>
                                                <el-option label="分钟" :value="3"> </el-option>
                                            </el-select>

                                        </el-form-item>

                                        <el-form-item label="处理方式" v-if="curNode.props.Audit.TimeLimit.Enabled">

                                            <el-radio-group v-model="curNode.props.Audit.TimeLimit.ExpireHandle" class="audit-type">
                                                <el-radio :label="1">通知提醒</el-radio><br />
                                                <el-radio :label="2">自动通过</el-radio><br />
                                                <el-radio :label="3">自动拒绝</el-radio>
                                            </el-radio-group>
                                            <el-switch v-if="curNode.props.Audit.TimeLimit.ExpireHandle==1" v-model="curNode.props.Audit.TimeLimit.LoopNotify" active-text="循环提醒" inactive-text="仅提醒一次">
                                            </el-switch>


                                        </el-form-item>

                                        <el-form-item label="表单编辑">
                                            <el-switch v-model="curNode.props.FormEdit" active-text="允许" inactive-text="禁止">
                                            </el-switch>

                                        </el-form-item>

                                    </template>

                                    <template v-if="curNode.type=='case'">

                                        <el-form-item label="条件分支">

                                            <el-table ref="cdtTable" :data="curNode.props.Cdts" border row-key="Id" size="mini" class="edit-table">

                                                <el-table-column prop="Text" label="标签" min-width="60">
                                                    <template slot-scope="scope">
                                                        <el-input v-model="scope.row.Text" @change="saveCurNodeProps" size="small"></el-input>

                                                    </template>
                                                </el-table-column>

                                                <el-table-column label="操作" width="120" align="center">
                                                    <template slot-scope="scope">

                                                        <el-link type="primary" :underline="false" v-if="!scope.row.IsDefault" icon='el-icon-edit' @click="editCdt(scope.$index)">设置</el-link>
                                                        <el-link type="primary" :underline="false" v-if="!scope.row.IsDefault" icon='el-icon-delete' @click="delCdt(scope.$index)">删除</el-link>

                                                    </template>
                                                </el-table-column>
                                            </el-table>

                                            <el-link type="primary" :underline="false" icon='el-icon-plus' @click="addCdt()">添加</el-link>
                                        </el-form-item>

                                    </template>


                                </el-form>

                            </el-card>

                        </el-aside>

                    </el-container>

                </el-tab-pane>





            </el-tabs>
        </el-main>
        <el-footer class="footer">

            <el-button-group style="float:right;">
                <el-button type="warning" icon="el-icon-circle-close" size="mini" @click="confirmCloseCurPage">取消</el-button>
                <el-button type="primary" icon="el-icon-circle-check" size="mini" @click="saveDesignData(false)">保存</el-button>
                <el-button type="success" icon="el-icon-circle-check" size="mini" @click="saveDesignData(true)">保存并关闭</el-button>

            </el-button-group>
        </el-footer>


        <el-dialog title="设置条件" :visible.sync="cdtEditDialogVisible" append-to-body width="500px">
            <el-form ref="form" v-if="cdt" :model="curNode" class="edit-form" size="small" label-width="80px" label-position="left">

                <el-form-item label="Sql条件">
                    <el-input v-model="cdt.SqlCdt" type="textarea" :rows="2" placeholder="age>20"></el-input>
                </el-form-item>

                <el-form-item label="代码条件">
                    <el-input v-model="cdt.CodeCdt" type="textarea" :rows="2" placeholder="c#脚本代码"></el-input>
                </el-form-item>

                <el-form-item label="条件关系">
                    <el-radio-group v-model="cdt.CdtType">
                        <el-radio :label="1">并且</el-radio>
                        <el-radio :label="2">或者</el-radio>
                    </el-radio-group>


                    <div class="el-form-item__error" v-if="cdtErrMsg">
                        {{cdtErrMsg}}
                    </div>
                </el-form-item>
            </el-form>

            <span slot="footer">
                <el-button @click="cdtEditDialogVisible = false" size="small">取 消</el-button>
                <el-button type="primary" @click="saveCdt" size="small">确 定</el-button>
            </span>

        </el-dialog>


        <el-dialog title="设置条件" :visible.sync="auditLeaderDialogVisible" append-to-body width="500px">
            申请人的上级领导：
            <div style="padding:10px;">

                <el-checkbox-group v-model="checkedAuditLeaderIds">
                    <el-checkbox v-for="item in auditLeaders" :label="item.Id" :key="item.Id">{{item.Name}}</el-checkbox>
                </el-checkbox-group>

            </div>

            <span slot="footer">
                <el-button @click="auditLeaderDialogVisible = false" size="small">取 消</el-button>
                <el-button type="primary" @click="setAuditLeader" size="small">确 定</el-button>
            </span>

        </el-dialog>

        <RoleSelect ref="roleSelect" @change="setAuditRole"></RoleSelect>


    </el-container>



</template>




<script src="./Design.js"></script>
<style scoped src="./Design.css"></style>


