<template>
    <el-dialog title="按钮设置" :visible.sync="isShow" :close-on-click-modal="false" width="800px">
        <el-form :model="model" label-width="120px" :rules="rules" ref="form" size="medium">
            <el-row>
                <el-col :span="12">
                    <el-form-item label="名称：" prop="Name">
                        <el-input v-model="model.Name"></el-input>
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <el-form-item label="图标：" prop="Icon">

                        <IconSelect v-model="model.Icon"></IconSelect>
                    
                    </el-form-item>
                </el-col>
            </el-row>

            <el-row>
                <el-col :span="12">
                    <el-form-item label="目标：" prop="Target">
                        <el-select v-model="model.Target" placeholder="请选择" filterable :disabled="model.IsSys" class="w100">

                            <el-option key="1" label="调用Api" value="api"> </el-option>
                            <el-option key="3" label="打开Tab页面" value="opentab"> </el-option>
                            <el-option key="2" label="打开外部页面" value="openurl"> </el-option>
                            <el-option key="5" label="打开弹窗页面" value="opendialog"> </el-option>
                            <el-option key="4" label="页面跳转" value="redirect"> </el-option>

                        </el-select>

                    </el-form-item>
                </el-col>


                <el-col :span="12">
                    <el-form-item label="排序：" prop="Sort">

                        <el-input-number v-model="model.Sort" controls-position="right" :min="0" class="w100"></el-input-number>

                    </el-form-item>
                </el-col>
            </el-row>
            <el-row v-if="model.Target=='api'">
                <el-col :span="12">
                    <el-form-item label="请求方式：" prop="Method">
                        <el-select v-model="model.Method" placeholder="请选择" class="w100">
                            <el-option key="1" label="Get" value="get"> </el-option>
                            <el-option key="2" label="Post" value="post"> </el-option>
                        </el-select>
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <el-form-item label="参数传递：" prop="ArgTran">
                        <el-select v-model="model.ArgTran" placeholder="请选择" class="w100">
                            <el-option key="1" label="Body" value="body"> </el-option>
                            <el-option key="2" label="Url" value="url"> </el-option>
                        </el-select>

                    </el-form-item>
                </el-col>
            </el-row>

            <el-form-item label="目标地址：" prop="Url" :disabled="model.IsSys">
                <el-input v-model="model.Url"></el-input>
            </el-form-item>


            <el-row>
                <el-col :span="12">
                    <el-form-item label="折叠到更多：" prop="ShowInMore">

                        <el-switch active-text="是" v-model="model.ShowInMore" :active-value="true" :inactive-value="false"
                                   inactive-text="否">
                        </el-switch>

                    </el-form-item>

                </el-col>

                <el-col :span="12">
                    <el-form-item label="按钮类型：" prop="Type">

                        <el-select v-model="model.Type" placeholder="请选择" filterable class="w100">
                            <el-option key="0" label="默认按钮" value="def"> </el-option>
                            <el-option key="1" label="主要按钮" value="primary"> </el-option>
                            <el-option key="2" label="成功按钮" value="success"> </el-option>
                            <el-option key="3" label="信息按钮" value="info"> </el-option>
                            <el-option key="4" label="警告按钮" value="warning"> </el-option>
                            <el-option key="5" label="危险按钮" value="danger"> </el-option>

                        </el-select>

                    </el-form-item>
                </el-col>
            </el-row>
            <el-form-item label="角色设置：" prop="Condition">
                <el-select v-model="model.Roles" placeholder="请选择" filterable class="w100" multiple>
                    <el-option :key="role.Code" :label="role.Name" :value="role.Code" v-for="role in roles"> </el-option>
                </el-select>

            </el-form-item>

            <el-form-item label="前置条件：" prop="Condition">
                <el-input type="textarea" :rows="5" v-model="model.Condition"></el-input>
            </el-form-item>


        </el-form>
        <div slot="footer">

            <el-button type="danger" @click="del" size="small" icon="el-icon-delete" v-if="!(isAdd)">删除</el-button>

            <el-button type="primary" @click="save" size="small" icon="el-icon-circle-check">确定</el-button>
            <el-button @click="isShow=false" size="small" icon="el-icon-circle-close">取消</el-button>
        </div>
    </el-dialog>
</template>

<script>
 
    export default {
        props: {


        },
        data() {
            return {
                roles: [],
                isAdd: false,
                model: {},
                btns: [],
                isShow: false,
                isTop: false,
                rules: {

                    Name: [
                        { required: true, message: '请输入名称', trigger: 'blur' }

                    ],
                    Sort: [
                        { required: true, message: '请输入排序', trigger: 'blur' }

                    ],
                    Target: [
                        { required: true, message: '请选择目标', trigger: 'change' }

                    ],
                    Type: [
                        { required: true, message: '请选择列席', trigger: 'change' }

                    ],


                },
            }
        },
        computed: {

        },
        methods: {

            async getRoles() {

                this.roles = (await $roleApi.getAll()).Data;
            },

            iconChange(icon) {
                this.model.Icon = icon;
            },
            showIconSelect() {
                this.$refs.iconSelect.show();
            },
            show(btn, btns, isTop) {
                this.btns = btns;
                this.isAdd = !btn;
                if (this.isAdd) {
                    btn = { Name: '', IsShow: true, Icon: '', Method: "post", ArgTran: "body", Type: 'def', Target: "api", ShowInMore: false, Url: "", Sort: btns.length + 1, Roles: [], IsSys: false, Id: this.$newId() };

                }

                if (btn.Type === undefined)
                    btn.Type = 'def';

                if (btn.Roles === undefined)
                    btn.Roles = [];

                this.model = this.$copyObj(btn);
                this.isShow = true;
                this.isTop = isTop;

            },
            del() {
                var index = this.btns.findIndex(m => m.Id == this.model.Id);
                this.btns.splice(index, 1);
                this.isShow = false;
            },
            save() {
                this.$refs['form'].validate(valid => {
                    if (valid) {
                        this.$emit("change", { model: this.model, isAdd: this.isAdd });

                        if (this.model.Target != 'api') {
                            delete this.model.Method;
                            delete this.model.ArgTran;
                        }

                        if (this.isAdd) {
                            this.btns.push(this.model)

                        } else {

                            var btn = this.btns.find(m => m.Id == this.model.Id);
                            Object.assign(btn, this.model);
                        }
                        this.btns.sort((a, b) => { return a.Sort - b.Sort });
                        this.isShow = false;

                    }
                })

            },
        },
        mounted() {
            this.getRoles();
        },

    }</script>

