<template>
  <el-dialog title="编辑字段类型" :visible.sync="isShow" :close-on-click-modal="false">
    <el-form :model="model" label-width="120px" :rules="rules" ref="form" size="medium">
      <el-row>
        <el-col :span="24">
          <el-form-item label="自定义名称：" prop="Descpt">
            <el-input v-model="model.Descpt"></el-input>
          </el-form-item>
        </el-col>


      </el-row>

      <el-row>


        <el-col :span="24">
          <el-form-item label="自定义类型：" prop="Name">
            <el-input v-model="model.Name" placeholder="自定义类型编码如:str"></el-input>
          </el-form-item>
        </el-col>
      </el-row>

      <el-row>
        <el-col :span="24">
          <el-form-item label="字段类型：" prop="DataType">
            <el-input v-model="model.DataType" placeholder="数据类型如varchar、int"></el-input>
          </el-form-item>
        </el-col>


      </el-row>

      <el-row>


        <el-col :span="12">
          <el-form-item label="类型长度：" prop="Length">

            <div style="display:flex;align-items:center">
              <el-input-number v-model="model.Length" style="width:100%" placeholder="-1表示可自定义设置长度" :precision="0" controls-position="right" :min="-1"></el-input-number>

              <el-tooltip class="item" effect="dark" content="-1表示可自定义设置长度,0表示不需设置长度" placement="top-start">
                <i class="el-icon-question" style="margin-left:5px;"></i>
              </el-tooltip>
            </div>

          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="类型精度：" prop="Precision">
            <div style="display:flex;align-items:center">
              <el-input-number v-model="model.Precision" style="width:100%" placeholder="0表示不需要设置精度" :precision="0" controls-position="right" :min="0" :max="6"></el-input-number>

              <el-tooltip class="item" effect="dark" content="0表示不需要设置精度" placement="top-start">
                <i class="el-icon-question" style="margin-left:5px;"></i>
              </el-tooltip>
            </div>
          </el-form-item>
        </el-col>

      </el-row>




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
        mixins: [crud({ api: window.$dbFieldTypeApi })],
    data() {

      return {
        emptyModel: {
          Length: 0,
          Precision: 0

        },
        rules: {

          Name: [
            { required: true, message: '请输入自定义类型', trigger: 'blur' }

          ],
          Descpt: [
            { required: true, message: '请输入自定义名称', trigger: 'blur' }

          ],
          DataType: [
            { required: true, message: '请输入字段类型', trigger: 'blur' }

          ],

          Precision: [
            { required: true, message: '请输入精度', trigger: 'blur' }

          ],

          Length: [
            { required: true, message: '请输入长度', trigger: 'blur' }

          ],


        },



      }
    },
    methods: {


    },
    components: {}


  }
</script>



