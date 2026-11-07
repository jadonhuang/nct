<template>
  <el-dialog title="新增数据表" :visible.sync="isShow" :close-on-click-modal="false">
    <el-form :model="model" label-width="100px" :rules="rules" ref="form" size="medium">
      <el-row>
        <el-col :span="12">
          <el-form-item label="表名称：" prop="Name">
            <el-input v-model="model.Name" placeholder="只能包含字母数字和下划线且必须以字母开头"></el-input>
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="表说明：" prop="Descpt">
            <el-input v-model="model.Descpt"></el-input>
          </el-form-item>
        </el-col>
      </el-row>


      <el-form-item label="表字段：" prop="Fields" size="mini">

        <el-table height="300" size="mini" border :data="fields" style="width: 100%; margin-bottom: 2px;" class="edit-table"  @selection-change="selectionChange">
          <el-table-column type="selection" align="center"
                           width="50">
          </el-table-column>
          <el-table-column width="55" type="index" label="序号">
          
          </el-table-column>

          <el-table-column label="名称" min-width="120" class-name="edit-cell">
            <template slot-scope="scope">

              <div class="edit-cell-holder" @click="setCellEdit(scope.$index,0)">
                <div v-if="isCellEdit(scope.$index,0)">

                  <el-input v-model="scope.row.Name" size="mini"></el-input>

                </div>
                <div v-else class="cell-text">
                  {{scope.row.Name}}
                </div>
              </div>

            </template>
          </el-table-column>

          <el-table-column label="类型" min-width="100" class-name="edit-cell">
            <template slot-scope="scope">

              <div class="edit-cell-holder" @click="setCellEdit(scope.$index,1)">
                <div v-if="isCellEdit(scope.$index,1)">

                  <el-select placeholder="请选择" size="mini" v-model="scope.row.FieldType" filterable @change="setLength">
                    <el-option v-for="item in fieldTypes"
                               :key="item.Name"
                               :label="item.Descpt"
                               :value="item.Name">
                    </el-option>
                  </el-select>

                </div>
                <div v-else class="cell-text">
                  {{formatTypeText(scope.row)}}
                </div>
              </div>

            </template>
          </el-table-column>

          <el-table-column label="长度" min-width="80" class-name="edit-cell">
            <template slot-scope="scope">

              <div class="edit-cell-holder" @click="setCellEdit(scope.$index,2)">
                <div v-if="isCellEdit(scope.$index,2)&&hasLength(scope.$index)">

                  <el-input-number v-model="scope.row.Length" :precision="0" :controls="false" :min="0"></el-input-number>

                </div>
                <div v-else class="cell-text">
                  {{scope.row.Length}}
                </div>
              </div>

            </template>
          </el-table-column>


          <el-table-column label="备注" min-width="150" class-name="edit-cell">
            <template slot-scope="scope">

              <div class="edit-cell-holder" @click="setCellEdit(scope.$index,3)">
                <div v-if="isCellEdit(scope.$index,3)">

                  <el-input v-model="scope.row.Remark" size="mini"></el-input>

                </div>
                <div v-else class="cell-text">
                  {{scope.row.Remark}}
                </div>
              </div>

            </template>
          </el-table-column>


        </el-table>
        <div>

          <el-button-group>
            <el-button icon="el-icon-circle-plus-outline" size="small" @click="addField">新增</el-button>
            <el-button icon="el-icon-delete" size="small" @click="delField">删除</el-button>
          </el-button-group>

        </div>

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
        mixins: [crud({ api: window.$dbTableApi })],
    data() {


      var validateFields = (rule, value, callback) => {
        if (this.fields.length === 0) {
          callback(new Error('请添加字段'));
        } else {

          var index = 0;
          var err = '';
          for (var f of this.fields) {
            index++;
            if (this.$isEmptyStr(f.Name)) {
              err = `第${index}行字段名称不能为空`
              break;
            }
            if (this.$isEmptyStr(f.FieldType)) {
              err = `第${index}行字段类型不能为空`
              break;
            }
            if (this.$isEmptyStr(f.Length)) {
              err = `第${index}行字段长度不能为空`
              break;
            }
           
          }
          if (err) {
            callback(new Error(err));
          } else {

            callback();
          }
        }



      };

      return {
        editCellId: '',
        rowIndex:0,
         fields:[],
        fieldTypes: [],
        rules: {
       
          Name: [
            { required: true, message: '请输入名称', trigger: 'blur' }
           
          ],
          Fields: [
            { validator: validateFields }

          ]


        },
    
      


      }
    },
    watch: {
      isShow: function (v) {
        if (!v)
          this.$refs.form.clearValidate();

        this.fields = [];
        if (v&&this.fieldTypes.length == 0)
          this.getFieldTypes();
      },


    },
    methods: {

      addBefore() {
        this.model.Fields = this.fields;

      },

      hasLength(rowIndex) {

        var row = this.fields[rowIndex];
        if (!row.FieldType) return false;

        var type = this.fieldTypes.find(m => m.Name == row.FieldType);
        if (!type) return false;

        return type.Length == -1;


      },
      formatTypeText(row) {
        var type = this.fieldTypes.find(m => m.Name == row.FieldType);
        if (type) return type.Descpt;
        return '';

      },
      setLength(v) {

        var row = this.fields[this.rowIndex];
        if (!row.FieldType) {
          row.Length = 0;
          return;

        }

        var type = this.fieldTypes.find(m => m.Name == row.FieldType);

        var len = type.Length;
        if (len == -1)
          row.Length = 50;
        else
          row.Length = len;
      },

      setCellEdit(rowIndex, colIndex) {
        this.rowIndex = rowIndex;
        var id = `${rowIndex}_${colIndex}`;
        this.editCellId = id;

      },

      isCellEdit(rowIndex, colIndex) {
        var id = `${rowIndex}_${colIndex}`;
        return this.editCellId == id;

      },

      addField() {

        var field = {
          Id: this.$newId(),
          Name: '',
          Descpt:'',
          Remark: '',
          Length:0,
        }

        this.fields.push(field)


      },
      delField() {
        var ids = this.selectIds;
        this.fields = this.fields.filter(m => ids.indexOf(m.Id) == -1);

      },
      getFieldTypes() {
        $dbFieldTypeApi.getList({ pageindex: 1, pagesize: 100, query: {},orderby:"Id" }).then(res => {

          this.fieldTypes = res.Data.Rows;

        });

      }
    
    },
    components: {  }


  }
</script>



