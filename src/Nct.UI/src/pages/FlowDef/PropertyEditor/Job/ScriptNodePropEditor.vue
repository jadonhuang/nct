<template>
    <div>
        <el-form-item>
            <div class="title-wrap">输入变量   <el-link title="新增变量" icon="el-icon-circle-plus-outline" @click="addVariable(1)" :underline="false"></el-link></div>
            <table class="table">
                <tr>
                    <th width="40%">名称</th>
                    <th>值</th>
                </tr>

                <tr v-for="(item,index) in model.props.Variables" :key="index+'i'">
                    <td>
                        <el-input v-model="item.Name" placeholder="变量名" @input="propsChange"></el-input>
                    </td>
                    <td>

                        <div class="wrap">
                            <div class="var-right">
                                <VariableEditor :minHeight="25" :node="nodeModel" v-model="item.Value" :readonly="true" @input="propsChange"></VariableEditor>
                            </div>
                            <span class="btn-area">
                                <el-link icon="el-icon-delete" @click="removeVariable(1, index)" :underline="false"></el-link>
                            </span>
                        </div>

                    </td>
                </tr>

            </table>
        </el-form-item>
        <el-form-item>
            <div class="title-wrap">脚本 </div>
            <el-input type="textarea"
                      :rows="6"
                      placeholder="请输入C#脚本"
                      v-model=" model.props.Code" @input="propsChange">
            </el-input>

        </el-form-item>

        <el-form-item>
            <div class="title-wrap">输出变量 <el-link title="新增变量" icon="el-icon-circle-plus-outline" @click="addVariable(2)" :underline="false"></el-link></div>
            <table class="table">
                <tr>
                    <th width="40%">名称</th>
                    <th>类型</th>
                </tr>

                <tr v-for="(item,index) in model.props.Outputs" :key="index+'i'">
                    <td>
                        <el-input v-model="item.Name" placeholder="变量名" size="mini" @input="propsChange"></el-input>
                    </td>
                    <td>
                        <div class="wrap">
                            <div class="var-right">
                                <el-select v-model="item.Type" style="width:100%" @change="propsChange">
                                    <el-option :label="op.name" :value="op.value||op.name" v-for="op in $consts.variableTypes" :key="op.name"> </el-option>
                                </el-select>
                            </div>
                            <span class="btn-area">
                                <el-link icon="el-icon-delete" @click="removeVariable(2,index)" :underline="false"></el-link>
                            </span>
                        </div>

                    </td>
                </tr>

            </table>

        </el-form-item>

        <el-form-item label="">
            <el-row>
                <el-col :span="10" class="title-wrap">失败重试</el-col>
                <el-col :span="14" class="right"><el-switch v-model="model.props.RetryConfig.Enabled"> </el-switch></el-col>
            </el-row>

            <template v-if="model.props.RetryConfig.Enabled">
                <el-row>
                    <el-col :span="10">最大重试次数</el-col>
                    <el-col :span="14" class="right">
                        <el-input-number v-model="model.props.RetryConfig.MaxRetryTimes" style="width:100px" size="mini" controls-position="right" :min="1" :max="10"></el-input-number>
                    </el-col>
                </el-row>

                <el-row>
                    <el-col :span="10">重试间隔（毫秒）</el-col>
                    <el-col :span="14" class="right">
                        <el-input-number v-model="model.props.RetryConfig.Interval" style="width: 100px" size="mini" controls-position="right" :min="0"></el-input-number>
                    </el-col>
                </el-row>
            </template>
        </el-form-item>



    </div>
</template>

<script>
    import VariableEditor from './VariableEditor.vue'
    export default {
        name: 'ScriptNodePropEditor',
        components: {
            VariableEditor
        },
        props: {
            model: Object,
            lf:null
        },
        data() {
            return {

            }
        },

        watch: {


        },
        computed: {
            nodeModel() {
                return this.lf.getNodeModelById(this.model.id);
            }
        },
        mounted() {
            this.model.props.Code = this.model.props.Code ||`public override Task<object> ExecuteAsync(dynamic context)\n{\n  return null;\n}`
        },
        methods: {
            propsChange() {
                this.$eventHub.$emit('node-prop-change');

            },
            removeVariable(type, index) {
                if (type == 1) {
                    this.model.props.Variables.splice(index, 1);
                } else {

                    this.model.props.Outputs.splice(index, 1);
                }
                this.propsChange();
            },
            addVariable(type) {

                if (type == 1) {
                    this.model.props.Variables.push({ Name: "", Value: "" });
                } else {

                    this.model.props.Outputs.push({ Name: "", Value: "" });
                }
                this.propsChange();
            },






        }


    }</script>

<style scoped>

    .title-wrap {
        height: 30px;
        line-height: 30px;
        font-weight: bold;
        color: #606266;
        position:relative;
    }

    /deep/ .title-wrap .el-link {
        font-size:14px;
        margin-left:50px;
    
    }
        
        
        /deep/ .table .el-input__inner {
            border: none !important;
        }

    .wrap .var-right {
        flex: 1;
    }

    .right {
    
      text-align:right;
    }

    .wrap {
        display: flex;
        flex-direction: row;
    }

    .btn-area {
        width: 30px;
        text-align: center;
    }


    /deep/ .table .editor {
        border: none;
    }
</style>

<style scoped src="../PropEditor.css"></style>