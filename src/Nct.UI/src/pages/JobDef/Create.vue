

<template>
    <el-dialog title="新增" :visible.sync="isShow"  :close-on-click-modal="false" width="600px">
        <el-form :model="model" label-width="120px" :rules="rules" ref="form" size="medium">

            <el-row>
                <el-col :span="24">
                    <el-form-item label="任务名称：" prop="Name">
                        <el-input v-model="model.Name"></el-input>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row>
                <el-col :span="24">
                    <el-form-item label="任务类型：" prop="JobType" required>
                        <el-radio-group v-model="model.JobType">
                            <el-radio :label="1">自定义流程</el-radio>
                            <el-radio :label="2">系统方法</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>

            </el-row>
            <el-row>
                <el-col :span="24">
                    <el-form-item label="是否启用：" prop="IsEnabled" required>
                        <el-switch v-model="model.IsEnabled">
                        </el-switch>
                    </el-form-item>
                </el-col>

            </el-row>
            <el-row v-if="model.JobType==2">
                <el-col :span="24">
                    <el-form-item label="执行方法：" prop="ExeMethod" required>
                        <el-input v-model="model.ExeMethod" placeholder="类名全称+方法名,eg:Nct.Application.Flow.TestApp.Run"></el-input>
                    </el-form-item>
                </el-col>

            </el-row>


            <el-row>
                <el-col :span="24">
                    <el-form-item label="执行方式：" prop="IntervalType" required>
                        <el-radio-group v-model="model.IntervalType">
                            <el-radio :label="1">时间间隔</el-radio>
                            <el-radio :label="2">Cron表达式</el-radio>

                        </el-radio-group>
                    </el-form-item>
                </el-col>
            </el-row>


            <el-row>
                <el-col :span="24" v-if="model.IntervalType==1">
                    <el-form-item label="运行间隔：" prop="IntervalMinute" required>

                        每隔 <el-input-number v-model="model.IntervalMinute" controls-position="right" :min="1" :max="10000"></el-input-number>分钟
                    </el-form-item>
                </el-col>
                <el-col :span="24" v-else>
                    <el-form-item label="Cron表达式：" prop="IntervalCron" required>
                        <el-input v-model="model.IntervalCron"></el-input>
                    </el-form-item>
                </el-col>
            </el-row>
        </el-form>
        <div slot="footer">

            <el-button type="primary" @click="addRow" :loading="loading" size="small" icon="el-icon-circle-check">提交</el-button>
            <el-button @click="isShow=false" size="small" icon="el-icon-circle-close">取消</el-button>

        </div>


    </el-dialog>
</template>
<script>

    import crud from '@/mixins/crud.js'

    export default {
        components: {

        },
        mixins: [crud({ api: window.$jobDefApi })],

        watch: {
            isShow: function (v) {
                if (!v)
                    this.$refs.form.clearValidate();

            },

        },

        data() {


            var validateIntervalMinute = (rule, value, callback) => {
                if (this.model.IntervalType == 1 && !this.model.IntervalMinute) {
                    callback(new Error('请选择运行间隔'));
                } else {
                    callback();
                }



            };

            var validateIntervalCron = (rule, value, callback) => {
                if (this.model.IntervalType == 2 && !this.model.IntervalCron) {
                    callback(new Error('请输入Cron表达式'));
                } else {
                    callback();
                }



            };
               
            var validateExeMethod = (rule, value, callback) => {
                if (this.model.JobType == 2 && !this.model.ExeMethod) {
                    callback(new Error('请输入执行方法名'));
                } else {
                    callback();
                }



            };



            return {
                emptyModel: {
                    Name: '',
                    IntervalMinute: 10,
                    IntervalType: 1,
                    JobType: 1,
                    IsEnabled: true,
                },
                rules: {

                    Name: [
                        { required: true, message: '请输入名称', trigger: 'blur' }

                    ],

                    IntervalMinute: [
                        { validator: validateIntervalMinute }

                    ],
                    IntervalCron: [
                        { validator: validateIntervalCron }

                    ],
                    ExeMethod: [
                        { validator: validateExeMethod }

                    ]

                },

            }
        },
        methods: {

        },
        created() {
            
        },


    }
</script>



