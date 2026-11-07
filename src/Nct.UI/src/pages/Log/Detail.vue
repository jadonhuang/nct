<template>
  <el-dialog title="日志详情" :visible.sync="isShow"  :close-on-click-modal="false" :before-close="hide">
    <el-form :model="model" label-width="120px"  ref="addForm" v-loading="loading">
      <el-row>
        <el-col :span="12" >
          <el-form-item label="请求用户：" >
           {{model.UserName}}
          </el-form-item>
        </el-col>
        <el-col :span="12" >
          <el-form-item label="服务名：">
            {{model.ServiceName}}
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="12">
          <el-form-item label="方法名：">
            {{model.MethodName}}
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="请求时间：">
            {{model.ExecutionTime}}
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="12">
          <el-form-item label="执行时间：">
            {{model.ExecutionDuration}}ms
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="请求IP：">
            {{model.ClientIpAddress}}
          </el-form-item>
        </el-col>
      </el-row>

      <el-row>
        <el-col :span="24">
          <el-form-item label="浏览器信息：">
            {{model.BrowserInfo}}
          </el-form-item>
        </el-col>
       
      </el-row>

      <el-row>
        <el-col :span="24">
          <el-form-item label="参数信息：">
            {{model.Parameters}}
          </el-form-item>
        </el-col>

      </el-row>

      <el-row>
        <el-col :span="24">
          <el-form-item label="执行结果：">
            {{model.Success?"成功":"失败"}}
          </el-form-item>
        </el-col>

      </el-row>

      <el-row v-if="!model.Success">
        <el-col :span="24">
          <el-form-item label="错误信息：">
            {{model.Exception}}
          </el-form-item>
        </el-col>

      </el-row>


       
    </el-form>
    <div slot="footer" class="dialog-footer">
      <el-button @click.native="hide">取消</el-button>
    </div>
  </el-dialog>
</template>
<script>
    import crud from '@/mixins/crud.js'
    export default {
        mixins: [crud({ api: window.$logApi })],

    data() {
      return {
       

      }
    },
    watch: {
    
    },
    methods: {
    

    },
    components: { }


  }
</script>

