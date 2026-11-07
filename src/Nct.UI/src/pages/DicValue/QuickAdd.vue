<template>
    <el-dialog title="批量添加选项" :visible.sync="isShow" :close-on-click-modal="false" width="400px" append-to-body>
        <el-input type="textarea"
                  :rows="10"
                  placeholder="一行一个选项"
                  v-model="str">
        </el-input>

        <div slot="footer">

            <el-button type="primary" @click="save"  size="small" icon="el-icon-circle-check">提交</el-button>
            <el-button @click="isShow=false" size="small" icon="el-icon-circle-close">取消</el-button>

        </div>
    </el-dialog>
</template>
<script>

    export default {

        data() {


            return {
                str: '',
              isShow: false,



            }
        },

      

        methods: {
            
            show() {
                this.isShow = true;
                this.str = '';
            },
            save() {

                if (this.$isEmptyStr(this.str)) {

                    return this.$errTip('请输入选项')

                }
                var options = this.str.split('\n').filter(n => !this.$isEmptyStr(n));
                this.$emit("change", options);
               
                this.isShow = false;

            },


        },


        components: {}


    }
</script>



