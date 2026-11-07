<template>
  <el-dialog title="编辑个人信息" :visible.sync="isShow" :close-on-click-modal="false"  >
    <el-form :model="model" label-width="80px" :rules="rules" ref="addForm" v-loading="loading" size="medium">

      <el-row>
        <el-col :span="24" class="header">
          <el-form-item label="姓名：" prop="RealName">
            <el-input v-model="model.RealName" auto-complete="off"></el-input>
          </el-form-item>
        </el-col>

      </el-row>

      <el-row>
        <el-col :span="24">
          <el-form-item label="性别：" prop="Sex">
            <el-radio-group v-model="model.Sex">
              <el-radio class="radio" :label="1">男</el-radio>
              <el-radio class="radio" :label="2">女</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>

      </el-row>

      <el-row>
        <el-col :span="24">
          <el-form-item label="生日：" prop="Birthday">
            <el-date-picker type="date" style="width:100%" placeholder="选择日期" v-model="model.Birthday"></el-date-picker>
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="头像：" prop="Pic">
        <ImgUpload :value="model.Pic" @success="uploadSuccess" :limitSize="200"></ImgUpload>
      </el-form-item>


      <el-form-item label="地址：" prop="Address">
        <el-input v-model="model.Address"></el-input>
      </el-form-item>

    </el-form>
    <div slot="footer">
      <el-button type="primary" @click.native="doEdit" :loading="loading" size="small">提交</el-button>
      <el-button @click.native="isShow=false" size="small">取消</el-button>

    </div>
  </el-dialog>
</template>
<script>

  export default {

    data() {
      return {
        isShow: false,
        model: {
          RealName: '',
          Sex: 0,
          Birthday: null,
          Address: '',
          Pic: null,

        },

        rules: {
         
          RealName: [
            { required: true, message: '请输入姓名', trigger: 'blur' },
            { max: 20, message: '最多 20 个字符', trigger: 'blur' }
          ],
       
          Birthday: [
            { required: true, message: '请选择生日', trigger: 'change' }
          ],
          Sex: [
            { required: true, message: '请选择性别', trigger: 'blur' }

          ],

        },
      
        loading: false
      

      }
    },

    methods: {

      show() {
        this.isShow = true;
        this.getInfo();

      },

      uploadSuccess: function (img) {
        this.model.Pic = img;
      },

      getInfo() {

        this.model = Object.assign({}, this.$store.state.user);
      },


      doEdit() {
        this.$refs.addForm.validate((valid) => {
          if (!valid) return;
          this.loading = true;
            $userApi.updateUserInfo(this.model).then((res) => {
            if (res.IsSuccess) {

              this.$store.commit('setUser', this.model);
              this.isShow = false;
              this.$okTip("修改成功");

            } else {
              this.$errTip(res.Msg);
            }
            this.loading = false;

          });

        });



      }
    

    },


  }
</script>
