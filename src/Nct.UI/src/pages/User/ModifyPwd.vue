<template>
  <el-dialog title="修改密码" :visible.sync="isShow" :close-on-click-modal="false" width="600px" >
      <el-form :model="model" label-width="100px" :rules="rules" ref="addForm" v-loading="loading" size="medium" >


          <el-form-item label="旧密码：" prop="OldPassword">
              <el-input type="password" v-model="model.OldPassword" auto-complete="off"></el-input>
          </el-form-item>

          <el-form-item label="新密码：" prop="NewPassword">
              <el-input type="password" v-model="model.NewPassword" auto-complete="off"></el-input>
          </el-form-item>

          <el-form-item label="确认密码：" prop="ConfirmPassword">
              <el-input type="password" v-model="model.ConfirmPassword" auto-complete="off"></el-input>
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

          var validatePass = (rule, value, callback) => {
              if (value === '') {
                  callback(new Error('请再次输入密码'));
              } else if (value !== this.model.NewPassword) {
                  callback(new Error('两次输入密码不一致'));
              } else {
                  callback();
              }
          };

      return {
        isShow: false,
        model: {
            OldPassword: '',
            NewPassword: null,
            ConfirmPassword: '',
        },

        rules: {
         
            OldPassword: [
            { required: true, message: '请输入旧密码', trigger: 'blur' }

          ],
       
            NewPassword: [
                { required: true, message: '请输入新密码', trigger: 'blur' },
                { max: 20, message: '长度最多20位', trigger: 'blur' },
                { min: 6, message: '长度最少6位', trigger: 'blur' }
            ],
            ConfirmPassword: [{ validator: validatePass, trigger: 'blur' }
               

            ],

        },
      
        loading: false
      

      }
    },

    methods: {

      show() {
        this.isShow = true;
      
      },

  

      doEdit() {
        this.$refs.addForm.validate((valid) => {
          if (!valid) return;
          this.loading = true;
            $userApi.updatePassword(this.model).then((res) => {
            if (res.IsSuccess) {
              this.isShow = false;
                this.$okTip("修改成功");
                this.$store.commit('logout');
                this.$router.push('/login');

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
