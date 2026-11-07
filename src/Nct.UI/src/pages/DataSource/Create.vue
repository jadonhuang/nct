<template>
    <el-dialog title="新增数据源" :visible.sync="isShow" resizeable="N" :close-on-click-modal="false" width="65%">
        <el-form :model="model" label-width="100px" :rules="rules" ref="form" size="medium">
            <el-row>
                <el-col :span="12">
                    <el-form-item label="名称：" prop="Name">
                        <el-input v-model="model.Name"></el-input>
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <el-form-item label="编码：" prop="Code">
                        <el-input v-model="model.Code"></el-input>
                    </el-form-item>
                </el-col>
            </el-row>


            <el-form-item label="类型：" prop="Type">
                <el-radio-group v-model="model.Type">
                    <el-radio :label="1">Sql</el-radio>
                    <el-radio :label="2">C#</el-radio>
                </el-radio-group>
            </el-form-item>

            <el-form-item label="脚本：" prop="Data" v-if="model.Data!==undefined">
                <div style="height:340px;">
                    <MonacoEditor class="nct-monaco-editor" v-model="model.Data" :options="options" :language="model.Type==1?'sql':'csharp'" theme="vs" />
                </div>
            </el-form-item>


            <el-form-item label="说明：" prop="Descpt">
                <el-input  v-model="model.Descpt"></el-input>
            </el-form-item>


        </el-form>
        <div slot="footer">

            <el-button type="primary" @click="addRow" :loading="loading" size="small" icon="el-icon-circle-check">提交</el-button>
            <el-button @click="isShow=false" size="small" icon="el-icon-circle-close">取消</el-button>

        </div>


    </el-dialog>
</template>
<script>

    import crud from '@/mixins/crud.js'
    import MonacoEditor from 'vue-monaco'
 
    export default {
        components: {
            MonacoEditor
        },
        mixins: [crud({ api: window.$dataSourceApi })],
        data() {

            return {
                options: { minimap: { enabled: false }, automaticLayout: true },
                emptyModel: {
                    Type: 1,
                    Data: '',
                },
                rules: {

                    Name: [
                        { required: true, message: '请输入名称', trigger: 'blur' },
                        {
                            validator: function (rule, value, callback, source, options) {
                                callback();
                              

                            }
                        }

                    ],
                    Type: [
                        { required: true, message: '请选择类型', trigger: 'blur' }

                    ],
                    Code: [
                        { required: true, message: '请输入编码', trigger: 'blur' }

                    ],
                    Data: [
                        { required: true, message: '请输入脚本', trigger: 'blur' }

                    ],


                },




            }
        },
        methods: {

        },
        created() {

        },


    }
</script>

<style scoped>

</style>



