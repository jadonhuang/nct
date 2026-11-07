<template>
  <el-dialog title="导入数据表" :visible.sync="isShow" :close-on-click-modal="false" width="500px">
      <el-form :model="model" label-width="100px" :rules="rules" ref="form" size="medium">
          <el-form-item label="表名称：" prop="Name">
              <el-input v-model="model.Name"></el-input>
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
        mixins: [crud({ api: window.$dbTableApi,add:'import' })],
    data() {

      return {
        rules: {
       
          Name: [
            { required: true, message: '请输入名称', trigger: 'blur' }
           
          ],
        


        },
    
      


      }
    },
    watch: {
      isShow: function (v) {
        if (!v)
          this.$refs.form.clearValidate();
      },


    },
    methods: {

    
    
    },
    components: {  }


  }
</script>



