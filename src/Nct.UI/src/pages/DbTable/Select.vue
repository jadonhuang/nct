<template>

  <el-dialog title="数据表选择" :visible.sync="isShow" :close-on-click-modal="false" width="30%" append-to-body>


    <el-form :model="model" label-width="100px" :rules="rules" ref="form" size="medium">

      <el-form-item label="数据表：" prop="Table">
        <el-select v-model="model.Table" placeholder="请选择" filterable class="w100">
          <el-option v-for="item in tables"
                     :key="item.Name"
                     :label="item.Name"
                     :value="item.Name">

          </el-option>
        </el-select>
      </el-form-item>


      <el-form-item label="关联字段：" prop="RefField" v-if="isSubTable" >
        <el-select v-model="model.RefField" placeholder="请选择" filterable class="w100">
          <el-option v-for="item in fields"
                     :key="item.Name"
                     :label="item.Name"
                     :value="item.Name">

          </el-option>
        </el-select>
      </el-form-item>


    </el-form>


    <div slot="footer">

      <el-button type="primary" @click="confirm"  size="small" icon="el-icon-circle-plus-outline">确定</el-button>
      <el-button @click="isShow=false" size="small" icon="el-icon-circle-close">取消</el-button>

    </div>


  </el-dialog>
</template>
<script>



  export default {
    
    data() {
      return {
        model: { Table: '', RefField:'' },  
        isShow: false,
        tables: [],
        fields:[],
        isSubTable: false,
        rules: {
          Table: [
            { required: true, message: '请选择数据表', trigger: 'change' },
           
          ],
          RefField: [
            { required: true, message: '请选择关联字段', trigger: 'change' },

          ],
         


        },
      
      }
    },
    computed :{
     

    },
    watch: {
      "model.Table": function () {

        if (this.isSubTable)
          this.getFields();
      }
    },
    methods: {
      show(isSubTable) {
        this.isShow = true;
        this.isSubTable = isSubTable;
        this.getTables();

      },
      getTables() {

        var queryParams = { pageIndex: 1, pageSize: 1000, Query: {} };

          $dbTableApi.getList(queryParams).then(res => {

          this.tables = res.Data.Rows;

        });
      },
      getFields() {

          $dbTableApi.getCustomFields(this.model.Table).then(res => {
          var fields = res.Data;
          this.fields = fields.filter(m => m.FieldType=='str')

        });

      
      },
      confirm() {
        this.$refs['form'].validate(valid => {

          if (valid) {        
            var tb = { Name: this.model.Table, RefField: this.model.RefField }
            this.$emit('change', tb);
            this.isShow = false;
            this.model.RefField = '';
            this.model.Table = '';
          }

        })


     

      }
   

    },
    mounted() {

    }







  }
</script>

