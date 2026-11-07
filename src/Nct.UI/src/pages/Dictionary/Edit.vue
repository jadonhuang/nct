<template>
  <el-dialog title="编辑分类" :visible.sync="isShow" :close-on-click-modal="false" width="500px">
    <el-form :model="model" label-width="100px" :rules="rules" ref="form" size="medium"  v-loading="loading">

      <el-form-item label="分类编码：" prop="Code">
        <el-input v-model="model.Code"></el-input>
      </el-form-item>


      <el-form-item label="分类名称：" prop="Name">
        <el-input v-model="model.Name" ></el-input>
      </el-form-item>


      <el-form-item label="分类排序：" prop="Sort">
        <el-input v-model="model.Sort" ></el-input>
      </el-form-item>


      <el-form-item label="分类描述：" prop="Descpt">
        <el-input v-model="model.Descpt"></el-input>
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
        mixins: [crud({ api: window.$dicApi })],
    data() {

      return {

        rules: {

          Name: [
            { required: true, message: '请输入分类名称', trigger: 'blur' }

          ],
          Code: [
            { required: true, message: '请输入分类编码', trigger: 'blur' }

          ],


        },


      }
    },
    methods: {


    },
    components: {}


  }
</script>



