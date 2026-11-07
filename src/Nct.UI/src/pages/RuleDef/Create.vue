<template>
    <el-dialog title="新增规则" :visible.sync="isShow" :close-on-click-modal="false">
        <el-form :model="model" label-width="100px" :rules="rules" ref="form" size="medium">
            <el-row>
                <el-col :span="24">
                    <el-form-item label="名称：" prop="Name">
                        <el-input v-model="model.Name"></el-input>
                    </el-form-item>
                </el-col>

            </el-row>

            <el-row>
                <el-col :span="12">
                    <el-form-item label="类型：" prop="Type">
                        <el-radio-group v-model="model.Type">
                            <el-radio :label="1">Js</el-radio>
                            <el-radio :label="2">C#</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <el-form-item label="类别：" prop="Cate">
                        <DicValueSelect v-model="model.Cate" parentCode="rulecate"></DicValueSelect>
                    </el-form-item>
                </el-col>

            </el-row>




            <el-form-item label="脚本：" prop="Data">
                <div style="height:200px;">
                    <MonacoEditor class="nct-monaco-editor " v-model="model.Data" :options="options" :language="model.Type==1?'javascript':'csharp'" theme="vs" />
                </div>
            </el-form-item>


            <el-form-item label="说明：" prop="Descpt">
                <el-input type="textarea" v-model="model.Descpt"></el-input>
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
        mixins: [crud({ api: window.$ruleDefApi })],
        data() {

            return {
                emptyModel: {
                    Name: '',
                    Type:1,
                    Descpt: '',
                    Data: '',
                    Cate:'',

                },
                options: { minimap: { enabled: false }, automaticLayout: true },
              
                rules: {

                    Name: [
                        { required: true, message: '请输入名称', trigger: 'blur' }

                    ],
                    Type: [
                        { required: true, message: '请选择类型', trigger: 'change' }

                    ],
                    Cate: [
                        { required: true, message: '请选择类别', trigger: 'change' }

                    ],
                    Data: [
                        { required: true, message: '请输入脚本', trigger: 'blur' }

                    ],


                },




            }
        },
        methods: {
                 

        },
        watch: {
            isShow: function (v) {
                if (this.$refs.form)
                    this.$refs.form.clearValidate();
                
            },


        },
        async created() {

        }



    }
</script>



