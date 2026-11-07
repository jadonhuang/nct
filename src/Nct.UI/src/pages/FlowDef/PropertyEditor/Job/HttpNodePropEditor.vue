<template>
    <div>
        <el-form-item label="API">

            <div class="api-set">
                <div class="api-method">
                    <el-select v-model="model.props.Method" @change="propsChange">
                        <el-option label="GET" value="GET"> </el-option>
                        <el-option label="POST" value="POST"> </el-option>
                        <el-option label="PUT" value="PUT"> </el-option>
                        <el-option label="DELETE" value="DELETE"> </el-option>
                    </el-select>
                </div>
                <div class="api-url">
                    <VariableEditor :minHeight="32" @input="propsChange" :allowTypes="simpleVarTypes" :node="nodeModel" v-model="model.props.Url" tip="接口地址，输入'/'快速插入变量"></VariableEditor>
                </div>
            </div>
        </el-form-item>

        <el-form-item label="Headers">
            <table class="table">
                <tr>
                    <th style="width:40%">键</th>
                    <th>值</th>
                </tr>

                <tr v-for="(item,index) in model.props.Headers" :key="index+'h'">
                    <td>
                        <VariableEditor :minHeight="25" :allowTypes="['String']" :node="nodeModel" v-model="item.Name" @input="propsChange"></VariableEditor>
                    </td>
                    <td class="value_td">
                        <VariableEditor :minHeight="25"  :allowTypes="simpleVarTypes" :node="nodeModel" v-model="item.Value" @click="handleHeaderValueClick(index)" @input="propsChange"></VariableEditor>

                        <div class="btn-area">
                            <el-link icon="el-icon-delete" @click="removeHeader(index)" :underline="false"></el-link>
                        </div>


                    </td>
                </tr>

            </table>

        </el-form-item>
        <el-form-item label="Params">
            <table class="table">
                <tr>
                    <th style="width:40%">键</th>
                    <th>值</th>
                </tr>

                <tr v-for="(item,index) in model.props.Params" :key="index+'p'">
                    <td>
                        <VariableEditor :minHeight="25"  :allowTypes="simpleVarTypes" :node="nodeModel" v-model="item.Name" @input="propsChange"></VariableEditor>
                    </td>
                    <td class="value_td">
                        <VariableEditor :minHeight="25"  :allowTypes="simpleVarTypes" :node="nodeModel" v-model="item.Value" @click="handleParamValueClick(index)" @input="propsChange"></VariableEditor>
                        <div class="btn-area">
                            <el-link icon="el-icon-delete" @click="removeParam(index)" :underline="false"></el-link>
                        </div>
                    </td>
                </tr>

            </table>

        </el-form-item>


        <el-form-item label="Body">

            <div style="margin-bottom:10px;">
                <el-radio-group v-model="model.props.Body.Type" @input="handleBodyTypeChange">
                    <el-radio label="none">none</el-radio>
                    <el-radio label="x-www-form-urlencoded">x-www-form-urlencoded</el-radio>
                    <el-radio label="json">json</el-radio>
                    <!--<el-radio label="raw">raw</el-radio>-->
                </el-radio-group>
            </div>

            <table class="table" v-if="model.props.Body.Type=='x-www-form-urlencoded'">
                <tr>
                    <th style="width:40%">键</th>
                    <th>值</th>
                </tr>

                <tr v-for="(item,index) in model.props.Body.Data" :key="index+'b'">
                    <td>
                        <VariableEditor :minHeight="25"  :allowTypes="simpleVarTypes" :node="nodeModel" v-model="item.Name"></VariableEditor>
                    </td>
                    <td class="value_td">
                        <VariableEditor :minHeight="25"  :allowTypes="simpleVarTypes" :node="nodeModel" v-model="item.Value" @click="handleBodyDataClick(index)"></VariableEditor>
                        <div class="btn-area">
                            <el-link icon="el-icon-delete" @click="removeBodyData(index)" :underline="false"></el-link>
                        </div>
                    </td>
                </tr>

            </table>
            <VariableEditor tip="body数据，输入'/'快速插入变量"  :allowTypes="simpleVarTypes" :minHeight="50" v-else-if="model.props.Body.Type!='none'" :node="nodeModel" v-model="item.Value" v-for="(item,index) in model.props.Body.Data" :key="index+'var'"></VariableEditor>

        </el-form-item>


        <el-collapse>
            <el-collapse-item title="输出变量">
                <div> 响应内容：body(string) </div>
                <div> 响应状态码：status_code(number) </div>
                <div> 响应头：headers(object) </div>
                <div> 文件：file(string)  </div>
            </el-collapse-item>

        </el-collapse>
        <el-form-item label="">

            <el-row>
                <el-col :span="12">失败重试</el-col>
                <el-col :span="12" class="right"><el-switch v-model="model.props.RetryConfig.Enabled"> </el-switch></el-col>
            </el-row>

            <template v-if="model.props.RetryConfig.Enabled">
                <el-row>
                    <el-col :span="12">最大重试次数</el-col>
                    <el-col :span="12" class="right">
                        <el-input-number v-model="model.props.RetryConfig.MaxRetryTimes" style="width:100px" size="mini" controls-position="right" :min="1" :max="10"></el-input-number>
                    </el-col>
                </el-row>

                <el-row>
                    <el-col :span="12">重试间隔（毫秒）</el-col>
                    <el-col :span="12" class="right">
                        <el-input-number v-model="model.props.RetryConfig.Interval" style="width: 100px" size="mini" controls-position="right" :min="0"></el-input-number>
                    </el-col>
                </el-row>
            </template>
            <el-row>
                <el-col :span="12">超时设置(秒)</el-col>
                <el-col :span="12" class="right">
                    <el-input-number v-model="model.props.Timeout" style="width: 100px" size="mini" controls-position="right"></el-input-number>
                </el-col>
            </el-row>

        </el-form-item>

       







    </div>
