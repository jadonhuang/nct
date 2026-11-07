<template>

  <el-form-item label="数据表" v-if="isShow">

    <div style="display:flex">
      <el-select v-model="model.Table" placeholder="请选择" filterable>
        <el-option v-for="item in tables"
                   :key="item.Name"
                   :label="item.Name"
                   :value="item.Name">
        </el-option>
      </el-select>
      <el-link :underline="false" type="primary" :disabled="!model.Table" title="编辑表字段" style="margin-left:2px;" icon="el-icon-edit" @click="designer.showTableEditor(model.Table)"></el-link>
    </div>

  </el-form-item>



</template>

<script>

  export default {
    props: {
      designer: Object,
      model: Object,

    },
    computed: {
      isShow() {
        var flag = this.model.Table !== undefined && this.designer.needSelectTable;
        if (flag) {
            var parent = this.designer.getItemParent(this.model);
            flag = flag && !this.designer.isItem(parent);
        }
        return flag;
      },
      tables() {

        if (this.model.Type=='Table')
          return this.designer.designData.DbTables.filter(m => !!m.RefField);

        return this.designer.designData.DbTables;

      },


     
    },
    methods: {


    }


  }



</script>

