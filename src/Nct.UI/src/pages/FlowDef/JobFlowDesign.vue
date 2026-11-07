<template>
    <el-container class="h100">
        <el-main style="padding:0" class="main">
            <el-container class="work-area">

                <el-main class="main-area">
                  
                    <div ref="container" class="fl-container">

                    </div>

                </el-main>

                <el-aside class="item-prop-holder" width="380px" v-if="curNode||runLogVisible">

                    <el-card class="item-prop-card" shadow="never" v-if="runLogVisible">

                        <div slot="header" class="relative">

                            <div class="prop-tab">
                                <div class="prop-tab-title">执行详情</div>
                            </div>

                            <el-link icon="el-icon-close" :underline="false" class="log_close_btn" @click="closeRunLogDetail"></el-link>

                        </div>
                      
                            <LogDetail class="edit-form log-detail" :logId="logId"></LogDetail>
                     
                    </el-card>


                    <el-card class="item-prop-card" shadow="never" v-else>

                        <div slot="header">
                            <div class="prop-tab">
                                <div class="prop-tab-title">节点属性</div>
                            </div>
                        </div>

                        <el-form ref="form" class="edit-form" :model="curNode" size="small" label-width="80px" label-position="top">

                            <el-form-item label="节点标签">
                                <el-input v-model="curNode.text" placeholder="请输入"></el-input>
                            </el-form-item>
                            <component if="curNode" :is="getNodePorpEditorName(curNode)" :model="curNode" :lf="lf"></component>

                        </el-form>

                    </el-card>

                </el-aside>

            </el-container>
        </el-main>
        <el-footer class="footer">

            <el-button-group style="float:right;">
                <el-button :disabled="saving" type="success" icon="el-icon-circle-check" size="mini" @click="saveDesignData(true)">保存并关闭</el-button>
                <el-button :disabled="saving" type="primary" icon="el-icon-circle-check" size="mini" @click="saveDesignData(false)">保存</el-button>
                <el-button :disabled="saving" type="warning" icon="el-icon-circle-close" size="mini" @click="confirmCloseCurPage">取消</el-button>
                <el-button :disabled="saving" :loading="running" icon="el-icon-mouse" size="mini" @click="testRun()" type="info">运行流程</el-button>
            </el-button-group>
        </el-footer>


       



    </el-container>



</template>




<script src="./JobFlowDesign.js"></script>
<style scoped src="./Design.css"></style>


<style scoped>
    .log-detail {
        max-height: calc(100vh - 172px) !important;
    }
    .log_close_btn {
        position: absolute;
        right: 20px;
        top: 14px;
        font-size:14px;
    }
    .edit-form {
        max-height: calc(100vh - 188px);
    }
    .main {
        background-color: #fff;
    }
    .footer {
        border-top: 1px solid #DCDFE6;
    }
    .el-form {
        padding: 12px;
    }
    /deep/ .edit-form .el-form-item {
        margin-bottom: 5px !important;
        margin-right: 0px;
        padding-right:0px;
    }


    /deep/ .edit-form .el-form-item__label {
        padding-bottom: 5px !important;
        line-height: 20px !important;
        font-weight: bolder;
        font-size: 12px;
        line-height: 25px !important;
        padding-left: 0px;
    }
    .prop-tab-title {
        padding-left: 12px;
        /*background-color: rgb(245, 245, 249);*/
    }

    /deep/ .main .lf-resize-control-nw, /deep/ .main .lf-resize-control-ne, /deep/ .main .lf-resize-control-sw {
        display: none;
    }

    
</style>
