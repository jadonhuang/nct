<template>
    <div>
        <el-form-item label="模型">
            <el-select v-model="model.props.ModelType"  filterable allow-create @change="propsChange" style="width: 100%;" >
                <el-option :label="item.Name" :value="item.Value" v-for="(item,index) in nodeModel.getModels()" :key="index"> </el-option>
               
                
            </el-select>
        </el-form-item>

        <el-form-item label="API Key">
            <VariableEditor :minHeight="32" @input="propsChange" :allowTypes="['String']" :node="nodeModel" v-model="model.props.ApiKey" tip="API Key，输入'/'快速插入变量"></VariableEditor>
        </el-form-item>

        <el-form-item label="API 地址">
            <VariableEditor :minHeight="32" @input="propsChange" :allowTypes="['String']" :node="nodeModel" v-model="model.props.ApiUrl" tip="API 地址（可选，用于自定义模型），输入'/'快速插入变量"></VariableEditor>
        </el-form-item>

        <el-form-item label="提示词">
            <VariableEditor tip="提示词，输入'/'快速插入变量" :allowTypes="simpleVarTypes" :minHeight="100" :node="nodeModel" v-model="model.props.Prompt" @input="propsChange"></VariableEditor>
        </el-form-item>

        <el-form-item label="Temperature">
            <el-slider v-model="model.props.Temperature" :min="0" :max="2" :step="0.1" @change="propsChange"></el-slider>
            <div style="text-align: right; color: #999; font-size: 12px;">{{ model.props.Temperature }}</div>
        </el-form-item>

        <el-form-item label="最大 Tokens">
            <el-input-number v-model="model.props.MaxTokens" style="width: 100%" :min="1" :max="128000" @change="propsChange"></el-input-number>
        </el-form-item>


        <el-collapse>
            <el-collapse-item title="输出变量">
                <div> AI 响应内容：text(string) </div>
            </el-collapse-item>
        </el-collapse>

        <el-form-item label="">
            <el-row>
                <el-col :span="12">失败重试</el-col>
                <el-col :span="12" class="right"><el-switch v-model="model.props.RetryConfig.Enabled" @change="propsChange"> </el-switch></el-col>
            </el-row>

            <template v-if="model.props.RetryConfig.Enabled">
                <el-row>
                    <el-col :span="12">最大重试次数</el-col>
                    <el-col :span="12" class="right">
                        <el-input-number v-model="model.props.RetryConfig.MaxRetryTimes" style="width:100px" size="mini" controls-position="right" :min="1" :max="10" @change="propsChange"></el-input-number>
                    </el-col>
                </el-row>

                <el-row>
                    <el-col :span="12">重试间隔（毫秒）</el-col>
                    <el-col :span="12" class="right">
                        <el-input-number v-model="model.props.RetryConfig.Interval" style="width: 100px" size="mini" controls-position="right" :min="0" @change="propsChange"></el-input-number>
                    </el-col>
                </el-row>
            </template>
        </el-form-item>
    </div>
</template>

<script>
    import VariableEditor from './VariableEditor.vue'
    export default {
        name: 'AiModelNodePropEditor',
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

        computed: {
            nodeModel() {
                return this.lf.getNodeModelById(this.model.id);
            }
        },

        methods: {

            propsChange() {
                this.$eventHub.$emit('node-prop-change');
            },

        }


    }
</script>

<style scoped>
    .right {
      text-align:right;
    }
</style>
<style scoped src="../PropEditor.css"></style>
