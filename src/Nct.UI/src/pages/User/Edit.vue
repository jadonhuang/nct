<template>

    <section class="nct-work-area no-padding" v-loading="loading">
        <div class="form-container">
            <el-form :model="model" label-width="100px" style="padding: 20px" :rules="rules" ref="form" size="medium">
                <el-row>
                    <el-col :span="12" class="header">
                        <el-form-item label="用户名：" prop="UserName">

                            <el-input v-model="model.UserName" disabled></el-input>
                        </el-form-item>
                    </el-col>

                    <el-col :span="12">
                        <el-form-item label="用户密码：" prop="Password">
                            <el-input type="password" v-model="model.Password" auto-complete="off" placeholder="不修改则保留空"></el-input>
                        </el-form-item>
                    </el-col>

                </el-row>

                <el-row>

                    <el-col :span="12" class="header">
                        <el-form-item label="用户姓名：" prop="RealName">
                            <el-input v-model="model.RealName" auto-complete="off"></el-input>
                        </el-form-item>
                    </el-col>
                    <el-col :span="12">
                        <el-form-item label="用户性别：" prop="Sex">
                            <el-radio-group v-model="model.Sex">
                                <el-radio class="radio" :label="1">男</el-radio>
                                <el-radio class="radio" :label="2">女</el-radio>
                            </el-radio-group>
                        </el-form-item>
                    </el-col>

                </el-row>

                <el-row>

                    <el-col :span="12">
                        <el-form-item label="所属角色：" prop="RoleIds">

                            <el-select v-model="model.RoleIds" multiple filterable clearable placeholder="请选择" class="w100">
                                <el-option v-for="item in roles"
                                           :key="item.Id"
                                           :label="item.Name"
                                           :value="item.Id">
                                </el-option>
                            </el-select>

                        </el-form-item>
                    </el-col>
                    <el-col :span="12">
                        <el-form-item label="所属部门：" prop="OrgId">
                            <el-cascader placeholder="请选择" v-model="model.OrgId" class="w100"
                                         :options="orgs" :props="props"
                                         filterable clearable></el-cascader>


                        </el-form-item>

                    </el-col>


                </el-row>

                <el-row>

                    <el-col :span="12">
                        <el-form-item label="出生年月：" prop="Birthday">
                            <el-date-picker type="date" style="width:100%" placeholder="选择日期" v-model="model.Birthday"></el-date-picker>
                        </el-form-item>
                    </el-col>
                    <el-col :span="12">
                        <el-form-item label="状态：" prop="IsActive">
                            <el-radio-group v-model="model.IsActive">
                                <el-radio :label="true">正常</el-radio>
                                <el-radio :label="false">禁用</el-radio>


                            </el-radio-group>
                        </el-form-item>

                    </el-col>


                </el-row>

                <el-row>

                    <el-col :span="12">
                        <el-form-item label="联系电话：" prop="RoleIds">

                            <el-input v-model="model.Tel"></el-input>
                        </el-form-item>


                    </el-col>
                    <el-col :span="12">
                        <el-form-item label="联系地址：" prop="Address">
                            <el-input v-model="model.Address"></el-input>
                        </el-form-item>

                    </el-col>

                </el-row>




                <el-form-item label="用户头像：" prop="Pic">
                    <ImgUpload :value="model.Pic" @success="uploadSuccess" :limitSize="200"></ImgUpload>
                </el-form-item>

            </el-form>

        </div>
        <div class="el-dialog__footer">

            <el-button type="primary" @click.native="updateRow" :loading="loading" size="small" icon="el-icon-circle-check">提交</el-button>
            <el-button @click.native="$closeDialog()" size="small" icon="el-icon-circle-close">取消</el-button>

        </div>
    </section>

    
</template>
<script>


    import crud from '@/mixins/crud.js'
    import emitter from '@/mixins/emitter';
    export default {
        mixins: [crud({ api: window.$userApi, get: 'getEditInfo' }), emitter],

        data() {
            return {
                roles: [],
                orgs:[],
                rules: {
                    UserName: [
                        { required: true, message: '请输入用户名', trigger: 'blur' },
                        { min: 2, max: 20, message: '长度在 2 到 20 个字符', trigger: 'blur' }
                    ],
                    Sex: [
                        { required: false, message: '请选择性别', trigger: 'blur' },

                    ],
                    RealName: [
                        { required: true, message: '请输入姓名', trigger: 'blur' },
                        { max: 20, message: '最多 20 个字符', trigger: 'blur' }
                    ],
                    Password: [
                        /*   { required: true, message: '请输入密码', trigger: 'blur' },*/
                        { min: 6, max: 20, message: '长度在 6 到 20 个字符', trigger: 'blur' }
                    ],
                    //Birthday: [
                    //  { type: 'date', required: true, message: '请选择生日', trigger: 'change' }
                    //],
                    //Address: [
                    //  { required: true, message: '请输入联系地址', trigger: 'blur' }
                    //]


                },
                props: {
                    multiple: false,
                    'checkStrictly': true,
                    'emitPath': false, 'label': 'Name',
                    'value': 'Id', 'children': 'Children', 'expandTrigger': 'click'
                }

            }
        },
      
        methods: {

            uploadSuccess: function (img) {
                this.model.Pic = img;
            },
            processData(data) {
                this.$removeEmptyChildren(data.Orgs);
                this.roles = data.Roles;
                this.orgs = data.Orgs;
                var user = data.User;
                user.RoleIds = data.UserRoleIds;
                if (data.UserOrgIds && data.UserOrgIds.length>0)
                    user.OrgId = data.UserOrgIds[0];
                return user;

            },
            updateAfter(res) {
                this.dispatch('FormList', 'list.reload', [this]);
                this.$closeDialog();
            }

        },
        created() {
            var id = this.$route.query.id;
            this.getRow(id);
        }
       

    }
</script>


