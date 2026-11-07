<template>
    <el-dialog title="新增部门" :visible.sync="isShow" :close-on-click-modal="false" width="500px">
        <el-form :model="model" label-width="100px" :rules="rules" ref="form" size="medium">

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

            <el-button type="primary" @click="addRow" :loading="loading" size="small" icon="el-icon-circle-check">提交</el-button>
            <el-button @click="isShow=false" size="small" icon="el-icon-circle-close">取消</el-button>

        </div>
    </el-dialog>
</template>
<script>

    import crud from '@/mixins/crud.js'

    export default {
        mixins: [crud({ api: window.$orgApi })],
        data() {

            return {
                pid: 0,
                rules: {

                    Name: [
                        { required: true, message: '请输入名称', trigger: 'blur' }

                    ]
                },
                managers: []




            }
        },
        methods: {
            setPid(pid) {
                this.pid = pid;
            },
            addBefore() {
                this.model.ParentId = this.pid;
                this.model.ManagerIds = this.managers.map(m => m.Id);

            },
          
            async selectUsers() {

                let users = await this.$showUserSelect();
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