</template>

<script>
    import VariableEditor from './VariableEditor.vue'
    export default {
        name: 'HttpNodePropEditor',
        components: {
            VariableEditor
        },
        props: {

            model: Object,
            lf: null

        },
        data() {
            return {
                 simpleVarTypes:['String','Number']
                
            }
        },

        watch: {
           


        },

        computed: {
            nodeModel() {
                return this.lf.getNodeModelById(this.model.id);
            }
        },



        methods: {

            propsChange() {
                this.$eventHub.$emit('node-prop-change');

            },

     
            handleBodyTypeChange() {

                this.model.props.Body.Data = [];
                this.handleBodyDataClick(-1);
                this.propsChange();
            },

            removeHeader(index) {
                this.model.props.Headers.splice(index, 1);
                this.handleHeaderValueClick(-1);
                this.propsChange();

            },
            handleHeaderValueClick(index) {
                if (index == this.model.props.Headers.length - 1) {
                    this.model.props.Headers.push({ Name: "", Value: "" });
                    this.propsChange();
                }


            },


            removeParam(index) {
                this.model.props.Params.splice(index, 1);
                this.handleParamValueClick(-1);
                this.propsChange();

            },
            handleParamValueClick(index) {
                if (index == this.model.props.Params.length - 1) {
                    this.model.props.Params.push({ Name: "", Value: "" });
                    this.propsChange();
                }


            },


            removeBodyData(index) {
                this.model.props.Body.Data.splice(index, 1);
                this.handleBodyDataClick(-1);
                this.propsChange();

            },
            handleBodyDataClick(index) {
                if (index == this.model.props.Body.Data.length - 1) {
                    this.model.props.Body.Data.push({ Name: "", Value: "" });
                    this.propsChange();
                }


            },


        


        }


    }</script>

<style scoped>

    .right {
      text-align:right;
    
    }

    .value_td {
        position: relative;
    }

        .value_td:hover .btn-area {
            display: block;
        }

    .btn-area {
        position: absolute;
        right: 10px;
        top: 0;
        display: none;
    }

   

    /deep/ .table .editor {
        border: none;
    }

    .api-set {
        display: flex;
    }

    .api-method {
        width: 75px;
    }

    .api-url {
        padding-left: 5px;
        flex: 1;
    }
</style>
<style scoped src="../PropEditor.css"></style>
