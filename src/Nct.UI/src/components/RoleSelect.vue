<template>

    <el-dialog title="角色" :visible.sync="isShow" :close-on-click-modal="false" width="500px" append-to-body>

        <el-select v-model="roleIds" multiple filterable clearable placeholder="请选择" class="w100">
            <el-option v-for="item in roles"
                       :key="item.Id"
                       :label="item.Name"
                       :value="item.Id">
            </el-option>
        </el-select>
        <span slot="footer">

            <el-button type="primary" @click.native="change" size="small" icon="el-icon-circle-check">确认</el-button>
            <el-button @click.native="isShow=false" size="small" icon="el-icon-circle-close">取消</el-button>

        </span>

    </el-dialog>
</template>
<script>


    export default {

        data() {
            return {
                isShow: false,
                roleIds: [],
                roles: [],
            }
        },
        computed: {
          

        },
        watch: {

        },
        methods: {
            show(ids) {
                this.isShow = true;
                this.roleIds = ids || [];
                this.getRoles();

            },
            async getRoles() {
                let data = await $roleApi.getAll();
                this.roles = data.Data;
            },
            change() {

                let roles = this.roles.filter(m => this.roleIds.indexOf(m.Id)!=-1);
                this.$emit('change', roles);
                this.isShow = false;
            },


        },
        mounted() {

        }


    }
</script>

