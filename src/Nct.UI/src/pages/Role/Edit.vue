<template>
  <el-dialog title="编辑角色" :visible.sync="isShow" :close-on-click-modal="false" >
    <el-tabs v-model="selectTab">
      <el-tab-pane label="基本信息" name="info">
     
        <el-form :model="model" label-width="100px" :rules="rules" ref="form" size="medium"  v-loading="loading">
          <el-form-item label="角色编号：" prop="Code">
            <el-input v-model="model.Code" auto-complete="off"></el-input>
          </el-form-item>
          <el-form-item label="角色名称：" prop="Name">
            <el-input v-model="model.Name" auto-complete="off"></el-input>
          </el-form-item>

          <el-form-item label="角色描述：" prop="Descpt">
            <el-input v-model="model.Descpt" auto-complete="off" type="textarea"></el-input>
          </el-form-item>


        </el-form>
      </el-tab-pane>
      <el-tab-pane label="权限设置" name="rgt">

        <Permission  ref="editForm" :value="model.Permissions" :isShow="isShow" @changed="pmsChange"></Permission>

      </el-tab-pane>
    </el-tabs>

    <div slot="footer">

      <el-button type="primary" @click.native="updateRow" :loading="loading" size="small" icon="el-icon-circle-check">提交</el-button>
      <el-button @click.native="isShow=false" size="small" icon="el-icon-circle-close">取消</el-button>

    </div>

  </el-dialog>
</template>
<script>

  import Permission from './Permission.vue';
  import crud from '@/mixins/crud.js'

  export default {
        mixins: [crud({ api: window.$roleApi  })],
    data() {
      return {

        rules: {
          Name: [
            { required: true, message: '请输入角色名称', trigger: 'blur' },
            { min: 2, max: 20, message: '长度在 2 到 20 个字符', trigger: 'blur' }
          ],

          Code: [
            { required: true, message: '请输入角色编号', trigger: 'blur' },
            { min: 2, max: 20, message: '长度在 2 到 20 个字符', trigger: 'blur' }
          ],



        },
        selectTab: 'info',       
        emptyModel: {
          Name: '',
          Code: '',
          Descpt: '',
          Permissions: ''

        }



      }
    },
    watch: {
      isShow: function (v) {
     
        this.selectTab = 'info';
      
      },


    },
    methods: {
      pmsChange: function (value) {

        this.model.Permissions = value;
      },


    },

    components: { Permission }

  }
</script>
