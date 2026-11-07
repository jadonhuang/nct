<template>
    <el-dialog title="编辑部门" :visible.sync="isShow" :close-on-click-modal="false" width="500px">
        <el-form :model="model" label-width="100px" :rules="rules" ref="form" size="medium" v-loading="loading">

            <el-form-item label="部门名称：" prop="Name">
                <el-input v-model="model.Name"></el-input>
            </el-form-item>



            <el-form-item label="部门描述：" prop="Descpt">
                <el-input v-model="model.Descpt"></el-input>
            </el-form-item>


            <el-form-item label="部门领导：" prop="ManagerIds">
                <div>
                    <el-link icon="el-icon-edit" @click="selectUsers">选择用户</el-link>
                </div>
                <div>
                    <el-tag v-for="user in managers" style="margin-right:5px;" disable-transitions
                            :key="user.Id"
                            @close="removeMgr"
                            closable>
                        {{user.RealName||user.UserName}}
                    </el-tag>

                </div>

            </el-form-item>


        </el-form>
        <div slot="footer">

            <el-button type="primary" @click="updateRow" :loading="loading" size="small" icon="el-icon-circle-check">提交</el-button>
            <el-button @click="isShow=false" size="small" icon="el-icon-circle-close">取消</el-button>

        </div>
    </el-dialog>
</template>
<script>

    import crud from '@/mixins/crud.js'

    export default {
        mixins: [crud({ api: window.$orgApi, get: 'getEditInfo' })],
        data() {

            return {

                rules: {

                    Name: [
                        { required: true, message: '请输入名称', trigger: 'blur' }

                    ],



                },
                managers:[]


            }
        },
        methods: {
            processData(data) {

                this.managers = data.Managers;
                let dto = data;
                delete dto.Managers;
                dto.ManagerIds = this.managers.map(m => m.Id);
                return dto;

            },
            updateBefore() {
                this.model.ManagerIds = this.managers.map(m => m.Id);
            },
            async selectUsers() {
                let options = { selectedIds: this.model.ManagerIds }
                let users = await this.$showUserSelect(options);
                if (users) {
                    this.managers = users;
                }
            },
            removeMgr(user) {

                var index = this.managers.findIndex(m => m.Id == user.Id);              
                this.managers.splice(index, 1);
          
            }

        },
        components: {}


    }
</script>



