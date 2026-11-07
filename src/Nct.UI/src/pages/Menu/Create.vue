<template>
    <el-dialog title="新增菜单" :visible.sync="isShow" :close-on-click-modal="false">
        <el-form :model="model" label-width="100px" :rules="rules" ref="form" size="medium">

            <el-form-item label="菜单名称：" prop="Name">
                <el-input v-model="model.Name"></el-input>
            </el-form-item>

            <el-form-item label="菜单编码：" prop="Code">
                <el-input v-model="model.Code"></el-input>
            </el-form-item>

            <el-row>
                <el-col :span="12">
                    <el-form-item label="是否启用：" prop="IsShow">
                        <el-radio-group v-model="model.IsShow">
                            <el-radio :label="true">启用</el-radio>
                            <el-radio :label="false">禁用</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <el-form-item label="菜单类型：" prop="IsMenu">
                        <el-radio-group v-model="model.IsMenu">
                            <el-radio :label="true">菜单</el-radio>
                            <el-radio :label="false">按钮</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>

            </el-row>




            <el-form-item label="菜单地址：" prop="Url" v-if="model.IsMenu">
                <el-input v-model="model.Url"></el-input>
            </el-form-item>


            <el-form-item label="菜单图标：" prop="Icon" v-if="model.IsMenu">
                <IconSelect v-model="model.Icon" type="input"></IconSelect>
            </el-form-item>

            <el-form-item label="菜单序号：" prop="Sort">
                <el-input v-model="model.Sort"></el-input>
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

    export default {
        mixins: [crud({ api: window.$menuApi })],
        data() {

            return {
                pid: 0,
                emptyModel: {
                    IsMenu: true,
                    IsShow: true,
                    Icon: '',
                },
                rules: {

                    Name: [
                        { required: true, message: '请输入名称', trigger: 'blur' }

                    ],
                    Code: [
                        { required: true, message: '请输入编码', trigger: 'blur' }

                    ],
                    IsMenu: [
                        { required: true, message: '请选择类型', trigger: 'blur' }

                    ],
                    IsShow: [
                        { required: true, message: '请选择状态', trigger: 'blur' }

                    ],

                },




            }
        },
        methods: {
            showIconSelect() {

                this.$refs.iconSelect.show();
            },

            iconChange(icon) {
                this.model.Icon = icon;
            },
            setPid(pid) {
                this.pid = pid;
            },
            addBefore() {
                this.model.ParentId = this.pid;

            }

        },
        components: {}


    }
</script>



